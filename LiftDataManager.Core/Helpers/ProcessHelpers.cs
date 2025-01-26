using Microsoft.Data.Sqlite;
using System.Diagnostics;
namespace LiftDataManager.Core.Helpers;

public static class ProcessHelpers
{
    const string workPathDb = @"C:\Work\Administration\DataBase\LiftDataParameter.db";

    public static void StartProgram(string filename, string startargs)
    {
        using Process p = new();
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.FileName = filename;
        p.StartInfo.Arguments = startargs;
        p.Start();
    }
    public static async Task<int> StartProgramWithExitCodeAsync(string filename, string startargs)
    {
        using Process p = new();
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.FileName = filename;
        p.StartInfo.Arguments = startargs;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.Start();
        await p.WaitForExitAsync();
        return p.ExitCode;
    }

    public static void MakeBackupFile(string? fullPath, string? orderNumber)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            return;
        }
        if (!File.Exists(fullPath))
        {
            return;
        }
        var backupPath = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrWhiteSpace(backupPath))
        {
            return;
        }
        var newName = Path.Combine(backupPath, orderNumber + "-LDM_Backup" + Path.GetExtension(fullPath));
        if (newName is not null && Path.IsPathFullyQualified(newName))
        {
            if (File.Exists(newName))
            {
                FileInfo backupFileInfo = new(newName);
                if (backupFileInfo.IsReadOnly)
                {
                    backupFileInfo.IsReadOnly = false;
                }
            }
            File.Copy(fullPath, newName, true);
        }
    }

    public static async Task<bool> CopyDataBaseToWorkSpace(DbContext dbContext)
    {
        if (!Directory.Exists(Path.GetDirectoryName(workPathDb)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workPathDb)!);
        }

        var connectionString = dbContext.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }
        string? dbPath;
        try
        {
            var pathArray = connectionString.Split(';');
            if (pathArray.Length == 0)
            {
                return false;
            }
            dbPath = pathArray[0];
            if (dbPath.Contains('\\'))
            {
                dbPath = dbPath[13..connectionString.LastIndexOf('"')];
            }
            else
            {
                dbPath = dbPath.Replace("Data Source=", "");
            }
        }
        catch
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(dbPath))
        {
            return false;
        }
        if (dbContext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        dbContext.Database.CloseConnection();
        await Task.Delay(1500);
        try
        {
            File.Copy(dbPath, workPathDb, true);
        }
        catch
        {
            return false;
        }
        await Task.CompletedTask;
        return true;
    }

    public static string CreateOrderFolderStructure(string rootPath, string orderName, bool createXml)
    {
        var pathList = new List<string>
        {
            Path.Join(rootPath, orderName),
            Path.Join(rootPath, orderName, "Berechnungen", "PDF"),
            Path.Join(rootPath, orderName, "Bestellungen"),
            Path.Join(rootPath, orderName, "Bgr00", "CAD-CFP"),
            Path.Join(rootPath, orderName, "Fotos"),
            Path.Join(rootPath, orderName, "SV"),
            Path.Join(rootPath, orderName, "Montage-TÜV-Dokumentation", "TÜV", "Zertifikate")
        };

        foreach (var path in pathList)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        if (!createXml)
        {
            return string.Empty;
        }
        else 
        {
            var newOrderFileName = Path.Join(rootPath, orderName, $"{orderName}-AutoDeskTransfer.xml");
            File.Copy(@"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml", newOrderFileName, false);
            return newOrderFileName;
        }
    }
}