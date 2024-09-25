using Microsoft.EntityFrameworkCore.Design;

namespace LiftDataManager.Core.DataAccessLayer;
public class ParameterDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ParameterContext>
{
    private readonly string connectionString = @"Data Source=\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db;foreign keys = false;";

    public ParameterContext CreateDbContext(string[] args)
    {
        DbContextOptions<ParameterContext> options = new DbContextOptionsBuilder<ParameterContext>().UseSqlite(connectionString).Options;

        return new ParameterContext(options);
    }
}
