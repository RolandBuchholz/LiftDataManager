using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Models.ComponentModels;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.Core.Services;

public partial class CalculationsModuleService : ICalculationsModule
{
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<CalculationsModuleService> _logger;

    public Dictionary<int, TableRow<int, double>> Table6 { get; }
    public Dictionary<int, TableRow<int, double>> Table7 { get; }
    public Dictionary<int, TableRow<int, double>> Table8 { get; }

    public CalculationsModuleService(ParameterContext parametercontext, ILogger<CalculationsModuleService> logger)
    {
        _parametercontext = parametercontext;
        _logger = logger;
        var loadTable6 = _parametercontext.Set<LoadTable6>().ToArray();
        var loadTable7 = _parametercontext.Set<LoadTable7>().ToArray();
        var personsTable8 = _parametercontext.Set<PersonsTable8>().ToArray();

        Table6 = SetTableData(loadTable6, "kg", "m²");
        Table7 = SetTableData(loadTable7, "kg", "m²");
        Table8 = SetTableData(personsTable8, "Pers.", "m²");
    }

    public bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem)
    {
        var loadTable6 = GetLoadFromTable(area, "Tabelle6");
        var loadTable7 = GetLoadFromTable(area, "Tabelle7");

        if (load >= loadTable6) return true;
        if (cargotyp == "Lastenaufzug" && drivesystem == "Hydraulik")
        {
            if (loadTable7 > 0)
            {
                LogTabledata("Tabelle 7", load);
                return load >= loadTable7;
            }
            else
            {
                LogTabledata("Tabelle 6", load);
                return load >= loadTable6;
            }
        }
        return false;
    }

    public double GetLoadFromTable(double area, string tableName)
    {
        var table = tableName switch
        {
            "Tabelle6" => Table6,
            "Tabelle7" => Table7,
            _ => Table6,
        };

        TableRow<int, double>? nutzlast = null;
        if (table == null)  return 0; 
        if (area <= 0)  return 0; 
        if (tableName == "Tabelle6" && area > 5.0) return Math.Round(2500 + (area - 5.0) / 0.16 * 100,0);
        if (tableName == "Tabelle6" && area < 0.37) return 0;
        if (tableName == "Tabelle7" && area > 5.04) return Math.Round(1600 + (area - 5.04) / 0.40 * 100, 0);
        if (tableName == "Tabelle7" && area < 1.68) return 0;
        if (table.Any(x => x.Value.SecondValue == area))
        {
            nutzlast = table.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return nutzlast.FirstValue;
        };
        var lowTableEntry = table.Where(x => x.Value.SecondValue < area).Last();
        var highTableEntry = table.Where(x => x.Value.SecondValue > area).First();
        return Math.Round(lowTableEntry.Value.FirstValue + (highTableEntry.Value.FirstValue - lowTableEntry.Value.FirstValue) /
                (highTableEntry.Value.SecondValue - lowTableEntry.Value.SecondValue) * (area - lowTableEntry.Value.SecondValue),0); 
    }

    public int GetPersonenCarArea(double area)
    {
        TableRow<int, double>? personenAnzahl = null;
        if (Table8 == null) return 0; 
        if (area < 0.28) return 0;
        if (area > 3.13) return Convert.ToInt32(20 + (area - 3.13) / 0.115);
        if (Table8.Any(x => x.Value.SecondValue == area))
        {
            personenAnzahl = Table8.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return personenAnzahl.FirstValue;
        };
        personenAnzahl = Table8.Where(x => x.Value.SecondValue < area).Last().Value;
        LogTabledata("Tabelle 8", personenAnzahl.FirstValue);
        return personenAnzahl.FirstValue;
    }

    private static Dictionary<int, TableRow<int, double>> SetTableData(object[]? tabledata, string firstUnit, string secondUnit)
    {
        var dic = new Dictionary<int, TableRow<int, double>>();
        if (tabledata is null) return dic;
        switch (tabledata.GetType().Name)
        {
            case "LoadTable6[]":
                foreach (var item in (LoadTable6[])tabledata)
                {
                    dic.Add(item.Load, new TableRow<int, double>
                    {
                        FirstValue = item.Load,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            case "LoadTable7[]":
                foreach (var item in (LoadTable7[])tabledata)
                {
                    dic.Add(item.Load, new TableRow<int, double>
                    {
                        FirstValue = item.Load,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            case "PersonsTable8[]":
                foreach (var item in (PersonsTable8[])tabledata)
                {
                    dic.Add(item.Persons, new TableRow<int, double>
                    {
                        FirstValue = item.Persons,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            default:
                break;
        }
        return dic;
    }

    [LoggerMessage(60121, LogLevel.Debug,
    "table {tableName} with {load}loaded")]
    partial void LogTabledata(string tableName, double load);
}