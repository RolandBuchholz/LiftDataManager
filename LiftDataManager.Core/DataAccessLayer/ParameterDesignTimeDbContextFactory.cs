using Microsoft.EntityFrameworkCore.Design;

namespace LiftDataManager.Core.DataAccessLayer;
public class ParameterDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ParameterContext>
{
    private readonly string connectionString = @"Data Source=\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db;foreign keys = true;";

    public ParameterContext CreateDbContext(string[] args)
    {
        DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(connectionString).Options;

        return new ParameterContext(options);
    }
}
