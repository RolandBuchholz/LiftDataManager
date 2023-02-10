using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace LiftDataManager.ViewModels;

public partial class QuickLinksViewModel : DataViewModelBase, INavigationAware
{
    private const string pathSynchronizeZAlift = @"C:\Work\Administration\PowerShellScripts\SynchronizeZAlift.ps1";
    private const string pathVaultPro = @"C:\Programme\Autodesk\Vault Client 2023\Explorer\Connectivity.VaultPro.exe";
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
    private const string pathSpezifikation = @"C:\Work\Administration\Spezifikation\Spezifikation.xlsm";

    private readonly ISettingService _settingService;
    private readonly ParameterContext _parametercontext;
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<QuickLinksViewModel> _logger;

    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
        ISettingService settingsSelectorService, ParameterContext parametercontext, IVaultDataService vaultDataService, ILogger<QuickLinksViewModel> logger) :
         base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        _parametercontext = parametercontext;
        _vaultDataService = vaultDataService;
        _logger = logger;
        CheckCanOpenFiles();
    }

    [ObservableProperty]
    private bool zAliftHtmlUpdated;
    partial void OnZAliftHtmlUpdatedChanged(bool value)
    {
        CanImportZAliftData = value & ZAliftAusUpdated;
    }

    [ObservableProperty]
    private bool zAliftAusUpdated;
    partial void OnZAliftAusUpdatedChanged(bool value)
    {
        CanImportZAliftData = value & ZAliftHtmlUpdated;
    }

    [ObservableProperty]
    private bool zAliftRegEditSuccessful;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSpeziPdfCommand))]
    private bool canOpenSpeziPdf;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCalculationsCommand))]
    private bool canOpenCalculations;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenVaultCommand))]
    private bool canOpenVault;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenZiehlAbeggCommand))]
    private bool canOpenZALift;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportZAliftDataCommand))]
    private bool canImportZAliftData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenZiehlAbeggHtmlCommand))]
    private bool canOpenZALiftHtml;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCFPCommand))]
    private bool canOpenCFP;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenLiloCommand))]
    private bool canOpenLilo;

    [ObservableProperty]
    private string? exWorkStatus;

    public void CheckCanOpenFiles()
    {
        SynchronizeViewModelParameter();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
        {
            CanOpenSpeziPdf = File.Exists(FullPathXml.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf"));
        }
        CanOpenVault = !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer);
        CanOpenCalculations = CanOpenVault;
        CanOpenCFP = File.Exists(_settingService.PathCFP);
        CanOpenLilo = File.Exists(_settingService.PathLilo);
        CanOpenZALift = File.Exists(_settingService.PathZALift);
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
        {
            var auftragsnummer = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
            CanOpenZALiftHtml = File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".html"));
        }
    }

    [RelayCommand]
    private async Task OpenSpeziAsync()
    {
        SynchronizeViewModelParameter();
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var filename = _settingService.PathExcel;
        var startargs = string.Empty;
        var closeLiftDataManager = false;

        if (!string.IsNullOrWhiteSpace(auftragsnummer))
        {
            if (CheckOut)
            {
                _ = _dialogService!.MessageDialogAsync("Spezifikation öffnen", "Öffnen der Spezifikation\nnur möglich wenn AutoDeskTransfer.xml eingechecked ist.");
                return;
            }

            var result = await _dialogService!.ConfirmationDialogAsync("Öffnungsoptionen Spezifikation", "Zum Bearbeiten öffnen", "Schreibgeschützt öffnen");
            if (result is not null)
            {
                if (result.Value)
                {
                    startargs = @"/e/" + auftragsnummer + "/false/ " + pathSpezifikation;
                    closeLiftDataManager = true;
                }
                else
                {
                    startargs = @"/e/" + auftragsnummer + "/true/ " + pathSpezifikation;
                }
            }
        }
        else
        {
            startargs = pathSpezifikation;
        }

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
            if (closeLiftDataManager)
            {
                Application.Current.Exit();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenSpeziPdf))]
    private void OpenSpeziPdf()
    {
        SynchronizeViewModelParameter();
        var filename = FullPathXml?.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf");
        var startargs = string.Empty;

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenCalculations))]
    private void OpenCalculations()
    {
        SynchronizeViewModelParameter();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
        {
            var path = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", "PDF");
            MakeVaultLink(path, "Folder");
            var filename = pathVaultPro;
            var startargs = @"C:\Temp\VaultLink.acr";
            if (File.Exists(FullPathXml))
            {
                StartProgram(filename, startargs);
            }
        }
    }

    [RelayCommand]
    private void OpenBauer()
    {
        SynchronizeViewModelParameter();
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var filename = @"C:\Work\Administration\Tools\Explorer Start.exe";

        if (File.Exists(FullPathXml))
        {
            if (auftragsnummer != null)
            {
                StartProgram(filename, auftragsnummer);
            }
        }
    }

    [RelayCommand]
    private void OpenWorkspace()
    {
        SynchronizeViewModelParameter();
        if (!string.IsNullOrWhiteSpace(FullPathXml))
        {
            var pathXml = Path.GetDirectoryName(FullPathXml);
            var filename = "explorer.exe";

            if (Directory.Exists(pathXml))
            {
                StartProgram(filename, pathXml);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenVault))]
    private void OpenVault()
    {
        SynchronizeViewModelParameter();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
        {
            MakeVaultLink(FullPathXml, "File");
            var filename = pathVaultPro;
            var startargs = @"C:\Temp\VaultLink.acr";
            if (File.Exists(FullPathXml))
            {
                StartProgram(filename, startargs);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenCFP))]
    private void OpenCFP()
    {
        SynchronizeViewModelParameter();
        var startargs = string.Empty;
        var pathCFP = _settingService.PathCFP;
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var bausatztyp = ParamterDictionary?["var_Bausatz"].Value;
        var shortSymbolDirveSystem = string.Empty;

        var driveSystem = _parametercontext.Set<CarFrameType>().Include(i => i.DriveType)
                                           .ToList()
                                           .FirstOrDefault(x => x.Name == bausatztyp);
        if (driveSystem is not null)
        {
            shortSymbolDirveSystem = driveSystem.DriveType!.Name == "Seil" ? "S" : "H";
            startargs = driveSystem.IsCFPControlled ? auftragsnummer + " " + bausatztyp + " " + shortSymbolDirveSystem : string.Empty;
        }

        if (File.Exists(pathCFP))
        {
            StartProgram(pathCFP, startargs);
        }
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanOpenZALift))]
    private async Task OpenZiehlAbeggAsync(ContentDialog extEditDialog, CancellationToken token)
    {
        SynchronizeViewModelParameter();
        _ = SetModelStateAsync();
        var startargs = "StartLAST";
        var pathZALift = _settingService.PathZALift;
        bool noEditMode = false;
        ZAliftAusUpdated = false;
        _zAliftAusUpdated = false;
        ZAliftHtmlUpdated = false;
        _zAliftHtmlUpdated = false;
        zAliftRegEditSuccessful = false;
        if (!File.Exists(pathZALift)) return;

        if (!CheckOut)
        {
            var message = """
                Die Auslegung ist aktuell nicht ausgecheckt!

                Auschecken:            Bearbeitung und Datenübernahme möglich
                Nicht Auschecken:  Keine Datenübernahme möglich
                Abbrechen:             Zurück zu LiftDataManager
                """;
            var checkOutResult = await _dialogService!.ConfirmationDialogAsync("Ziehl Abegg Auslegungsbearbeitung", message, "Auschecken", "Nicht Auschecken", "Abbrechen");
            if (checkOutResult != null)
            {
                if ((bool)checkOutResult)
                {
                    //TODO CheckOUT
                }
                else
                {
                    noEditMode = true;
                }
            }
            else
            {
                return;
            }
        }

        if (CanSaveAllSpeziParameters)
        {
            _ = _parameterDataService!.SaveAllParameterAsync(ParamterDictionary!, FullPathXml!, Adminmode);
        }

        CanImportZAliftData = false;
        ExWorkStatus = "Ziehl Abegg Auslegung wird bearbeitet";
        ZAliftHtmlUpdated = false;
        ZAliftAusUpdated = false;

        var dialog = extEditDialog.ShowAsync();

        if (!File.Exists(pathSynchronizeZAlift))
        {
            var downloadResult = await _vaultDataService.GetFileAsync("SynchronizeZAlift.ps1", true, true);
            _logger.LogInformation(60191, "downloadResult SynchronizeZAlift.ps1: ErrorState {downloadResult.ErrorState}", downloadResult.ErrorState);
        }

        if (File.Exists(pathSynchronizeZAlift))
        {
            var args = $"{pathSynchronizeZAlift} set '{FullPathXml}'";
            var exitCode = await StartProgramWithExitCodeAsync("PowerShell.exe", args);
            _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);

            if (exitCode == 0)
            {
                ZAliftRegEditSuccessful = true;
            }
        }

        if (noEditMode)
        {
            extEditDialog.Hide();
            StartProgram(pathZALift, startargs);
            return;
        }

        using FileSystemWatcher watcher = new(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen"));
        watcher.IncludeSubdirectories = false;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += OnChanged;

        using Process zaLift = new();
        zaLift.StartInfo.UseShellExecute = true;
        zaLift.StartInfo.FileName = pathZALift;
        zaLift.StartInfo.Arguments = startargs;
        zaLift.Start();

        bool exit = false;

        while (!exit)
        {
            exit = _zAliftAusUpdated && _zAliftHtmlUpdated;
            if (token.IsCancellationRequested)
            {
                exit = true;
                zaLift.Kill(true);
            }
            await Task.Delay(50);
        }
        
        ZAliftAusUpdated = _zAliftAusUpdated;
        ZAliftHtmlUpdated = _zAliftHtmlUpdated;
        if(ZAliftAusUpdated && ZAliftAusUpdated)
            ExWorkStatus = "Daten zur Übernahme bereit";

        watcher.Changed -= OnChanged;

        var result = await dialog;

        if (result == ContentDialogResult.Primary)
        {
            zaLift.Kill(true);
        }
    }

    private bool _zAliftAusUpdated;
    private bool _zAliftHtmlUpdated;

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        var auftragsnummer = Path.GetFileNameWithoutExtension(FullPathXml!).Replace("-AutoDeskTransfer", "");

        if (!string.IsNullOrWhiteSpace(auftragsnummer))
        {
            if (e.Name == auftragsnummer + ".aus")
            {
                _zAliftAusUpdated = true;
            }
            else if (e.Name == auftragsnummer + ".html")
            {
                _zAliftHtmlUpdated = true;
            }
        }
        Debug.WriteLine($"Changed: {e.FullPath}");
    }

    [RelayCommand(CanExecute = nameof(CanImportZAliftData))]
    private async Task ImportZAliftDataAsync()
    {
        await Task.Delay(500);
        await _dialogService!.MessageDialogAsync("Daten werden geschrieben.........", "");
    }

    [RelayCommand(CanExecute = nameof(CanOpenZALiftHtml))]
    private void OpenZiehlAbeggHtml()
    {
        SynchronizeViewModelParameter();
        var auftragsnummer = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_AuftragsNummer");
        var filename = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".html");
        var startargs = string.Empty;

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenLilo))]
    private void OpenLilo()
    {
        SynchronizeViewModelParameter();
        var startargs = "";
        var pathLilo = _settingService.PathLilo;

        if (File.Exists(pathLilo))
        {
            StartProgram(pathLilo, startargs);
        }
    }

    [RelayCommand]
    private void RefreshQuickLinks()
    {
        CheckCanOpenFiles();
    }

    private static void StartProgram(string filename, string startargs)
    {
        using Process p = new();
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.FileName = filename;
        p.StartInfo.Arguments = startargs;
        p.Start();
    }

    private async Task<int> StartProgramWithExitCodeAsync(string filename, string startargs)
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

    private static void MakeVaultLink(string path, string objectType)
    {
        var vaultPath = path.Replace(@"C:\Work", "$").Replace(@"\", "/");

        XmlWriter? oXmlWriter = null;
        var oXmlWriterSettings = new XmlWriterSettings();

        try
        {
            // Eigenschaften / Einstellungen festlegen
            oXmlWriterSettings.Indent = true;
            oXmlWriterSettings.IndentChars = "  ";
            oXmlWriterSettings.NewLineChars = "\r\n";

            oXmlWriter = XmlWriter.Create(@"C:\Temp\VaultLink.acr", oXmlWriterSettings);

            oXmlWriter.WriteStartElement("ADM", "http://schemas.autodesk.com/msd/plm/ExplorerAutomation/2004-11-01");

            oXmlWriter.WriteStartElement("Server");
            oXmlWriter.WriteString("192.168.0.1");
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteStartElement("Vault");
            oXmlWriter.WriteString("Vault");
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteStartElement("Operations");
            oXmlWriter.WriteStartElement("Operation");
            oXmlWriter.WriteAttributeString("ObjectType", objectType);
            oXmlWriter.WriteStartElement("ObjectID");
            oXmlWriter.WriteString(vaultPath);
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteStartElement("Command");
            oXmlWriter.WriteString("Select");
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteEndElement();
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
        finally
        {
            oXmlWriter?.Close();
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        CheckCanOpenFiles();
    }

    public void OnNavigatedFrom()
    {
    }
}