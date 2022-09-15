using System.Diagnostics;
using System.Text.Json;
using LiftDataManager.Core.Contracts.Services;

namespace LiftDataManager.Core.Services;

public class VaultDataService : IVaultDataService
{
    private string starttyp = string.Empty;
    private const string pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";
    private DownloadInfo DownloadInfo { get; set; } = new DownloadInfo();
    public int ExitCode { get; private set;}

    public async Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly)
    {
        starttyp = "get";

        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer, readOnly);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        return DownloadInfo;
    }

    public async Task<DownloadInfo> SetFileAsync(string auftragsnummer)
    {
        starttyp = "set";
        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        return DownloadInfo;
    }

    public async Task<DownloadInfo> UndoFileAsync(string auftragsnummer)
    {
        starttyp = "undo";
        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        return DownloadInfo;
    }

    private async Task<int> StartPowershellScriptAsync(string pathPowershellScripts, string starttyp, string auftragsnummer, bool readOnly = false)
    {
        var powershellScriptName = starttyp switch
        {
            "get" => "GetVaultFile.ps1",
            "set" => "SetVaultFile.ps1",
            "undo" => "UndoVaultFile.ps1",
            _ => "GetVaultFile.ps1",
        };
        var readOnlyPowershell = readOnly ? "$true" : "$false";

        try
        {
            using Process psScript = new();
            psScript.StartInfo.UseShellExecute = false;
            psScript.StartInfo.FileName = "PowerShell.exe";
            if (starttyp == "get")
            {
                psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {readOnlyPowershell}";
            }
            else
            {
                psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
            }
            psScript.StartInfo.CreateNoWindow = true;
            psScript.StartInfo.RedirectStandardOutput = true;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            psScript.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
            psScript.Start();
            var downloadResult = psScript.StandardOutput.ReadToEnd();
            psScript.WaitForExit();

            if (!string.IsNullOrWhiteSpace(downloadResult))
            {
                try
                {
                    var result = JsonSerializer.Deserialize<DownloadInfo>(downloadResult.Split("---DownloadInfo---")[1]);
                    if (result is not null)
                        DownloadInfo = result;
                }
                catch
                {
                    Console.WriteLine("Keine DownloadInfo gefunden");
                }
            }

            Console.WriteLine($"Vault PowershellScript: {powershellScriptName} finished ...");
            await Task.CompletedTask;
            return psScript.ExitCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await Task.CompletedTask;
            return 4;
        }
    }

}