using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductWebApp.Data;
using ProductWebApp.ModelBinding;
using ProductWebApp.Services;
using ProductWebApp.Services.Backend;
using QuestPDF.Infrastructure;
using System.IO;
using HeatBalance.Contracts;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

var useMicroservices = builder.Configuration.GetValue<bool>("Microservices:Enabled");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HeatBalanceRunService>();
builder.Services.AddScoped<PdfReportService>();

if (useMicroservices)
{
    builder.Services.AddHttpClient("HeatBalanceGateway");
    builder.Services.AddScoped<IHeatBalanceBackend, RemoteHeatBalanceBackend>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
        })
        .AddOpenIdConnect(options =>
        {
            var authority = builder.Configuration["Microservices:AuthAuthority"] ?? "http://localhost:5001";
            options.Authority = authority;
            options.ClientId = AuthConstants.WebClientId;
            options.ResponseType = "code";
            options.UsePkce = true;
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false;
            options.CallbackPath = "/signin-oidc";
            options.SignedOutCallbackPath = "/signout-callback-oidc";
            options.Scope.Clear();
            foreach (var scope in AuthConstants.WebScopes)
                options.Scope.Add(scope);
        });
}
else
{
    var dbProvider = builder.Configuration["Database:Provider"]?.Trim();
    dbProvider = string.IsNullOrWhiteSpace(dbProvider) ? "Sqlite" : dbProvider;

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (dbProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase) ||
            dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase) ||
            dbProvider.Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
        {
            var cs = builder.Configuration.GetConnectionString("Postgres")
                     ?? builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("Не задана строка подключения PostgreSQL.");
            options.UseNpgsql(cs);
            return;
        }

        var sqliteCs = builder.Configuration.GetConnectionString("Sqlite")
                       ?? "Data Source=App_Data/heat_balance.db";
        options.UseSqlite(sqliteCs);
    });

    builder.Services.AddScoped<IHeatBalanceBackend, LocalHeatBalanceBackend>();

    builder.Services
        .AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole(DbSeed.AdminRoleName, AuthConstants.AdminRole));
});

builder.Services
    .AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/Cabinet");
        options.Conventions.AuthorizeFolder("/Admin", "RequireAdmin");
    })
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(0, new FlexibleNumberModelBinderProvider());
    });

var app = builder.Build();

if (!useMicroservices)
{
    var dbProvider = app.Configuration["Database:Provider"]?.Trim();
    dbProvider = string.IsNullOrWhiteSpace(dbProvider) ? "Sqlite" : dbProvider;
    if (!(dbProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase) ||
          dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase) ||
          dbProvider.Equals("Npgsql", StringComparison.OrdinalIgnoreCase)))
    {
        Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "App_Data"));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (useMicroservices)
{
    app.MapGet("/login", async (HttpContext context) =>
    {
        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = "/" });
    });
    app.MapGet("/logout", async (HttpContext context) =>
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        return Results.Redirect("/");
    });
}

app.MapRazorPages();

if (!useMicroservices)
    await DbSeed.InitializeAsync(app.Services, app.Configuration, app.Logger);

app.Run();
