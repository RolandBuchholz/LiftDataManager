using System.Diagnostics;
namespace LiftDataManager.Core.Helpers;

public static class ProcessHelpers
{
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
}
