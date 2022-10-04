namespace LiftDataManager.Core.DataAccessLayer;

public class ParameterContext : DbContext
{
    public DbSet<ParameterDto> Parameters { get; set; }
    public DbSet<DeliveryType> ParameterTyps { get; set; }
    public DbSet<ParameterCategory> ParameterCategorys { get; set; }
    public DbSet<ParameterTypeCode> TypeCodes { get; set; }

    private readonly string _connectionString;
    private readonly bool _useConsoleLogger;

    //private readonly string connectionString = @"C:\Work\LiftDataParameter2.db;foreign keys = true;";
    private readonly string connectionString = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db;foreign keys = true;";

    public ParameterContext()
    {

    }

    //public ParameterContext(string connectionString, bool useConsoleLogger)
    //{

    //    _connectionString = connectionString;
    //    _useConsoleLogger = useConsoleLogger;

    //    var path = @"C:\Work\";
    //    DbPath = System.IO.Path.Join(path, "LiftDataParameter.db;foreign keys=true;");
    //}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={connectionString}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParameterContext).Assembly);
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

