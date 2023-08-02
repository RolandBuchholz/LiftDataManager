using LiftDataManager.Core.Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace LiftDataManager.Core.Services;

public class VaultDataService : IVaultDataService
{
    private string starttyp = string.Empty;
    private const string pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";
    private readonly ILogger<VaultDataService> _logger;
    private DownloadInfo DownloadInfo { get; set; } = new DownloadInfo();
    public int ExitCode { get; private set; }

    public VaultDataService(ILogger<VaultDataService> logger)
    {
        _logger = logger;
    }

    public async Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly, bool customFile)
    {
        starttyp = "get";
        _logger.LogInformation(60111, "start read data from vaultserver");
        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer, readOnly, customFile);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        _logger.LogInformation(60111, "finished read data from vaultserver");
        return DownloadInfo;
    }

    public async Task<DownloadInfo> SetFileAsync(string auftragsnummer, bool customFile)
    {
        starttyp = "set";
        _logger.LogInformation(60112, "start save data to vaultserver");
        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer, false, customFile);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        _logger.LogInformation(60112, "finished save data to vaultserver");
        return DownloadInfo;
    }

    public async Task<DownloadInfo> UndoFileAsync(string auftragsnummer, bool customFile)
    {
        starttyp = "undo";
        _logger.LogInformation(60113, "start undo data vaultserver");
        var result = StartPowershellScriptAsync(pathPowershellScripts, starttyp, auftragsnummer, false, customFile);
        ExitCode = result.Result;
        DownloadInfo.ExitCode = ExitCode;
        await Task.CompletedTask;
        _logger.LogInformation(60113, "finished undo data vaultserver");
        return DownloadInfo;
    }

    private async Task<int> StartPowershellScriptAsync(string pathPowershellScripts, string starttyp, string auftragsnummer, bool readOnly = false, bool customFile = false)
    {
        var powershellScriptName = starttyp switch
        {
            "get" => "GetVaultFile.ps1",
            "set" => "SetVaultFile.ps1",
            "undo" => "UndoVaultFile.ps1",
            _ => "GetVaultFile.ps1",
        };
        var readOnlyPowershell = readOnly ? "$true" : "$false";
        var customFilePowershell = customFile ? "$true" : "$false";

        _logger.LogInformation(60114, "start powershellScript with {starttyp}", starttyp);
        try
        {
            using Process psScript = new();
            psScript.StartInfo.UseShellExecute = false;
            psScript.StartInfo.FileName = "PowerShell.exe";
            if (starttyp == "get")
            {
                psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {readOnlyPowershell} {customFilePowershell}";
            }
            else
            {
                psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {customFilePowershell}";
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
                    _logger.LogWarning(61014, "powershellScript no downloadInfo found");
                    Console.WriteLine("Keine DownloadInfo gefunden");
                }
            }

            Console.WriteLine($"Vault PowershellScript: {powershellScriptName} finished ...");
            await Task.CompletedTask;
            _logger.LogInformation(60114, "finished powershellScript with {ExitCode}", psScript.ExitCode);
            return psScript.ExitCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            _logger.LogError(61114, "powershellScript throw Exception: {Message}", e.Message);
            await Task.CompletedTask;
            return 4;
        }
    }
}