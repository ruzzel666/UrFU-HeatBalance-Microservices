var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapReverseProxy();
app.MapGet("/", () => Results.Ok(new
{
    service = "gateway",
    routes = new[]
    {
        "/connect/*",
        "/api/datasets/*",
        "/api/runs/*",
        "/api/reports/*",
        "/api/conveyor/*",
        "/api/chamber/*",
        "/api/electric/*",
        "/api/drum/*"
    }
}));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
