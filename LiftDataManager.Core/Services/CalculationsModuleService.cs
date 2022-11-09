using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.ComponentModels;

namespace LiftDataManager.Core.Services;

public class CalculationsModuleService : ICalculationsModule
{
    public bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem)
    {
        var loadTable6 = GetLoadFromTable(area, "Tabelle6");
        var loadTable7 = GetLoadFromTable(area, "Tabelle7");

        if (load >= loadTable6) return true;
        if (cargotyp == "Lastenaufzug" && drivesystem == "Hydraulik" && load >= loadTable7 ) return true;
        return false;
    }

    public double GetLoadFromTable(double area, string tableName)
    {
        var table = tableName switch
        {
            "Tabelle6" => GetTable("Tabelle6"),
            "Tabelle7" => GetTable("Tabelle7"),
            _ => GetTable("Tabelle6"),
        };

        TableRow<int, double>? nutzlast = null;
        if (table == null)
        { return 0; };
        if (area <= 0)
        { return 0; };

        if (tableName == "Tabelle6" && area > 5.0)
        {
            return 2500 + (area - 5.0) / 0.16 * 100;
        };

        if (tableName == "Tabelle6" && area < 0.37)
        {
            return 0;
        };

        if (tableName == "Tabelle7" && area > 5.04)
        {
            return 1600 + (area - 5.04) / 0.40 * 100;
        };

        if (tableName == "Tabelle7" && area < 1.68)
        {
            return 0;
        };

        if (table.Any(x => x.Value.SecondValue == area))
        {
            nutzlast = table.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return nutzlast.FirstValue;
        };

        var lowTableEntry = table.Where(x => x.Value.SecondValue < area).Last();
        var highTableEntry = table.Where(x => x.Value.SecondValue > area).First();
        return Math.Round(lowTableEntry.Value.FirstValue + (highTableEntry.Value.FirstValue - lowTableEntry.Value.FirstValue) /
                (highTableEntry.Value.SecondValue - lowTableEntry.Value.SecondValue) * (area - lowTableEntry.Value.SecondValue)); 
    }

    public int GetPersonenCarArea(double area)
    {
        var table = GetTable("Tabelle8");
        TableRow<int, double>? personenAnzahl = null;
        if (table == null)
        { return 0; };
        if (area < 0.28)
        { return 0; };

        if (area > 3.13)
        {
            return Convert.ToInt32(20 + (area - 3.13) / 0.115);
        };

        if (table.Any(x => x.Value.SecondValue == area))
        {
            personenAnzahl = table.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return personenAnzahl.FirstValue;
        };
        personenAnzahl = table.Where(x => x.Value.SecondValue < area).Last().Value;
        return personenAnzahl.FirstValue;
    }

    public Dictionary<int, TableRow<int, double>> GetTable(string tableName)
    {
        KeyValuePair<int, double>[] table;
        switch (tableName)
        {
            case "Tabelle6":
                table = new KeyValuePair<int, double>[]
                {
                    new KeyValuePair<int, double>(100, 0.37),
                    new KeyValuePair<int, double>(180, 0.58),
                    new KeyValuePair<int, double>(225, 0.70),
                    new KeyValuePair<int, double>(300, 0.90),
                    new KeyValuePair<int, double>(375, 1.10),
                    new KeyValuePair<int, double>(400, 1.17),
                    new KeyValuePair<int, double>(450, 1.30),
                    new KeyValuePair<int, double>(525, 1.45),
                    new KeyValuePair<int, double>(600, 1.60),
                    new KeyValuePair<int, double>(630, 1.66),
                    new KeyValuePair<int, double>(675, 1.75),
                    new KeyValuePair<int, double>(750, 1.90),
                    new KeyValuePair<int, double>(800, 2.00),
                    new KeyValuePair<int, double>(825, 2.05),
                    new KeyValuePair<int, double>(900, 2.20),
                    new KeyValuePair<int, double>(975, 2.35),
                    new KeyValuePair<int, double>(1000, 2.40),
                    new KeyValuePair<int, double>(1050, 2.50),
                    new KeyValuePair<int, double>(1125, 2.65),
                    new KeyValuePair<int, double>(1200, 2.80),
                    new KeyValuePair<int, double>(1250, 2.90),
                    new KeyValuePair<int, double>(1275, 2.95),
                    new KeyValuePair<int, double>(1350, 3.10),
                    new KeyValuePair<int, double>(1425, 3.25),
                    new KeyValuePair<int, double>(1500, 3.40),
                    new KeyValuePair<int, double>(1600, 3.56),
                    new KeyValuePair<int, double>(2000, 4.20),
                    new KeyValuePair<int, double>(2500, 5.00)
                };
                return SetTableData(table, "kg", "m²");
            case "Tabelle7":
                 table = new KeyValuePair<int, double>[]
                {
                    new KeyValuePair<int, double>(400, 1.68),
                    new KeyValuePair<int, double>(450, 1.84),
                    new KeyValuePair<int, double>(525, 2.08),
                    new KeyValuePair<int, double>(600, 2.32),
                    new KeyValuePair<int, double>(630, 2.42),
                    new KeyValuePair<int, double>(675, 2.56),
                    new KeyValuePair<int, double>(750, 2.80),
                    new KeyValuePair<int, double>(800, 2.96),
                    new KeyValuePair<int, double>(825, 3.04),
                    new KeyValuePair<int, double>(900, 3.28),
                    new KeyValuePair<int, double>(975, 3.52),
                    new KeyValuePair<int, double>(1000, 3.60),
                    new KeyValuePair<int, double>(1050, 3.72),
                    new KeyValuePair<int, double>(1125, 3.90),
                    new KeyValuePair<int, double>(1200, 4.08),
                    new KeyValuePair<int, double>(1250, 4.20),
                    new KeyValuePair<int, double>(1275, 4.26),
                    new KeyValuePair<int, double>(1350, 4.44),
                    new KeyValuePair<int, double>(1425, 4.62),
                    new KeyValuePair<int, double>(1500, 4.80),
                    new KeyValuePair<int, double>(1600, 5.04)
                };
                return SetTableData(table, "kg", "m²");
            case "Tabelle8":

                 table = new KeyValuePair<int, double>[] 
                 {
                    new KeyValuePair<int, double>(1, 0.28),
                    new KeyValuePair<int, double>(2, 0.49),
                    new KeyValuePair<int, double>(3, 0.60),
                    new KeyValuePair<int, double>(4, 0.79),
                    new KeyValuePair<int, double>( 5, 0.98),
                    new KeyValuePair<int, double>( 6, 1.17),
                    new KeyValuePair<int, double>( 7, 1.31),
                    new KeyValuePair<int, double>(8, 1.45),
                    new KeyValuePair<int, double>(9, 1.59),
                    new KeyValuePair<int, double>(10, 1.73),
                    new KeyValuePair<int, double>(11, 1.87),
                    new KeyValuePair<int, double>(12, 2.01),
                    new KeyValuePair<int, double>(13, 2.15),
                    new KeyValuePair<int, double>(14, 2.29),
                    new KeyValuePair<int, double>(15, 2.43),
                    new KeyValuePair<int, double>(16, 2.57),
                    new KeyValuePair<int, double>(17, 2.71),
                    new KeyValuePair<int, double>(18, 2.85),
                    new KeyValuePair<int, double>(19, 2.99),
                    new KeyValuePair<int, double>(20, 3.13)
                 };
                return SetTableData(table, "Pers.", "m²");
            default:
                return new Dictionary<int, TableRow<int, double>>();
        }
    }

    private static Dictionary<int, TableRow<int, double>> SetTableData(KeyValuePair<int, double>[] tabledata, string firstUnit, string secondUnit)
    {
        var dic = new Dictionary<int, TableRow<int, double>>();

        foreach (var item in tabledata)
        {
            dic.Add(item.Key, new TableRow<int, double>
            {
                FirstValue = item.Key,
                SecondValue = item.Value,
                FirstUnit = firstUnit,
                SecondUnit = secondUnit
            });
        }
        return dic;
    }
}