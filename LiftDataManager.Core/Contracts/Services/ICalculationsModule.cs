using LiftDataManager.Core.Models.ComponentModels;

namespace LiftDataManager.Core.Contracts.Services;
public interface ICalculationsModule
{
    Dictionary<int, TableRow<int, double>> GetTable(string tableName);

    int GetPersonenCarArea(double area);

    double GetLoadFromTable(double area, string tableName);

    bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem);
}
