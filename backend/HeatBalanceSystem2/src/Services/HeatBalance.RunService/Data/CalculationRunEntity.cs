using System.ComponentModel.DataAnnotations;

namespace HeatBalance.RunService.Data;

public sealed class CalculationRunEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid InputDataSetId { get; set; }

    [Required]
    public string OwnerId { get; set; } = default!;

    public DateTimeOffset ExecutedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public string ResultJson { get; set; } = "{}";

    [StringLength(2000)]
    public string? Notes { get; set; }
}
