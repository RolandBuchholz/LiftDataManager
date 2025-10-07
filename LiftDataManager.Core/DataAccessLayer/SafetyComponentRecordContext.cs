using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer;

public partial class SafetyComponentRecordContext(DbContextOptions<SafetyComponentRecordContext> options) : DbContext(options)
{
    public DbSet<SafetyComponentRecord>? SafetyComponentRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SafetyComponentRecordContext).Assembly, a => 
            a.Namespace is not null && 
            a.Namespace.StartsWith("LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration"));
    }
}