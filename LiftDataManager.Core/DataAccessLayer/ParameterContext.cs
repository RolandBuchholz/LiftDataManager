namespace LiftDataManager.Core.DataAccessLayer;

public partial class ParameterContext(DbContextOptions<ParameterContext> options) : DbContext(options)
{
    public DbSet<ParameterDto>? ParameterDtos { get; set; }
    public DbSet<DropdownValue>? DropdownValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterContext).Assembly, a =>
            a.Namespace is not null &&
            a.Namespace.StartsWith("LiftDataManager.Core.DataAccessLayer.Configuration"));
    }
}