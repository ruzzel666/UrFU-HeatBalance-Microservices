using HeatBalance.RunService.Data;
using HeatBalance.RunService.Services;
using HeatBalance.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Runs")
                       ?? "Data Source=App_Data/runs.db";
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "App_Data"));

builder.Services.AddDbContext<RunDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ServiceTokenProvider>();
builder.Services.AddScoped<FurnaceCalculationClient>();
builder.Services.AddScoped<DatasetApiClient>();
builder.Services.AddHeatBalanceApiDefaults();
builder.Services.AddHeatBalanceJwtAuth(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RunDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseHeatBalanceApiDefaults();
app.MapGet("/", () => Results.Ok(new { service = "runs" }));
app.Run();
