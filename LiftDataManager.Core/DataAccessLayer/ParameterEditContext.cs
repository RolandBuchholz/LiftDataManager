namespace LiftDataManager.Core.DataAccessLayer;

public partial class ParameterEditContext(DbContextOptions<ParameterEditContext> options) : DbContext(options)
{
    public DbSet<ParameterDto>? ParameterDtos { get; set; }
    public DbSet<DropdownValue>? DropdownValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterEditContext).Assembly, a =>
            a.Namespace is not null &&
            a.Namespace.StartsWith("LiftDataManager.Core.DataAccessLayer.Configuration"));
    }
}