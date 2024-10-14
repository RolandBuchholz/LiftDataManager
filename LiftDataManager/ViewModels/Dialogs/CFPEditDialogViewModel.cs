using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class CFPEditDialogViewModel : ObservableObject
{
    private readonly DispatcherQueue dispatcherQueue;
    private readonly ISettingService _settingService;
    private readonly ILogger<CFPEditDialogViewModel> _logger;
    private readonly ParameterContext _parametercontext;

    public FileSystemWatcher? CFPWatcher { get; set; }
    public CFPEditDialogViewModel(ISettingService settingsSelectorService, ILogger<CFPEditDialogViewModel> logger, ParameterContext parametercontext)
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _settingService = settingsSelectorService;
        _logger = logger;
        _parametercontext = parametercontext;
    }

    public string? FullPathXml { get; set; }
    public string? PathAutodeskTransfer { get; set; }
    public string? OrderNumber { get; set; }

    [RelayCommand]
    public async Task CFPEditDialogLoadedAsync(CFPEditDialog sender)
    {
        FullPathXml = sender.FullPathXml;
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            ExWorkStatus = "ungüliger Autodesktranfer Pfad";
            return;
        }
        PathAutodeskTransfer = Path.GetDirectoryName(FullPathXml);
        if (string.IsNullOrWhiteSpace(PathAutodeskTransfer))
        {
            return;
        }
        OrderNumber = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        CFPWatcher = new()
        {
            Path = PathAutodeskTransfer,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        CFPWatcher.Changed += OnChangedCFP;
        sender.Closed += CFPEditDialogClosed;
        await StartCarFrameProgramAsync(sender.CarFrameTyp);
    }

    private void CFPEditDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (CFPWatcher is not null)
        {
            CFPWatcher.Changed -= OnChangedCFP;
            CFPWatcher.Dispose();
        }
        sender.Closed -= CFPEditDialogClosed;
        if (string.IsNullOrWhiteSpace(PathAutodeskTransfer))
        {
            return;
        }
        var fromCFP = Path.Combine(PathAutodeskTransfer, "fromCFP.txt");
        if (File.Exists(fromCFP))
        {
            File.Delete(fromCFP);
        }
        if (args.Result == ContentDialogResult.None)
        {
            if (string.IsNullOrWhiteSpace(PathAutodeskTransfer))
            {
                return;
            }
            try
            {
                var restoreFileXml = Path.Combine(PathAutodeskTransfer, OrderNumber + "-AutoDeskTransfer.xml");
                FileInfo restoreFileXmlInfo = new(restoreFileXml);
                if (restoreFileXmlInfo.IsReadOnly)
                    return;
                var backupFileXml = Path.Combine(PathAutodeskTransfer, OrderNumber + "-LDM_Backup.xml");

                if (File.Exists(backupFileXml))
                {
                    File.Move(backupFileXml, restoreFileXml, true);
                    _logger.LogInformation(60192, "{restoreFileXml} restored from backupfile", restoreFileXml);
                }
                else
                {
                    _logger.LogError(61092, "no backupfile found, restoring Autodesktransfer.xml failed");
                }
            }
            catch (Exception)
            {

                _logger.LogError(61092, "restoring Autodesktransfer.xml failed");
            }
        }
    }

    [RelayCommand]
    public void PrimaryButtonClicked()
    {
        if (string.IsNullOrWhiteSpace(PathAutodeskTransfer))
        {
            return;
        }
        var backupFileXml = Path.Combine(PathAutodeskTransfer, OrderNumber + "-LDM_Backup.xml");
        if (File.Exists(backupFileXml))
        {
            File.Delete(backupFileXml);
        }
    }

    [ObservableProperty]
    private string? exWorkStatus = "CFP Auslegung wird bearbeitet";

    [ObservableProperty]
    private bool cFPDataReadyForImport;
    partial void OnCFPDataReadyForImportChanged(bool value)
    {
        ExWorkStatus = value ? "Daten zur Übernahme bereit" : "CFP Auslegung wird bearbeitet";
    }
    private async Task StartCarFrameProgramAsync(string? carFrameTyp)
    {
        CFPDataReadyForImport = false;
        var startargs = $"{OrderNumber}";
        var pathCFP = _settingService.PathCFP;
        if (!File.Exists(pathCFP))
        {
            return;
        }
        if (FullPathXml is null)
        {
            return;
        }
        var shortSymbolDirveSystem = string.Empty;

        if (!string.IsNullOrWhiteSpace(carFrameTyp))
        {
            var driveSystem = _parametercontext.Set<CarFrameType>().Include(i => i.DriveType)
                                                                   .ToList()
                                                                   .FirstOrDefault(x => x.Name == carFrameTyp);
            if (driveSystem is not null)
            {
                shortSymbolDirveSystem = driveSystem.DriveType!.Name == "Seil" ? "S" : "H";
                startargs = $"{OrderNumber} {driveSystem.CFPStartIndex} {shortSymbolDirveSystem}";
            }
        }
        PathAutodeskTransfer = Path.GetDirectoryName(FullPathXml);
        if (string.IsNullOrWhiteSpace(PathAutodeskTransfer))
        {
            return;
        }
        if (!Directory.Exists(Path.Combine(PathAutodeskTransfer, "Berechnungen")))
        {
            Directory.CreateDirectory(Path.Combine(PathAutodeskTransfer, "Berechnungen"));
        }
        ProcessHelpers.MakeBackupFile(Path.Combine(PathAutodeskTransfer, OrderNumber + "-AutoDeskTransfer.xml"), OrderNumber);

        if (!File.Exists(pathCFP))
        {
            if (CFPWatcher is not null)
            {
                CFPWatcher.Changed -= OnChangedCFP;
                CFPWatcher.Dispose();
            }
            return;
        }

        using Process carFrameProgram = new();
        carFrameProgram.StartInfo.UseShellExecute = true;
        carFrameProgram.StartInfo.FileName = pathCFP;
        carFrameProgram.StartInfo.Arguments = startargs;
        carFrameProgram.Start();
        await Task.CompletedTask;
    }
    private void OnChangedCFP(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        if (e.Name == "fromCFP.txt")
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                CFPDataReadyForImport = true;
            });
        }
        Debug.WriteLine($"Changed: {e.FullPath}");
    }
}