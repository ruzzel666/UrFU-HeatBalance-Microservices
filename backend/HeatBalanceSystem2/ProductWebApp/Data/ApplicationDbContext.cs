using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductWebApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<InputDataSet> InputDataSets => Set<InputDataSet>();
    public DbSet<CalculationRun> CalculationRuns => Set<CalculationRun>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<InputDataSet>(e =>
        {
            e.HasIndex(x => new { x.OwnerId, x.Name }).IsUnique(false);
            e.Property(x => x.InputJson).HasColumnType("jsonb");
            e.HasMany(x => x.Runs).WithOne(x => x.InputDataSet).HasForeignKey(x => x.InputDataSetId);
        });

        builder.Entity<CalculationRun>(e =>
        {
            e.Property(x => x.ResultJson).HasColumnType("jsonb");
            e.HasIndex(x => new { x.InputDataSetId, x.ExecutedAt });
        });
    }

    public override int SaveChanges()
    {
        TouchTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TouchTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void TouchTimestamps()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<InputDataSet>())
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
    }
}

