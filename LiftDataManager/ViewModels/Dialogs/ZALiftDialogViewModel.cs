using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class ZALiftDialogViewModel : ObservableObject
{
    private const string pathSynchronizeZAlift = @"C:\Work\Administration\PowerShellScripts\SynchronizeZAlift.ps1";
    private readonly DispatcherQueue dispatcherQueue;
    private readonly ISettingService _settingService;
    private readonly ILogger<ZALiftDialogViewModel> _logger;
    private readonly IVaultDataService _vaultDataService;
    public Process? ZaLift { get; set; }
    public FileSystemWatcher? Watcher { get; set; }

    public ZALiftDialogViewModel(ISettingService settingsSelectorService, ILogger<ZALiftDialogViewModel> logger, IVaultDataService vaultDataService)
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _settingService = settingsSelectorService;
        _vaultDataService = vaultDataService;
        _logger = logger;
    }

    [RelayCommand]
    public async Task ZALiftDialogLoadedAsync(ZALiftDialog sender)
    {
        FullPathXml = sender.FullPathXml;
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            ExWorkStatus = "ungüliger Autodesktranfer Pfad";
            return;
        }
        var pathAutodeskTransfer = Path.GetDirectoryName(FullPathXml);
        if (string.IsNullOrWhiteSpace(pathAutodeskTransfer))
        {
            return;
        }
        OrderNumber = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        Watcher = new()
        {
            Path = Path.Combine(pathAutodeskTransfer, "Berechnungen"),
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        Watcher.Changed += OnChanged;
        sender.Closed += ZALiftDialogClosed;
        await StartZetaLiftProgrammAsync();
    }

    private void ZALiftDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (Watcher is not null)
        {
            Watcher.Changed -= OnChanged;
            Watcher.Dispose();
        }
        sender.Closed -= ZALiftDialogClosed;
    }

    [RelayCommand]
    public void PrimaryButtonClicked()
    {
        ZaLift?.Kill(true);
    }

    public string? FullPathXml { get; set; }
    public string? OrderNumber { get; set; }

    [ObservableProperty]
    private string? exWorkStatus = "Ziehl Abegg Auslegung wird bearbeitet";

    [ObservableProperty]
    private bool zAliftRegEditSuccessful;

    [ObservableProperty]
    private bool zAliftDataReadyForImport;
    partial void OnZAliftDataReadyForImportChanged(bool value)
    {
        ExWorkStatus = value ? "Daten zur Übernahme bereit" : "Ziehl Abegg Auslegung wird bearbeitet";
    }

    [ObservableProperty]
    private bool zAliftHtmlUpdated;
    partial void OnZAliftHtmlUpdatedChanged(bool value)
    {
        ZAliftDataReadyForImport = value & ZAliftAusUpdated;
    }

    [ObservableProperty]
    private bool zAliftAusUpdated;
    partial void OnZAliftAusUpdatedChanged(bool value)
    {
        ZAliftDataReadyForImport = value & ZAliftHtmlUpdated;
    }

    private async Task StartZetaLiftProgrammAsync()
    {
        var startargs = "StartLAST";
        var pathZALift = _settingService.PathZALift;
        if (!File.Exists(pathZALift))
        {
            return;
        }
        if (FullPathXml is null)
        {
            return;
        }
        var pathAutodeskTransfer = Path.GetDirectoryName(FullPathXml);
        if (string.IsNullOrWhiteSpace(pathAutodeskTransfer))
        {
            return;
        }
        if (!Directory.Exists(Path.Combine(pathAutodeskTransfer, "Berechnungen")))
        {
            Directory.CreateDirectory(Path.Combine(pathAutodeskTransfer, "Berechnungen"));
        }

        ProcessHelpers.MakeBackupFile(Path.Combine(pathAutodeskTransfer, "Berechnungen", OrderNumber + ".html"), OrderNumber);
        ProcessHelpers.MakeBackupFile(Path.Combine(pathAutodeskTransfer, "Berechnungen", OrderNumber + ".aus"), OrderNumber);

        if (!File.Exists(pathSynchronizeZAlift))
        {
            var downloadResult = await _vaultDataService.GetFileAsync("SynchronizeZAlift.ps1", true, true);
            _logger.LogInformation(60191, "downloadResult SynchronizeZAlift.ps1: ErrorState {downloadResult.ErrorState}", downloadResult.ErrorState);
        }

        if (File.Exists(pathSynchronizeZAlift))
        {
            var args = $"{pathSynchronizeZAlift} set '{FullPathXml}'";
            var exitCode = await ProcessHelpers.StartProgramWithExitCodeAsync("PowerShell.exe", args);
            _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);

            if (exitCode == 0)
            {
                ZAliftRegEditSuccessful = true;
            }
        }

        if (!File.Exists(pathZALift))
        {
            if (Watcher is not null)
            {
                Watcher.Changed -= OnChanged;
                Watcher.Dispose();
            }
            return;
        }

        ZaLift = new();
        ZaLift.StartInfo.UseShellExecute = true;
        ZaLift.StartInfo.FileName = pathZALift;
        ZaLift.StartInfo.Arguments = startargs;
        ZaLift.Start();
    }
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        if (!string.IsNullOrWhiteSpace(OrderNumber))
        {
            if (e.Name == OrderNumber + ".aus")
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    ZAliftAusUpdated = true;
                });
            }
            else if (e.Name == OrderNumber + ".html")
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    ZAliftHtmlUpdated = true;
                });
            }
        }
        Debug.WriteLine($"Changed: {e.FullPath}");
    }
}