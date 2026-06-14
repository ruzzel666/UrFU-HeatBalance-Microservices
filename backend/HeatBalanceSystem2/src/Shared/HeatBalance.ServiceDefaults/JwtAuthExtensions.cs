using HeatBalance.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeatBalance.ServiceDefaults;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddHeatBalanceJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Auth:Authority"] ?? "http://localhost:5001";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = "sub";
                options.TokenValidationParameters.RoleClaimType = "role";
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.DatasetsRead, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.DatasetsRead));
            options.AddPolicy(AuthPolicies.DatasetsWrite, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.DatasetsWrite));
            options.AddPolicy(AuthPolicies.DatasetsDelete, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.DatasetsDelete));
            options.AddPolicy(AuthPolicies.RunsRead, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.RunsRead));
            options.AddPolicy(AuthPolicies.RunsExecute, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.RunsExecute));
            options.AddPolicy(AuthPolicies.ReportsGenerate, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.ReportsGenerate));
            options.AddPolicy(AuthPolicies.Admin, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.Admin));
            options.AddPolicy(AuthPolicies.ConveyorCalculate, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.ConveyorCalculate));
            options.AddPolicy(AuthPolicies.ChamberCalculate, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.ChamberCalculate));
            options.AddPolicy(AuthPolicies.ElectricCalculate, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.ElectricCalculate));
            options.AddPolicy(AuthPolicies.DrumCalculate, p => p.RequireAuthenticatedUser().RequireScope(AuthPolicies.DrumCalculate));
        });

        return services;
    }

    public static IServiceCollection AddHeatBalanceApiDefaults(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        return services;
    }

    public static WebApplication UseHeatBalanceApiDefaults(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        return app;
    }
}
