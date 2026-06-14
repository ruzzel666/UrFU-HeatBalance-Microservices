using Microsoft.EntityFrameworkCore;

namespace HeatBalance.DatasetService.Data;

public sealed class DatasetDbContext : DbContext
{
    public DatasetDbContext(DbContextOptions<DatasetDbContext> options) : base(options)
    {
    }

    public DbSet<InputDataSetEntity> InputDataSets => Set<InputDataSetEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<InputDataSetEntity>(e =>
        {
            e.HasIndex(x => new { x.OwnerId, x.Name });
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<InputDataSetEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
