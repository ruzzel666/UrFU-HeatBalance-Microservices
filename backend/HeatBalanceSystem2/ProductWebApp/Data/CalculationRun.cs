using System.ComponentModel.DataAnnotations;

namespace ProductWebApp.Data;

public class CalculationRun
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid InputDataSetId { get; set; }
    public InputDataSet InputDataSet { get; set; } = default!;

    public DateTimeOffset ExecutedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public string ResultJson { get; set; } = "{}";

    [StringLength(2000)]
    public string? Notes { get; set; }
}

