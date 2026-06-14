using System.ComponentModel.DataAnnotations;
using HeatBalance.Contracts;

namespace HeatBalance.DatasetService.Data;

public sealed class InputDataSetEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string OwnerId { get; set; } = default!;

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public CalculationType CalculationType { get; set; }

    [StringLength(4000)]
    public string Comment { get; set; } = string.Empty;

    public int SchemaVersion { get; set; } = 1;

    [Required]
    public string InputJson { get; set; } = "{}";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DatasetDto ToDto() => new()
    {
        Id = Id,
        OwnerId = OwnerId,
        Name = Name,
        CalculationType = CalculationType,
        Comment = Comment,
        SchemaVersion = SchemaVersion,
        InputJson = InputJson,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt
    };
}
