using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace LiftDataManager.ViewModels;

public partial class QuickLinksViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<QuicklinkControlMessage>
{
    private const string pathSynchronizeZAlift = @"C:\Work\Administration\PowerShellScripts\SynchronizeZAlift.ps1";
    private const string pathVaultPro = @"C:\Programme\Autodesk\Vault Client 2023\Explorer\Connectivity.VaultPro.exe";
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";

    private readonly ISettingService _settingService;
    private readonly ParameterContext _parametercontext;
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<QuickLinksViewModel> _logger;
    private readonly IValidationParameterDataService _validationParameterDataService;

    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
        IValidationParameterDataService validationParameterDataService, ISettingService settingsSelectorService, ParameterContext parametercontext,
        IVaultDataService vaultDataService, ILogger<QuickLinksViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService)
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
    private bool showCFPDataBaseOverriddenWarning;

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

        if (ParameterDictionary.Count == 0 || string.IsNullOrWhiteSpace(ParameterDictionary["var_Aufzugstyp"].Value))
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

        if (ParameterDictionary.Count != 0 &&
            !string.IsNullOrWhiteSpace(FullPathXml) &&
            string.Equals(ParameterDictionary["var_CFPdefiniert"].Value, "True", StringComparison.CurrentCultureIgnoreCase))
        {
            var basePath = Path.GetDirectoryName(FullPathXml);
            if (!string.IsNullOrWhiteSpace(basePath))
            {
                var calculationsPath = Path.Combine(basePath, "Berechnungen");

                if (Directory.Exists(calculationsPath))
                {
                    var calculations = Directory.EnumerateFiles(calculationsPath);
                    ShowCFPDataBaseOverriddenWarning = calculations.Any(x => x.Contains("DB-Anpassungen"));
                }
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
            ProcessHelpers.StartProgram(filename, startargs);
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
                ProcessHelpers.StartProgram(filename, startargs);
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
                ProcessHelpers.StartProgram(filename, auftragsnummer);
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
                ProcessHelpers.StartProgram(filename, pathXml);
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
                ProcessHelpers.StartProgram(filename, startargs);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenCFP))]
    private async Task OpenCFP()
    {
        SynchronizeViewModelParameter();
        await SetModelStateAsync();
        if (CanSaveAllSpeziParameters)
        {
            await _parameterDataService.SaveAllParameterAsync(ParameterDictionary, FullPathXml!, Adminmode);
        }
        if (!CheckOut)
        {
            var checkOutResult = await CheckOutDialogAsync();
            if (checkOutResult is null)
            {
                return;
            }
            if ((bool)checkOutResult)
            {
                return;
            }
            var pathCFP = _settingService.PathCFP;
            //TODO NewCheckOutDialog
            //if (File.Exists(pathCFP))
            //{
            //    ProcessHelpers.StartProgram(pathCFP, startargs);
            //}
            return;
        }
        var dialog = await _dialogService.CFPEditDialogAsync(FullPathXml, ParameterDictionary["var_Bausatz"].Value);

        if (dialog)
        {
            var updatedResult = await _parameterDataService!.SyncFromAutodeskTransferAsync(FullPathXml!, ParameterDictionary!);
            if (updatedResult is not null)
            {
                if (updatedResult.Count != 0)
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
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenZALift))]
    private async Task OpenZiehlAbeggAsync()
    {
        SynchronizeViewModelParameter();
        await SetModelStateAsync();
        CanImportZAliftData = false;
        bool noEditMode = false;
        var auftragsnummer = ParameterDictionary["var_AuftragsNummer"].Value;

        if (!CheckOut)
        {
            var checkOutResult = await CheckOutDialogAsync();
            if (checkOutResult is null)
            {
                return;
            }

            if ((bool)checkOutResult)
            {
                return;
            }
            noEditMode = true;
        }
        var startargs = "StartLAST";
        var pathZALift = _settingService.PathZALift;
        if (noEditMode)
        {
            if (!File.Exists(pathZALift))
            {
                return;
            }
            ProcessHelpers.StartProgram(pathZALift, startargs);
            await Task.Delay(1000);
            if (File.Exists(pathSynchronizeZAlift))
            {
                var args = $"{pathSynchronizeZAlift} reset '{FullPathXml}'";
                var exitCode = await ProcessHelpers.StartProgramWithExitCodeAsync("PowerShell.exe", args);
                _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);

            }
            return;
        }

        if (CanSaveAllSpeziParameters)
        {
            await _parameterDataService!.SaveAllParameterAsync(ParameterDictionary, FullPathXml!, Adminmode);
        }
        var dialog = await _dialogService.ZALiftDialogAsync(FullPathXml);

        if (dialog)
        {
            CanImportZAliftData = true;
            await ImportZAliftDataAsync(false);
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
            var exitCode = await ProcessHelpers.StartProgramWithExitCodeAsync("PowerShell.exe", args);
            _logger.LogInformation(60192, "ExitCode SynchronizeZAlift.ps1: {exitCode}", exitCode);
        }
    }

    [RelayCommand(CanExecute = nameof(CanImportZAliftData))]
    private async Task ImportZAliftDataAsync(bool onlyDiveData)
    {
        var zaliftHtml = GetZaliftHtml();
        var zliDataDictionary = GetZliDataDictionary(zaliftHtml);

        if (ParameterDictionary is not null && zliDataDictionary.Count != 0)
        {
            var htmlNodes = zaliftHtml.DocumentNode.SelectNodes("//tr");

            if (!onlyDiveData)
            {
                ParameterDictionary["var_Q"].AutoUpdateParameterValue(zliDataDictionary["Nennlast_Q"]);
                ParameterDictionary["var_Gegengewichtsmasse"].AutoUpdateParameterValue(zliDataDictionary["Gegengewicht_G"]);
                try
                {
                    double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
                    double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_F");
                    double counterWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Gegengewichtsmasse");
                    ParameterDictionary["var_GGWNutzlastausgleich"].AutoUpdateParameterValue(Convert.ToString(Math.Round((counterWeight - carWeight) / load, 2)));
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
                ParameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue(detectionDistance);
                var deadTime = "0";
                try
                {
                    deadTime = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Totzeit"))?.ChildNodes[1].InnerText.Replace("ms", "").Trim();
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "deadTime not found");
                }
                ParameterDictionary["var_Totzeit"].AutoUpdateParameterValue(deadTime);
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

                ParameterDictionary["var_Handlueftung"].AutoUpdateParameterValue(brakerelease);

                var ventilation = zliDataDictionary["Motor-Fan"] != "ohne Belüftung" ? "True" : "False";
                ParameterDictionary["var_Fremdbelueftung"].AutoUpdateParameterValue(ventilation);

                try
                {
                    var brakeControl = htmlNodes.Any(x => x.InnerText.StartsWith("Bremsansteuermodul")).ToString();
                    ParameterDictionary["var_ElektrBremsenansteuerung"].AutoUpdateParameterValue(LiftParameterHelper.FirstCharToUpperAsSpan(brakeControl));
                }
                catch (Exception)
                {

                    _logger.LogWarning(61094, "ElektrBremsenansteuerung not found");
                }

                var hardened = zliDataDictionary["Treibscheibe-RF"].Contains("gehaertet") ? "True" : "False";
                ParameterDictionary["var_Treibscheibegehaertet"].AutoUpdateParameterValue(hardened);
            }

            if (zliDataDictionary.TryGetValue("Getriebebezeichnung", out string? drive))
            {
                ParameterDictionary["var_Antrieb"].AutoUpdateParameterValue(string.IsNullOrWhiteSpace(drive) ? string.Empty : drive.Replace(',', '.'));
            }
            ParameterDictionary["var_Treibscheibendurchmesser"].AutoUpdateParameterValue(zliDataDictionary["Treibscheibe-D"]);
            ParameterDictionary["var_ZA_IMP_Treibscheibe_RIA"].AutoUpdateParameterValue(zliDataDictionary["Treibscheibe-RIA"]);
            ParameterDictionary["var_ZA_IMP_Regler_Typ"].AutoUpdateParameterValue(!string.IsNullOrWhiteSpace(zliDataDictionary["Regler-Typ"]) ? zliDataDictionary["Regler-Typ"].Replace(" ", "") : string.Empty);

            if (zliDataDictionary.TryGetValue("Treibscheibe-SD", out string? ropeDiameter))
            {
                ParameterDictionary["var_Tragseiltyp"].AutoUpdateParameterValue("D " + ropeDiameter + "mm " + zliDataDictionary["Treibscheibe-Seiltyp"]);
            }
            else
            {
                ParameterDictionary["var_Tragseiltyp"].AutoUpdateParameterValue(zliDataDictionary["Treibscheibe-Seiltyp"]);
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
            ParameterDictionary["var_NumberOfRopes"].AutoUpdateParameterValue(numberOfRopes);

            var breakingload = string.Empty;
            try
            {
                breakingload = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Mindestbruchkraft"))?.ChildNodes[3].InnerText;
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "breakingload not found");
            }
            ParameterDictionary["var_Mindestbruchlast"].AutoUpdateParameterValue(breakingload);

            var ropeSafety = string.Empty;
            try
            {
                ropeSafety = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Seilsicherheit"))?.InnerText.Split('=', '&')[1].Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "ropeSafety not found");
            }
            ParameterDictionary["var_ZA_IMP_RopeSafety"].AutoUpdateParameterValue(ropeSafety);

            var exactRatedCurrent = string.Empty;
            var exactCapacityCurrent = string.Empty;
            var ratedCurrent = string.Empty;
            var maxCurrent = string.Empty;
            var ratedCapacity = string.Empty;
            var nominalVoltage = string.Empty;
            try
            {
                var exactCurrentString = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Netzstromaufnahme"))?.InnerText;

                if (!string.IsNullOrWhiteSpace(exactCurrentString))
                {
                    exactRatedCurrent = exactCurrentString[..exactCurrentString.IndexOf('A')].Replace("Netzstromaufnahme", "").Trim();
                    ratedCurrent = Math.Ceiling(Convert.ToDouble(exactRatedCurrent, CultureInfo.CurrentCulture) + 10).ToString() + ",0";
                    maxCurrent = Math.Round(Convert.ToDouble(exactRatedCurrent, CultureInfo.CurrentCulture) * 1.8 + 10, 2).ToString();

                    exactCapacityCurrent = exactCurrentString[(exactCurrentString.IndexOf('V') + 2)..exactCurrentString.IndexOf("kW")].Trim();
                    ratedCapacity = (Math.Ceiling(Convert.ToDouble(exactCapacityCurrent, CultureInfo.CurrentCulture)) + 2).ToString() + ",0";

                    nominalVoltage = exactCurrentString[(exactCurrentString.IndexOf('A') + 2)..exactCurrentString.IndexOf('V')].Trim();
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "ratedCurrent, ratedCapacity or nominalVoltage not found");
            }
            ParameterDictionary["var_ZA_IMP_Nennstrom"].AutoUpdateParameterValue(exactRatedCurrent);
            ParameterDictionary["var_ZA_IMP_Leistung"].AutoUpdateParameterValue(exactCapacityCurrent);
            ParameterDictionary["var_ZA_IMP_Nennstrom_AZ"].AutoUpdateParameterValue(ratedCurrent);
            ParameterDictionary["var_ZA_IMP_Leistung_AZ"].AutoUpdateParameterValue(ratedCapacity);
            ParameterDictionary["var_ZA_IMP_Stromart"].AutoUpdateParameterValue(nominalVoltage);
            ParameterDictionary["var_ZA_IMP_AnlaufstromMax"].AutoUpdateParameterValue(maxCurrent);

            ParameterDictionary["var_ZA_IMP_Motor_Pr"].AutoUpdateParameterValue(zliDataDictionary["Motor-Pr"]);
            ParameterDictionary["var_ZA_IMP_Motor_Ur"].AutoUpdateParameterValue(zliDataDictionary["Bemessungsspannung"]);
            ParameterDictionary["var_ZA_IMP_Motor_Ir"].AutoUpdateParameterValue(zliDataDictionary["Bemessungsstrom"]);

            var maxEngineCurrent = string.Empty;
            try
            {
                maxEngineCurrent = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Strom bei Maximalmoment"))?.ChildNodes[2].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "maxEngineCurrent not found");
            }
            ParameterDictionary["var_ZA_IMP_Motor_Strom_Maximalmoment"].AutoUpdateParameterValue(maxEngineCurrent);
            var powerDissipation = string.Empty;
            try
            {
                powerDissipation = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Mittl. Verlustleistung"))?.ChildNodes[1].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "powerDissipation not found");
            }
            ParameterDictionary["var_ZA_IMP_VerlustLeistung"].Value = powerDissipation;

            ParameterDictionary["var_AufhaengungsartRope"].AutoUpdateParameterValue(zliDataDictionary["Aufhaengung_is"]);
            ParameterDictionary["var_Umschlingungswinkel"].AutoUpdateParameterValue(zliDataDictionary["Treibscheibe-Umschlingung"]);
            var pulleyDiameter = "0";
            var numberofFKPulley = "0";
            try
            {
                var pulleyDiameterString = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Umlenkrollen"))?.InnerText.Trim();
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
            ParameterDictionary["var_Umlenkrollendurchmesser"].AutoUpdateParameterValue(pulleyDiameter);

            var numberofPulley = "0";
            try
            {
                numberofPulley = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Zahl der Umlenkrollen"))?.ChildNodes[1].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "numberofPulley not found");
            }
            ParameterDictionary["var_AnzahlUmlenkrollen"].AutoUpdateParameterValue(numberofPulley);
            ParameterDictionary["var_AnzahlUmlenkrollenFk"].AutoUpdateParameterValue(numberofFKPulley);
            ParameterDictionary["var_AnzahlUmlenkrollenGgw"].AutoUpdateParameterValue((Convert.ToInt32(numberofPulley, CultureInfo.CurrentCulture) - Convert.ToInt32(numberofFKPulley, CultureInfo.CurrentCulture)).ToString());
            ParameterDictionary["var_MotorGeber"].AutoUpdateParameterValue(zliDataDictionary["Geber-Typ"]);
        }
        _logger.LogInformation(60195, "ZAliftData imported");

        if (!onlyDiveData)
        {
            _ = _validationParameterDataService!.ValidateAllParameterAsync();
            await SetModelStateAsync();
        }

        //if (zAliftDataReadyForImport || onlyDiveData)
        //{
        //    await Task.CompletedTask;
        //}
        //else
        //{
        //    await _dialogService!.MessageDialogAsync("ZAlift Dataimport", "Ziehl Abegg Liftdaten erfolgreich importiert");
        //}
    }
    private HtmlDocument GetZaliftHtml()
    {
        var filePath = string.Empty;
        var zaliftHtml = new HtmlDocument();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
        {
            filePath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");
        }
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
        {
            zaliftHtml.Load(filePath);
        }
        return zaliftHtml;
    }
    private Dictionary<string, string> GetZliDataDictionary(HtmlDocument zaliftHtml)
    {
        var zliDataDictionary = new Dictionary<string, string>();
        if (zaliftHtml.Text is null)
        {
            return zliDataDictionary;
        }
        var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
                                                                        .InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        if (zliData is null)
        {
            return zliDataDictionary;
        }
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
            ProcessHelpers.StartProgram(filename, startargs);
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
            {
                startargs = pathLiloCalculation;
            }
        }
        if (File.Exists(pathLilo))
        {
            ProcessHelpers.StartProgram(pathLilo, startargs);
        }
    }

    [RelayCommand]
    private void RefreshQuickLinks()
    {
        CheckCanOpenFiles();
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
                LiftParameterNavigationHelper.NavigateToPage(typeof(HomePage), "CheckOut");
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

    public void OnNavigatedTo(object parameter)
    {
        CheckCanOpenFiles();
    }

    public void OnNavigatedFrom()
    {
    }
}