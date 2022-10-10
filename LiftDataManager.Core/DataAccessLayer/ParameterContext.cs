namespace LiftDataManager.Core.DataAccessLayer;

public class ParameterContext : DbContext
{
    public DbSet<ParameterDto>? Parameters { get; set; }
    public DbSet<ParameterCategory>? ParameterCategorys { get; set; }
    public DbSet<ParameterTypeCode>? TypeCodes { get; set; }

    private readonly string connectionString = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db;foreign keys = true;";

    public ParameterContext()
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={connectionString}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterContext).Assembly);
    }
}

