namespace LiftDataManager.Core.DataAccessLayer;

public class ParameterContext : DbContext
{
    public DbSet<ParameterDto>? ParameterDtos { get; set; }
    public DbSet<DropdownValue>? DropdownValues { get; set; }

    public ParameterContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterContext).Assembly);
    }
}