namespace LiftDataManager.Core.DataAccessLayer;

public class ParameterContext : DbContext
{
    public DbSet<ParameterDto>? Parameters { get; set; }
    public DbSet<ParameterCategory>? ParameterCategorys { get; set; }
    public DbSet<ParameterTypeCode>? TypeCodes { get; set; }

    public ParameterContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterContext).Assembly);
    }
}