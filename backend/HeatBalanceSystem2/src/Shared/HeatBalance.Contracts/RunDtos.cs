namespace HeatBalance.Contracts;

public sealed class CalculationRunDto
{
    public Guid Id { get; set; }
    public Guid InputDataSetId { get; set; }
    public DateTimeOffset ExecutedAt { get; set; }
    public string ResultJson { get; set; } = "{}";
    public string? Notes { get; set; }
    public DatasetDto? Dataset { get; set; }
}

public sealed class CreateRunRequest
{
    public Guid InputDataSetId { get; set; }
}

public sealed class CreateRunResponse
{
    public Guid RunId { get; set; }
    public string ResultJson { get; set; } = "{}";
}
