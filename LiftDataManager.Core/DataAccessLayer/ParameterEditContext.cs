namespace LiftDataManager.Core.DataAccessLayer;

public class ParameterEditContext : DbContext
{
    public DbSet<ParameterDto>? ParameterDtos { get; set; }
    public DbSet<DropdownValue>? DropdownValues { get; set; }

    public ParameterEditContext(DbContextOptions<ParameterEditContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterEditContext).Assembly);
    }
}