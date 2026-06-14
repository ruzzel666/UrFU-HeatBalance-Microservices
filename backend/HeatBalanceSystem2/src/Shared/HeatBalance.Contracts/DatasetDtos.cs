namespace HeatBalance.Contracts;

public sealed class DatasetDto
{
    public Guid Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CalculationType CalculationType { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int SchemaVersion { get; set; } = 1;
    public string InputJson { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class CreateDatasetRequest
{
    public string Name { get; set; } = string.Empty;
    public CalculationType CalculationType { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string InputJson { get; set; } = "{}";
}

public sealed class UpdateDatasetRequest
{
    public string Name { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string InputJson { get; set; } = "{}";
}
