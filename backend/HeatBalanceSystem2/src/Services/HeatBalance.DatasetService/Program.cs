using HeatBalance.DatasetService.Data;
using HeatBalance.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Dataset")
                       ?? "Data Source=App_Data/datasets.db";
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "App_Data"));

builder.Services.AddDbContext<DatasetDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddHeatBalanceApiDefaults();
builder.Services.AddHeatBalanceJwtAuth(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatasetDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseHeatBalanceApiDefaults();
app.MapGet("/", () => Results.Ok(new { service = "datasets" }));
app.Run();
