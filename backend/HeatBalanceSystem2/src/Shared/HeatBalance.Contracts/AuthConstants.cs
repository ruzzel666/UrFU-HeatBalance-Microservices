namespace HeatBalance.Contracts;

public static class AuthConstants
{
    public const string ApiAudience = "heatbalance-api";
    public const string WebClientId = "heatbalance-web";
    public const string RunServiceClientId = "heatbalance-run-service";
    public const string RunServiceClientSecret = "run-service-secret-dev";

    public const string AdminRole = "Admin";

    public static readonly string[] ApiScopes =
    [
        "datasets:read",
        "datasets:write",
        "datasets:delete",
        "runs:read",
        "runs:execute",
        "conveyor:calculate",
        "chamber:calculate",
        "electric:calculate",
        "drum:calculate",
        "reports:generate",
        "admin"
    ];

    public static readonly string[] WebScopes =
    [
        "openid",
        "profile",
        "email",
        "roles",
        "datasets:read",
        "datasets:write",
        "datasets:delete",
        "runs:read",
        "runs:execute",
        "reports:generate"
    ];

    public static readonly string[] RunServiceScopes =
    [
        "datasets:read",
        "conveyor:calculate",
        "chamber:calculate",
        "electric:calculate",
        "drum:calculate"
    ];
}

public static class AuthPolicies
{
    public const string DatasetsRead = "datasets:read";
    public const string DatasetsWrite = "datasets:write";
    public const string DatasetsDelete = "datasets:delete";
    public const string RunsRead = "runs:read";
    public const string RunsExecute = "runs:execute";
    public const string ReportsGenerate = "reports:generate";
    public const string Admin = "admin";
    public const string ConveyorCalculate = "conveyor:calculate";
    public const string ChamberCalculate = "chamber:calculate";
    public const string ElectricCalculate = "electric:calculate";
    public const string DrumCalculate = "drum:calculate";
}
