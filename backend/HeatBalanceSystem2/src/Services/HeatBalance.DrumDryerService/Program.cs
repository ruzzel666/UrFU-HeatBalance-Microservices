using HeatBalance.Contracts;
using HeatBalance.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHeatBalanceApiDefaults();
builder.Services.AddHeatBalanceJwtAuth(builder.Configuration);

var app = builder.Build();
app.UseHeatBalanceApiDefaults();
app.MapGet("/", () => Results.Ok(new { service = "DrumDryer", policy = AuthPolicies.DrumCalculate }));
app.Run();
