using Microsoft.EntityFrameworkCore;

namespace HeatBalance.RunService.Data;

public sealed class RunDbContext : DbContext
{
    public RunDbContext(DbContextOptions<RunDbContext> options) : base(options)
    {
    }

    public DbSet<CalculationRunEntity> CalculationRuns => Set<CalculationRunEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CalculationRunEntity>(e =>
        {
            e.HasIndex(x => new { x.InputDataSetId, x.ExecutedAt });
            e.HasIndex(x => x.OwnerId);
        });
    }
}
