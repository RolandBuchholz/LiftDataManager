using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer;

public class SafetyComponentRecordContext : DbContext
{
    public DbSet<SafetyComponentRecord>? SafetyComponentsRecords { get; set; }

    public SafetyComponentRecordContext(DbContextOptions<SafetyComponentRecordContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SafetyComponentRecordContext).Assembly);
    }
}