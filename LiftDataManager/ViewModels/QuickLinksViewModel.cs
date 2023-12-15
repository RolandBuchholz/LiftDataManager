﻿using HtmlAgilityPack;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace LiftDataManager.ViewModels;

public partial class QuickLinksViewModel : DataViewModelBase, INavigationAware, IRecipient<QuicklinkControlMessage>
{
    private const string pathSynchronizeZAlift = @"C:\Work\Administration\PowerShellScripts\SynchronizeZAlift.ps1";
    private const string pathVaultPro = @"C:\Programme\Autodesk\Vault Client 2023\Explorer\Connectivity.VaultPro.exe";
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";

    private readonly ISettingService _settingService;
    private readonly ParameterContext _parametercontext;
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<QuickLinksViewModel> _logger;
    private readonly IValidationParameterDataService _validationParameterDataService;

    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IValidationParameterDataService validationParameterDataService,
        ISettingService settingsSelectorService, ParameterContext parametercontext, IVaultDataService vaultDataService, ILogger<QuickLinksViewModel> logger) :
         base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        _parametercontext = parametercontext;
        _vaultDataService = vaultDataService;
        _validationParameterDataService = validationParameterDataService;
        _logger = logger;
        IsActive = true;
        CheckCanOpenFiles();
    }

    public void Receive(QuicklinkControlMessage message)
    {
        if (message is null)
            return;
        if (message.Value.UpdateQuicklinks)
        {
            CheckCanOpenFiles();
        }
        if (message.Value.SetDriveData)
        {
            if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
                CanOpenZALiftHtml = File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html"));
            if (CanOpenZALiftHtml)
            {
                _ = ImportZAliftDataAsync(true);
            }
        }
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

    [ObservableProperty]
    private bool zAliftRegEditSuccessful;

    [ObservableProperty]
    private bool zAliftDataReadyForImport;

    [ObservableProperty]
    private bool cFPDataReadyForImport;

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
    private bool canImportCFPData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenLiloCommand))]
    private bool canOpenLilo;

    [ObservableProperty]
    private string? exWorkStatus;

    [ObservableProperty]
    private string updatedParameter = string.Empty;

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

        if (ParameterDictionary is null || string.IsNullOrWhiteSpace(ParameterDictionary["var_Aufzugstyp"].Value))
        {
            CanOpenLilo = File.Exists(_settingService.PathLilo);
            CanOpenZALift = File.Exists(_settingService.PathZALift);
        }
        else
        {
            CanOpenZALift = ParameterDictionary["var_Aufzugstyp"].Value!.Contains("Seil") && File.Exists(_settingService.PathZALift);
            CanOpenLilo = ParameterDictionary["var_Aufzugstyp"].Value!.Contains("Hydraulik") && File.Exists(_settingService.PathLilo);
        }
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
            CanOpenZALiftHtml = File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html"));
        CanImportZAliftData = CanOpenZALiftHtml && CheckOut;
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
        var auftragsnummer = ParameterDictionary?["var_AuftragsNummer"].Value;
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

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanOpenCFP))]
    private async Task OpenCFP(ContentDialog cFPEditDialog, CancellationToken token)
    {
        SynchronizeViewModelParameter();
        _ = SetModelStateAsync();
        _fromCFPwritten = false;
        CFPDataReadyForImport = false;
        var startargs = string.Empty;
        var pathCFP = _settingService.PathCFP;
        if (!File.Exists(pathCFP))
            return;
        var auftragsnummer = ParameterDictionary?["var_AuftragsNummer"].Value;
        var bausatztyp = ParameterDictionary?["var_Bausatz"].Value;
        var shortSymbolDirveSystem = string.Empty;

        var driveSystem = _parametercontext.Set<CarFrameType>().Include(i => i.DriveType)
                                                               .ToList()
                                                               .FirstOrDefault(x => x.Name == bausatztyp);
        if (driveSystem is not null)
        {
            shortSymbolDirveSystem = driveSystem.DriveType!.Name == "Seil" ? "S" : "H";
            var identifierbausatztyp = !string.IsNullOrWhiteSpace(bausatztyp) ? bausatztyp.Replace(" ", "") : string.Empty;
            startargs = driveSystem.IsCFPControlled ? auftragsnummer + " " + identifierbausatztyp + " " + shortSymbolDirveSystem : string.Empty;
        }

        if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen")))
        {
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen"));
        }

        if (!CheckOut)
        {
            var checkOutResult = await CheckOutDialogAsync();
            if (checkOutResult is null)
                return;
            if ((bool)checkOutResult)
                return;
            if (File.Exists(pathCFP))
            {
                StartProgram(pathCFP, startargs);
            }
            return;
        }

        if (CanSaveAllSpeziParameters)
        {
            _ = _parameterDataService!.SaveAllParameterAsync(ParameterDictionary!, FullPathXml!, Adminmode);
        }

        MakeBackupFile(Path.Combine(Path.GetDirectoryName(FullPathXml)!, auftragsnummer + "-AutoDeskTransfer.xml"));
        ExWorkStatus = "CFP Auslegung wird bearbeitet";
        CanImportCFPData = false;

        var dialog = cFPEditDialog.ShowAsync();

        using FileSystemWatcher watcher = new(Path.GetDirectoryName(FullPathXml)!);
        watcher.IncludeSubdirectories = false;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += OnChangedCFP;

        if (!File.Exists(pathCFP))
        {
            watcher.Changed -= OnChangedCFP;
            return;
        }

        using Process carFrameProgram = new();
        carFrameProgram.StartInfo.UseShellExecute = true;
        carFrameProgram.StartInfo.FileName = pathCFP;
        carFrameProgram.StartInfo.Arguments = startargs;
        carFrameProgram.Start();

        bool exit = false;

        while (!exit)
        {
            exit = _fromCFPwritten;
            if (token.IsCancellationRequested)
            {
                exit = true;
                watcher.Changed -= OnChangedCFP;
                carFrameProgram.Kill(true);
            }
            await Task.Delay(50);
        }

        watcher.Changed -= OnChangedCFP;

        var fromCFP = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "fromCFP.txt");
        if (File.Exists(fromCFP))
            File.Delete(fromCFP);

        CFPDataReadyForImport = _fromCFPwritten;
        if (CFPDataReadyForImport)
            ExWorkStatus = "Daten zur Übernahme bereit";

        var result = await dialog;
        if (result == ContentDialogResult.Primary)
        {
            var backupFileXml = Path.Combine(Path.GetDirectoryName(FullPathXml)!, auftragsnummer + "-LDM_Backup.xml");
            if (File.Exists(backupFileXml))
                File.Delete(backupFileXml);

            var updatedResult = await _parameterDataService!.SyncFromAutodeskTransferAsync(FullPathXml!, ParameterDictionary!);
            if (updatedResult is not null)
            {
                if (updatedResult.Any())
                {
                    await _dialogService!.MessageDialogAsync("Aktualisierte Parameter", string.Join("\n", updatedResult.Select(x => x.ToString())));
                }
                ParameterDictionary!["var_CFPdefiniert"].Value = "True";
            }
        }
        else
        {
            await _dialogService!.MessageDialogAsync("CarFrameProgram abgebrochen",
                "Achtung:\n" +
                "Daten aus dem CarFrameProgram werden verworfen!\n" +
                "Backup wird der Autodesktransfer.xml wird wiederhergestellt!");
            try
            {
                var restoreFileXml = Path.Combine(Path.GetDirectoryName(FullPathXml)!, auftragsnummer + "-AutoDeskTransfer.xml");
                FileInfo restoreFileXmlInfo = new(restoreFileXml);
                if (restoreFileXmlInfo.IsReadOnly)
                    return;
                var backupFileXml = Path.Combine(Path.GetDirectoryName(FullPathXml)!, auftragsnummer + "-LDM_Backup.xml");

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

    private bool _fromCFPwritten;

    private void OnChangedCFP(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;
        if (e.Name == "fromCFP.txt")
            _fromCFPwritten = true;

        Debug.WriteLine($"Changed: {e.FullPath}");
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanOpenZALift))]
    private async Task OpenZiehlAbeggAsync(ContentDialog zaliftEditDialog, CancellationToken token)
    {
        SynchronizeViewModelParameter();
        _ = SetModelStateAsync();
        ZAliftRegEditSuccessful = false;
        var startargs = "StartLAST";
        var pathZALift = _settingService.PathZALift;
        if (!File.Exists(pathZALift))
            return;
        bool noEditMode = false;
        ZAliftAusUpdated = false;
        _zAliftAusUpdated = false;
        ZAliftHtmlUpdated = false;
        _zAliftHtmlUpdated = false;
        zAliftRegEditSuccessful = false;
        var auftragsnummer = ParameterDictionary?["var_AuftragsNummer"].Value;

        if (!CheckOut)
        {
            var checkOutResult = await CheckOutDialogAsync();
            if (checkOutResult is null)
                return;
            if ((bool)checkOutResult)
                return;

            noEditMode = true;
        }

        if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen")))
        {
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen"));
        }

        if (CanSaveAllSpeziParameters)
        {
            _ = _parameterDataService!.SaveAllParameterAsync(ParameterDictionary!, FullPathXml!, Adminmode);
        }

        MakeBackupFile(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".html"));
        MakeBackupFile(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".aus"));

        CanImportZAliftData = false;
        ExWorkStatus = "Ziehl Abegg Auslegung wird bearbeitet";
        ZAliftHtmlUpdated = false;
        ZAliftAusUpdated = false;

        var dialog = zaliftEditDialog.ShowAsync();

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
            zaliftEditDialog.Hide();
            StartProgram(pathZALift, startargs);
            await Task.Delay(1000);
            if (File.Exists(pathSynchronizeZAlift))
            {
                var args = $"{pathSynchronizeZAlift} reset '{FullPathXml}'";
                var exitCode = await StartProgramWithExitCodeAsync("PowerShell.exe", args);
                _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);

            }
            return;
        }

        using FileSystemWatcher watcher = new(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen"));
        watcher.IncludeSubdirectories = false;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += OnChanged;

        if (!File.Exists(pathZALift))
        {
            watcher.Changed -= OnChanged;
            return;
        }

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
        if (ZAliftAusUpdated && ZAliftAusUpdated)
            ExWorkStatus = "Daten zur Übernahme bereit";

        watcher.Changed -= OnChanged;

        var result = await dialog;

        if (result == ContentDialogResult.Primary)
        {
            zaLift.Kill(true);
        }
        else
        {
            try
            {
                var restoreFileHtml = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".html");
                FileInfo restoreFileHtmlInfo = new(restoreFileHtml);
                if (restoreFileHtmlInfo.IsReadOnly)
                {
                    restoreFileHtmlInfo.IsReadOnly = false;
                }
                File.Delete(restoreFileHtml);
                var restoreFileAus = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", auftragsnummer + ".aus");
                FileInfo restoreFileAusInfo = new(restoreFileAus);
                if (restoreFileAusInfo.IsReadOnly)
                {
                    restoreFileAusInfo.IsReadOnly = false;
                }
                File.Delete(restoreFileAus);
                File.Move(Path.Combine(Path.GetDirectoryName(restoreFileHtml)!, SpezifikationsNumber + "-LDM_Backup" + Path.GetExtension(restoreFileHtml)), restoreFileHtml);
                File.Move(Path.Combine(Path.GetDirectoryName(restoreFileAus)!, SpezifikationsNumber + "-LDM_Backup" + Path.GetExtension(restoreFileAus)), restoreFileAus);
                _logger.LogInformation(60192, "{restoreFileHtml} restored from backupfile", restoreFileHtml);
                _logger.LogInformation(60192, "{restoreFileAus} restored from backupfile", restoreFileAus);
            }
            catch (Exception)
            {
                _logger.LogError(61092, "restoring zaliftfiles failed");
            }
        }

        if (File.Exists(pathSynchronizeZAlift))
        {
            var args = $"{pathSynchronizeZAlift} reset '{FullPathXml}'";
            var exitCode = await StartProgramWithExitCodeAsync("PowerShell.exe", args);
            _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);
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
    private async Task ImportZAliftDataAsync(bool onlyDiveData)
    {
        var zaliftHtml = GetZaliftHtml();
        var zliDataDictionary = GetZliDataDictionary(zaliftHtml);

        if (ParameterDictionary is not null && zliDataDictionary.Any())
        {
            var htmlNodes = zaliftHtml.DocumentNode.SelectNodes("//tr");

            if (!onlyDiveData)
            {
                ParameterDictionary["var_Q"].Value = zliDataDictionary["Nennlast_Q"];
                ParameterDictionary["var_Gegengewichtsmasse"].Value = zliDataDictionary["Gegengewicht_G"];
                try
                {
                    double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
                    double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_F");
                    double counterWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Gegengewichtsmasse");
                    ParameterDictionary["var_GGWNutzlastausgleich"].Value = Convert.ToString(Math.Round((counterWeight - carWeight) / load, 2));
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "balance not found");
                }

                var detectionDistance = "0";
                try
                {
                    var detectionDistanceMeter = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Erkennungsweg"))?.ChildNodes[1].InnerText;

                    if (!string.IsNullOrWhiteSpace(detectionDistanceMeter))
                    {
                        detectionDistance = (Convert.ToDouble(detectionDistanceMeter.Replace("m", "").Trim(), CultureInfo.CurrentCulture) * 1000).ToString();
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "detectionDistance not found");
                }
                ParameterDictionary["var_Erkennungsweg"].Value = detectionDistance;
                var deadTime = "0";
                try
                {
                    deadTime = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Totzeit"))?.ChildNodes[1].InnerText.Replace("ms", "").Trim();
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "deadTime not found");
                }
                ParameterDictionary["var_Totzeit"].Value = deadTime;
                var vDetector = "0";
                try
                {
                    vDetector = Convert.ToDouble(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("V Detektor"))?.ChildNodes[1].InnerText.Replace("m/s", "").Trim(), CultureInfo.CurrentCulture).ToString();
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "vDetector not found");
                }
                ParameterDictionary["var_Vdetektor"].Value = vDetector;
                var brakerelease = string.Empty;
                if (zliDataDictionary["Bremse-Handlueftung"] == "ohne Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                    brakerelease = "207 V Bremse. ohne Handl. Mikrosch.";
                if (zliDataDictionary["Bremse-Handlueftung"] == "ohne Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                    brakerelease = "207 V Bremse. ohne Hand. Indukt. NS";
                if (zliDataDictionary["Bremse-Handlueftung"] == "mit Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                    brakerelease = "207 V Bremse. mit Handl. Mikrosch.";
                if (zliDataDictionary["Bremse-Handlueftung"] == "mit Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                    brakerelease = "207 V Bremse. mit Handl. induktiver NS";
                if (zliDataDictionary["Bremse-Handlueftung"] == "fuer Bowdenzug" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                    brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Mikrosch.";
                if (zliDataDictionary["Bremse-Handlueftung"] == "fuer Bowdenzug" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                    brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Indukt. NS";

                ParameterDictionary["var_Handlueftung"].Value = brakerelease;
                ParameterDictionary["var_Handlueftung"].DropDownListValue = brakerelease;

                var ventilation = zliDataDictionary["Motor-Fan"] != "ohne Belüftung" ? "True" : "False";
                ParameterDictionary["var_Fremdbelueftung"].Value = ventilation;

                try
                {
                    var brakeControl = htmlNodes.Any(x => x.InnerText.StartsWith("Bremsansteuermodul")).ToString();
                    ParameterDictionary["var_ElektrBremsenansteuerung"].Value = LiftParameterHelper.FirstCharToUpperAsSpan(brakeControl);
                }
                catch (Exception)
                {

                    _logger.LogWarning(61094, "ElektrBremsenansteuerung not found");
                }

                var hardened = zliDataDictionary["Treibscheibe-RF"].Contains("gehaertet") ? "True" : "False";
                ParameterDictionary["var_Treibscheibegehaertet"].Value = hardened;
            }

            if (zliDataDictionary.TryGetValue("Getriebebezeichnung", out string? drive))
            {
                ParameterDictionary["var_Antrieb"].Value = string.IsNullOrWhiteSpace(drive) ? string.Empty : drive.Replace(',', '.');
            }
            ParameterDictionary["var_Treibscheibendurchmesser"].Value = zliDataDictionary["Treibscheibe-D"];
            ParameterDictionary["var_ZA_IMP_Treibscheibe_RIA"].Value = zliDataDictionary["Treibscheibe-RIA"];
            ParameterDictionary["var_ZA_IMP_Regler_Typ"].Value = !string.IsNullOrWhiteSpace(zliDataDictionary["Regler-Typ"]) ? zliDataDictionary["Regler-Typ"].Replace(" ", "") : string.Empty;

            if (zliDataDictionary.TryGetValue("Treibscheibe-SD", out string? ropeDiameter))
            {
                ParameterDictionary["var_Tragseiltyp"].Value = "D " + ropeDiameter + "mm " + zliDataDictionary["Treibscheibe-Seiltyp"];
            }
            else
            {
                ParameterDictionary["var_Tragseiltyp"].Value = zliDataDictionary["Treibscheibe-Seiltyp"];
            }

            var numberOfRopes = string.Empty;
            try
            {
                numberOfRopes = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Anzahl der Seile"))?.ChildNodes[2].InnerText;
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "numberOfRopes not found");
            }
            ParameterDictionary["var_NumberOfRopes"].Value = numberOfRopes;

            var breakingload = string.Empty;
            try
            {
                breakingload = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Mindestbruchkraft"))?.ChildNodes[3].InnerText;
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "breakingload not found");
            }
            ParameterDictionary["var_Mindestbruchlast"].Value = breakingload;

            var ropeSafety = string.Empty;
            try
            {
                ropeSafety = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Seilsicherheit"))?.InnerText.Split('=', '&')[1].Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "ropeSafety not found");
            }
            ParameterDictionary["var_ZA_IMP_RopeSafety"].Value = ropeSafety;

            var ratedCurrent = string.Empty;
            var maxCurrent = string.Empty;
            var ratedCapacity = string.Empty;
            var nominalVoltage = string.Empty;
            try
            {
                var exactCurrentString = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Netzstromaufnahme"))?.InnerText;

                if (!string.IsNullOrWhiteSpace(exactCurrentString))
                {
                    var exactRatedCurrent = exactCurrentString[..exactCurrentString.IndexOf('A')].Replace("Netzstromaufnahme", "").Trim();
                    ratedCurrent = Math.Ceiling(Convert.ToDouble(exactRatedCurrent, CultureInfo.CurrentCulture)).ToString() + ",0";
                    maxCurrent = Math.Round(Convert.ToDouble(exactRatedCurrent, CultureInfo.CurrentCulture) * 1.8 + 10, 2).ToString();

                    var exactCapacityCurrent = exactCurrentString[(exactCurrentString.IndexOf('V') + 2)..exactCurrentString.IndexOf("kW")].Trim();
                    ratedCapacity = (Math.Ceiling(Convert.ToDouble(exactCapacityCurrent, CultureInfo.CurrentCulture)) + 2).ToString() + ",0";

                    nominalVoltage = exactCurrentString[(exactCurrentString.IndexOf('A') + 2)..exactCurrentString.IndexOf('V')].Trim();
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "ratedCurrent, ratedCapacity or nominalVoltage not found");
            }
            ParameterDictionary["var_ZA_IMP_Nennstrom"].Value = ratedCurrent;
            ParameterDictionary["var_ZA_IMP_Leistung"].Value = ratedCapacity;
            ParameterDictionary["var_ZA_IMP_Stromart"].Value = nominalVoltage;
            ParameterDictionary["var_ZA_IMP_AnlaufstromMax"].Value = maxCurrent;

            ParameterDictionary["var_ZA_IMP_Motor_Pr"].Value = zliDataDictionary["Motor-Pr"];
            ParameterDictionary["var_ZA_IMP_Motor_Ur"].Value = zliDataDictionary["Bemessungsspannung"];
            ParameterDictionary["var_ZA_IMP_Motor_Ir"].Value = zliDataDictionary["Bemessungsstrom"];

            var maxEngineCurrent = string.Empty;
            try
            {
                maxEngineCurrent = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Strom bei Maximalmoment"))?.ChildNodes[2].InnerText;
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "maxEngineCurrent not found");
            }
            ParameterDictionary["var_ZA_IMP_Motor_FE_"].Value = maxEngineCurrent;
            var powerDissipation = string.Empty;
            try
            {
                powerDissipation = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Mittl. Verlustleistung"))?.ChildNodes[1].InnerText;
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "powerDissipation not found");
            }
            ParameterDictionary["var_ZA_IMP_VerlustLeistung"].Value = powerDissipation;

            ParameterDictionary["var_AufhaengungsartRope"].Value = zliDataDictionary["Aufhaengung_is"];
            ParameterDictionary["var_Umschlingungswinkel"].Value = zliDataDictionary["Treibscheibe-Umschlingung"];
            var pulleyDiameter = "0";
            var numberofFKPulley = "0";
            try
            {
                var pulleyDiameterString = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Umlenkrollen"))?.InnerText;
                if (!string.IsNullOrWhiteSpace(pulleyDiameterString))
                {
                    pulleyDiameter = pulleyDiameterString[(pulleyDiameterString.IndexOf('=') + 1)..pulleyDiameterString.IndexOf("mm")].Trim();
                    numberofFKPulley = pulleyDiameterString[(pulleyDiameterString.Length - 2)..].Trim();
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "pulleyDiameter or numberofFKPulley not found");
            }
            ParameterDictionary["var_Umlenkrollendurchmesser"].Value = pulleyDiameter;

            var numberofPulley = "0";
            try
            {
                numberofPulley = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Zahl der Umlenkrollen"))?.ChildNodes[1].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "numberofPulley not found");
            }
            ParameterDictionary["var_AnzahlUmlenkrollen"].Value = numberofPulley;
            ParameterDictionary["var_AnzahlUmlenkrollenFk"].Value = numberofFKPulley;
            ParameterDictionary["var_AnzahlUmlenkrollenGgw"].Value = (Convert.ToInt32(numberofPulley, CultureInfo.CurrentCulture) - Convert.ToInt32(numberofFKPulley, CultureInfo.CurrentCulture)).ToString();
            ParameterDictionary["var_MotorGeber"].Value = zliDataDictionary["Geber-Typ"];
        }
        _logger.LogInformation(60195, "ZAliftData imported");

        if (!onlyDiveData)
        {
            _ = _validationParameterDataService!.ValidateAllParameterAsync();
            await SetModelStateAsync();
        }

        if (zAliftDataReadyForImport || onlyDiveData)
        {
            await Task.CompletedTask;
        }
        else
        {
            await _dialogService!.MessageDialogAsync("ZAlift Dataimport", "Ziehl Abegg Liftdaten erfolgreich importiert");
        }
    }

    private HtmlDocument GetZaliftHtml()
    {
        var filePath = string.Empty;
        var zaliftHtml = new HtmlDocument();

        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
            filePath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");

        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            zaliftHtml.Load(filePath);
        return zaliftHtml;
    }

    private Dictionary<string, string> GetZliDataDictionary(HtmlDocument zaliftHtml)
    {
        var zliDataDictionary = new Dictionary<string, string>();
        if (zaliftHtml.Text is null)
            return zliDataDictionary;

        var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
                                                                        .InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        if (zliData is null)
            return zliDataDictionary;

        foreach (var zlipar in zliData)
        {

            if (!string.IsNullOrWhiteSpace(zlipar) && zlipar.Contains('='))
            {
                var zliPairValue = zlipar.Split('=');

                if (!zliDataDictionary.ContainsKey(zliPairValue[0]))
                {
                    if (zliPairValue.Length == 2)
                    {
                        zliDataDictionary.Add(zliPairValue[0], zliPairValue[1]);
                    }
                    else if (zliPairValue.Length > 2)
                    {
                        zliDataDictionary.Add(zliPairValue[0], zliPairValue[1] + "=" + zliPairValue[2]);
                    }
                }
            }
        }
        return zliDataDictionary;
    }

    [RelayCommand(CanExecute = nameof(CanOpenZALiftHtml))]
    private void OpenZiehlAbeggHtml()
    {
        SynchronizeViewModelParameter();
        var auftragsnummer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_AuftragsNummer");
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
        var auftragsnummer = ParameterDictionary?["var_AuftragsNummer"].Value;
        var pathXml = Path.GetDirectoryName(FullPathXml);

        if (!string.IsNullOrWhiteSpace(pathXml))
        {
            var pathLiloCalculation = Path.Combine(pathXml, "Berechnungen", $"{auftragsnummer}.LILO");
            if (File.Exists(pathLiloCalculation))
                startargs = pathLiloCalculation;
        }
        if (File.Exists(pathLilo))
            StartProgram(pathLilo, startargs);
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

    private async Task<bool?> CheckOutDialogAsync()
    {
        var message = """
                Die Auslegung ist aktuell nicht ausgecheckt!

                Auschecken:            Bearbeitung und Datenübernahme möglich
                                                (Wechsel zur Homeansicht um Auschecken)
                Nicht Auschecken:  Keine Datenübernahme möglich
                Abbrechen:             Zurück zu LiftDataManager
                """;
        var checkOutResult = await _dialogService!.ConfirmationDialogAsync("Auslegungsbearbeitung", message, "Auschecken", "Nicht Auschecken", "Abbrechen");
        if (checkOutResult != null)
        {
            if ((bool)checkOutResult)
            {
                _navigationService!.NavigateTo("LiftDataManager.ViewModels.HomeViewModel", "CheckOut");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return null;
        }
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
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
        finally
        {
            oXmlWriter?.Close();
        }
    }

    private void MakeBackupFile(string fullPath)
    {
        if (!File.Exists(fullPath))
            return;

        var newName = Path.Combine(Path.GetDirectoryName(fullPath)!, SpezifikationsNumber + "-LDM_Backup" + Path.GetExtension(fullPath));

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

    public void OnNavigatedTo(object parameter)
    {
        CheckCanOpenFiles();
    }

    public void OnNavigatedFrom()
    {
    }
}