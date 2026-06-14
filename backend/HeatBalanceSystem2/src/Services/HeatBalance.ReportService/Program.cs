using HeatBalance.ReportService.Services;
using HeatBalance.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<PdfReportBuilder>();
builder.Services.AddScoped<RunApiClient>();
builder.Services.AddScoped<DatasetApiClient>();
builder.Services.AddHeatBalanceApiDefaults();
builder.Services.AddHeatBalanceJwtAuth(builder.Configuration);

var app = builder.Build();
app.UseHeatBalanceApiDefaults();
app.MapGet("/", () => Results.Ok(new { service = "reports" }));
app.Run();
