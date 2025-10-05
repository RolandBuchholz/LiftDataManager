using Microsoft.EntityFrameworkCore.Design;

namespace LiftDataManager.Core.DataAccessLayer;
public class SafetyComponentRecordDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SafetyComponentRecordContext>
{
    private readonly string connectionString = @"Data Source=\\Bauer\aufträge neu\Vorlagen\DataBase\SafetyComponentRecords.db;foreign keys = false;";

    public SafetyComponentRecordContext CreateDbContext(string[] args)
    {
        DbContextOptions<SafetyComponentRecordContext> options = new DbContextOptionsBuilder<SafetyComponentRecordContext>().UseSqlite(connectionString).Options;

        return new SafetyComponentRecordContext(options);
    }
}
