using LiftDataManager.Core.Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace LiftDataManager.Core.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="IFilesService"/> <see langword="interface"/> using UWP APIs.
/// </summary>
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

    /// <inheritdoc/>
    public async Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly, bool customFile = false)
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<(long, DownloadInfo)> GetAutoDeskTransferAsync(string liftNumber, SpezifikationTyp spezifikationTyp, bool readOnly = true)
    {
        var searchPattern = liftNumber + "-AutoDeskTransfer.xml";
        var watch = Stopwatch.StartNew();
        var workspaceSearch = await SearchWorkspaceAsync(searchPattern, spezifikationTyp);
        var stopTimeMs = watch.ElapsedMilliseconds;

        switch (workspaceSearch.Length)
        {
            case 0:
                {
                    _logger.LogInformation(60139, "{SpezifikationName}-AutoDeskTransfer.xml not found in workspace", liftNumber);
                    return (stopTimeMs, await GetFileAsync(liftNumber, readOnly));
                }
            case 1:
                {
                    var autoDeskTransferpath = workspaceSearch[0];
                    FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                    if (!AutoDeskTransferInfo.IsReadOnly)
                    {
                        _logger.LogInformation(60139, "Data {searchPattern} from workspace loaded", searchPattern);
                        return (stopTimeMs, new DownloadInfo()
                        {
                            ExitCode = 0,
                            CheckOutState = "CheckedOutByCurrentUser",
                            ExitState = ExitCodeEnum.NoError,
                            FullFileName = workspaceSearch[0],
                            Success = true,
                            IsCheckOut = true
                        });
                    }
                    else
                    {
                        return (stopTimeMs, await GetFileAsync(liftNumber, readOnly));
                    }
                }
            default:
                {
                    _logger.LogError(61039, "Searchresult {searchPattern} with multimatching files", searchPattern);
                    return (stopTimeMs, new DownloadInfo()
                    {
                        ExitCode = 5,
                        FileName = searchPattern,
                        FullFileName = searchPattern,
                        ExitState = ExitCodeEnum.MultipleAutoDeskTransferXml
                    });
                }
        }
    }

    /// <inheritdoc/>
    private async Task<string[]> SearchWorkspaceAsync(string searchPattern, SpezifikationTyp spezifikationTyp)
    {
        _logger.LogInformation(60139, "Workspacesearch started");
        string? path;

        if (spezifikationTyp is not null &&
            spezifikationTyp.Equals(SpezifikationTyp.Order))
        {
            path = @"C:\Work\AUFTRÄGE NEU\Konstruktion";
            if (!Directory.Exists(path))
            {
                return [];
            }
        }
        else
        {
            path = @"C:\Work\AUFTRÄGE NEU\Angebote";
            if (!Directory.Exists(path))
            {
                return [];
            }
        }
        var searchResult = await Task.Run(() => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories));
        return searchResult;
    }
}