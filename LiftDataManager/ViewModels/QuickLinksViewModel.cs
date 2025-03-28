using HtmlAgilityPack;
using System.Text.Json;
using System.Xml;

namespace LiftDataManager.ViewModels;

public partial class QuickLinksViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<QuicklinkControlMessage>
{
    private const string pathSynchronizeZAlift = @"C:\Work\Administration\PowerShellScripts\SynchronizeZAlift.ps1";
    private const string pathVaultPro = @"C:\Programme\Autodesk\Vault Client 2023\Explorer\Connectivity.VaultPro.exe";
    private string _pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    private readonly ParameterContext _parametercontext;
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<QuickLinksViewModel> _logger;
    private readonly IValidationParameterDataService _validationParameterDataService;
    public List<InfoCenterEntry>? SyncedParameter {  get; set; }

    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                               IValidationParameterDataService validationParameterDataService, ISettingService settingService, ILogger<DataViewModelBase> baseLogger, 
                               ParameterContext parametercontext, IVaultDataService vaultDataService, ILogger<QuickLinksViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
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
        {
            return;
        }
        if (message.Value.UpdateQuicklinks)
        {
            CheckCanOpenFiles();
        }
        if (message.Value.SetDriveData)
        {
            if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
                CanOpenZALiftHtml = File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html"));
            if (CanOpenZALiftHtml)
            {
                ImportZAliftDataAsync(true).SafeFireAndForget(onException: ex => LogTaskException(ex));
            }
        }
    }

    [ObservableProperty]
    public partial bool ShowCFPDataBaseOverriddenWarning { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSpeziPdfCommand))]
    public partial bool CanOpenSpeziPdf { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCalculationsCommand))]
    public partial bool CanOpenCalculations { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenBauerCommand))]
    public partial bool CanOpenBauer { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenVaultCommand))]
    public partial bool CanOpenVault { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenZiehlAbeggCommand))]
    public partial bool CanOpenZALift { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportZAliftDataCommand))]
    public partial bool CanImportZAliftData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenZiehlAbeggHtmlCommand))]
    public partial bool CanOpenZALiftHtml { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCFPCommand))]
    public partial bool CanOpenCFP { get; set; }

    [ObservableProperty]
    public partial bool CanImportCFPData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenLiloCommand))]
    public partial bool CanOpenLilo { get; set; }

    [ObservableProperty]
    public partial string UpdatedParameter { get; set; } = string.Empty;

    public void CheckCanOpenFiles()
    {
        SynchronizeViewModelParameter();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
        {
            CanOpenSpeziPdf = File.Exists(FullPathXml.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf"));
        }
        CanOpenVault = !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer);
        CanOpenCalculations = CanOpenVault;
        if (ParameterDictionary.Count > 0)
        {
            CanOpenCFP = File.Exists(_settingService.PathCFP) && !string.IsNullOrWhiteSpace(ParameterDictionary["var_Bausatz"].Value);
        }
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
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
        {
            CanOpenZALiftHtml = File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html"));
        }
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

        CanOpenCalculations = !VaultDisabled;
        CanOpenBauer = !VaultDisabled;
        CanOpenVault = !VaultDisabled;
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
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
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

    [RelayCommand(CanExecute = nameof(CanOpenBauer))]
    private void OpenBauer()
    {
        SynchronizeViewModelParameter();
        var auftragsnummer = ParameterDictionary["var_AuftragsNummer"].Value;
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
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
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
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            return;
        }
        SynchronizeViewModelParameter();
        CanImportZAliftData = false;
        var auftragsnummer = ParameterDictionary["var_AuftragsNummer"].Value;
        if (!CheckOut)
        {
            var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber, true);
            switch (dialogResult)
            {
                case CheckOutDialogResult.SuccessfulIncreaseRevision:
                    IncreaseRevision();
                    LikeEditParameter = true;
                    CheckOut = true;
                    break;
                case CheckOutDialogResult.SuccessfulNoRevisionChange:
                    LikeEditParameter = true;
                    CheckOut = true;
                    break;
                case CheckOutDialogResult.CheckOutFailed:
                    goto default;
                case CheckOutDialogResult.ReadOnly:
                    LikeEditParameter = false;
                    CheckOut = false;
                    break;
                default:
                    break;
            }
            if (CheckOut)
            {
                Messenger.Send(new RefreshModelStateMessage(new ModelStateParameters()
                {
                    IsCheckOut = CheckOut,
                    LikeEditParameterEnabled = LikeEditParameter
                }));
                SetModifyInfos();
            }
        }
        if (ParameterDictionary.Values.Any(p => p.IsDirty))
        {
            await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
        }
        var cFPEditDialogResult = await _dialogService.CFPEditDialogAsync(FullPathXml, ParameterDictionary["var_Bausatz"].Value);
        if (cFPEditDialogResult)
        {
            var updatedResult = await _parameterDataService.SyncFromAutodeskTransferAsync(FullPathXml, ParameterDictionary);
            if (updatedResult is not null)
            {
                if (updatedResult.Count != 0)
                {
                    await _dialogService.ParameterChangedDialogAsync(updatedResult);
                }
                ParameterDictionary["var_CFPdefiniert"].Value = "True";
            }
            _logger.LogInformation(60138, "Validate all parameter startet");
            await _validationParameterDataService.ValidateAllParameterAsync();
        }
        else
        {
            await _dialogService.MessageDialogAsync("CarFrameProgram abgebrochen",
                "Achtung:\n" +
                "Daten aus dem CarFrameProgram werden verworfen!\n" +
                "Backup wird der Autodesktransfer.xml wird wiederhergestellt!");
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenZALift))]
    private async Task OpenZiehlAbeggAsync()
    {
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            return;
        }
        SynchronizeViewModelParameter();
        CanImportZAliftData = false;
        var auftragsnummer = ParameterDictionary["var_AuftragsNummer"].Value;
        if (!CheckOut)
        {
            var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber, true);
            switch (dialogResult)
            {
                case CheckOutDialogResult.SuccessfulIncreaseRevision:
                    IncreaseRevision();
                    LikeEditParameter = true;
                    CheckOut = true;
                    break;
                case CheckOutDialogResult.SuccessfulNoRevisionChange:
                    LikeEditParameter = true;
                    CheckOut = true;
                    break;
                case CheckOutDialogResult.CheckOutFailed:
                    goto default;
                case CheckOutDialogResult.ReadOnly:
                    LikeEditParameter = false;
                    CheckOut = false;
                    break;
                default:
                    break;
            }
            if (CheckOut)
            {
                Messenger.Send(new RefreshModelStateMessage(new ModelStateParameters()
                {
                    IsCheckOut = CheckOut,
                    LikeEditParameterEnabled = LikeEditParameter
                }));
                SetModifyInfos();
            }
        }
        if (ParameterDictionary.Values.Any(p => p.IsDirty))
        {
            await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
        }
        var zALiftDialogResult = await _dialogService.ZALiftDialogAsync(FullPathXml);
        if (zALiftDialogResult)
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
        var pKDictionary = GetPKDictionary(zaliftHtml);
        SyncedParameter ??= [];
        SyncedParameter.Clear();

        if (ParameterDictionary is not null && zliDataDictionary.Count != 0 && pKDictionary.Count != 0)
        {
            var htmlNodes = zaliftHtml.DocumentNode.SelectNodes("//tr");

            if (!onlyDiveData)
            {
                SetSyncedParameter("var_Q", zliDataDictionary["Nennlast_Q"]);
                SetSyncedParameter("var_Gegengewichtsmasse", zliDataDictionary["Gegengewicht_G"]);
                try
                {
                    double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
                    double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_F");
                    double counterWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Gegengewichtsmasse");

                    SetSyncedParameter("var_GGWNutzlastausgleich", Convert.ToString(Math.Round((counterWeight - carWeight) / load, 2)));
                }
                catch (Exception)
                {
                    _logger.LogWarning(61094, "balance not found");
                }

                SetSyncedParameter("var_Erkennungsweg", LiftParameterHelper.ConvertMeterStringToMillimeterString(pKDictionary["UCM-Erkennungsweg"]));
                SetSyncedParameter("var_Totzeit", pKDictionary["UCM-Totzeit"]);
                SetSyncedParameter("var_Vdetektor", pKDictionary["UCM-Geschwindigkeitsdetektor"]);

                var brakerelease = string.Empty;
                if (zliDataDictionary["Bremse-Handlueftung"] == "ohne Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                {
                    brakerelease = "207 V Bremse. ohne Handl. Mikrosch.";
                }
                if (zliDataDictionary["Bremse-Handlueftung"] == "ohne Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                {
                    brakerelease = "207 V Bremse. ohne Hand. Indukt. NS";
                }
                if (zliDataDictionary["Bremse-Handlueftung"] == "mit Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                {
                    brakerelease = "207 V Bremse. mit Handl. Mikrosch.";
                }
                if (zliDataDictionary["Bremse-Handlueftung"] == "mit Handlueftung" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                {
                    brakerelease = "207 V Bremse. mit Handl. induktiver NS";
                }
                if (zliDataDictionary["Bremse-Handlueftung"] == "fuer Bowdenzug" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Mikroschalter")
                {
                    brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Mikrosch.";
                }
                if (zliDataDictionary["Bremse-Handlueftung"] == "fuer Bowdenzug" && zliDataDictionary["Bremse-Lueftueberwachung"] == "Naeherungsschalter")
                {
                    brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Indukt. NS";
                }
                SetSyncedParameter("var_Handlueftung", brakerelease);
                SetSyncedParameter("var_Fremdbelueftung", zliDataDictionary["Motor-Fan"] == "ohne Belüftung" || zliDataDictionary["Motor-Fan"] == "ohne" ? "False" : "True");
                SetSyncedParameter("var_ElektrBremsenansteuerung", pKDictionary["Bremsmodul-TYP"] == "ohne" ? "False" : "True");
                SetSyncedParameter("var_Treibscheibegehaertet", zliDataDictionary["Treibscheibe-RF"].Contains("gehaertet") ? "True" : "False");
            }

            SetSyncedParameter("var_Antrieb", pKDictionary["Getriebe-Typ"]);
            SetSyncedParameter("var_ZA_IMP_Regler_Typ", pKDictionary["Regler-Typ"]);
            SetSyncedParameter("var_Treibscheibendurchmesser", zliDataDictionary["Treibscheibe-D"]);
            SetSyncedParameter("var_ZA_IMP_Treibscheibe_RIA", zliDataDictionary["Treibscheibe-RIA"]);
            
            var ropeDescription = string.Empty;
            if (zliDataDictionary.TryGetValue("Treibscheibe-SD", out string? ropeDiameter))
            {
                ropeDescription = "D " + ropeDiameter + "mm " + zliDataDictionary["Treibscheibe-Seiltyp"];
            }
            else
            {
                ropeDescription = zliDataDictionary["Treibscheibe-Seiltyp"];
            }
            SetSyncedParameter("var_Tragseiltyp", ropeDescription);
            SetSyncedParameter("var_NumberOfRopes", pKDictionary["Treibscheibe-SZ"]);
            SetSyncedParameter("var_Mindestbruchlast", pKDictionary["Anlage-SBK"]);

            try
            {
                RopeCalculationData? ropeCalculationData;
                var ropeCalculationDataString = ParameterDictionary["var_RopeCalculationData"].Value;
                if (string.IsNullOrWhiteSpace(ropeCalculationDataString))
                {
                    ropeCalculationData = new RopeCalculationData() 
                    { 
                        RopeDescription = ropeDescription,
                    };
                }
                else
                {
                    ropeCalculationData = JsonSerializer.Deserialize<RopeCalculationData>(ParameterDictionary["var_RopeCalculationData"].Value!);
                }

                if (ropeCalculationData is not null)
                {
                    ropeCalculationData.RopeDescription = ropeDescription;
                    ropeCalculationData.NumberOfRopes = Convert.ToInt32(pKDictionary["Treibscheibe-SZ"]);
                    ropeCalculationData.RopeDiameter = Convert.ToDouble(zliDataDictionary["Treibscheibe-SD"], CultureInfo.CurrentCulture);
                    ropeCalculationData.MinimumBreakingStrength = Convert.ToInt32(pKDictionary["Anlage-SBK"]);
                    ropeCalculationData.WireStrength = ropeDescription.Contains("CTP") ? 2800 : 1570;
                    ropeCalculationData.RopeWeight = Convert.ToDouble(pKDictionary["Anlage-SKGM"], CultureInfo.CurrentCulture);
                    ropeCalculationData.MaximumNumberOfRopes = Convert.ToInt32(zliDataDictionary["Treibscheibe-RZ"]);
                    SetSyncedParameter("var_RopeCalculationData", JsonSerializer.Serialize(ropeCalculationData, _options).Replace("\r\n", "\n"));
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "create rope calculation data failed");
            }

            var ropeSafety = string.Empty;
            try
            {
                ropeSafety = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Seilsicherheit") || x.InnerText.StartsWith("Rope safety  nue"))?.InnerText.Split('=', '&')[1].Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "ropeSafety not found");
            }
            SetSyncedParameter("var_ZA_IMP_RopeSafety", ropeSafety);

            var tractionSheavePosition = string.Empty;
            var ropeDeflection = string.Empty;
            try
            {

                var shaftPosition = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Treibscheibe") || x.InnerText.StartsWith("Maschine"))?.InnerText.Replace("Treibscheibe", "").Replace("Maschine", "").Split(',');
                if (!string.IsNullOrWhiteSpace(shaftPosition?[0]) && !string.IsNullOrWhiteSpace(shaftPosition?[1]))
                {
                    tractionSheavePosition = HtmlEntity.DeEntitize(shaftPosition[0].Trim()) + ", " + HtmlEntity.DeEntitize(shaftPosition[1].Trim());
                }
                if (!string.IsNullOrWhiteSpace(shaftPosition?[2]))
                {
                    ropeDeflection = HtmlEntity.DeEntitize(shaftPosition[2].Trim());
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "Traction sheave position not found");
            }

            SetSyncedParameter("var_LageAntrieb", tractionSheavePosition);
            SetSyncedParameter("var_SeilabgangAntrieb", ropeDeflection);

            var compensationRopeWeight = string.Empty;
            try
            {
                compensationRopeWeight = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Unterseilgewicht"))?.ChildNodes[3].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "compensation rope weight not found");
            }
            SetSyncedParameter("var_UnterseilGewicht", compensationRopeWeight + " kg");


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
            SetSyncedParameter("var_ZA_IMP_Nennstrom", exactRatedCurrent);
            SetSyncedParameter("var_ZA_IMP_Leistung", exactCapacityCurrent);
            SetSyncedParameter("var_ZA_IMP_Nennstrom_AZ", ratedCurrent);
            SetSyncedParameter("var_ZA_IMP_Leistung_AZ", ratedCapacity);
            SetSyncedParameter("var_ZA_IMP_Stromart", nominalVoltage);
            SetSyncedParameter("var_ZA_IMP_AnlaufstromMax",maxCurrent);
            SetSyncedParameter("var_ZA_IMP_Motor_Pr", zliDataDictionary["Motor-Pr"]);
            SetSyncedParameter("var_ZA_IMP_Motor_Ur", zliDataDictionary["Bemessungsspannung"]);
            SetSyncedParameter("var_ZA_IMP_Motor_Ir", zliDataDictionary["Bemessungsstrom"]);

            var maxEngineCurrent = string.Empty;
            try
            {
                maxEngineCurrent = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Strom bei Maximalmoment"))?.ChildNodes[2].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "maxEngineCurrent not found");
            }
            SetSyncedParameter("var_ZA_IMP_Motor_Strom_Maximalmoment", maxEngineCurrent);
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

            SetSyncedParameter("var_AufhaengungsartRope", zliDataDictionary["Aufhaengung_is"]);
            SetSyncedParameter("var_Umschlingungswinkel", zliDataDictionary["Treibscheibe-Umschlingung"]);
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
            SetSyncedParameter("var_Umlenkrollendurchmesser", pulleyDiameter);

            var numberofPulley = "0";
            try
            {
                numberofPulley = htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Zahl der Umlenkrollen"))?.ChildNodes[1].InnerText.Trim();
            }
            catch (Exception)
            {
                _logger.LogWarning(61094, "numberofPulley not found");
            }
            SetSyncedParameter("var_AnzahlUmlenkrollen", numberofPulley);
            SetSyncedParameter("var_AnzahlUmlenkrollenFk", numberofFKPulley);
            SetSyncedParameter("var_AnzahlUmlenkrollenGgw", (Convert.ToInt32(numberofPulley, CultureInfo.CurrentCulture) - Convert.ToInt32(numberofFKPulley, CultureInfo.CurrentCulture)).ToString());
            SetSyncedParameter("var_MotorGeber", zliDataDictionary["Gebertyp"]);
        }
        _logger.LogInformation(60195, "ZAliftData imported");

        if (!onlyDiveData)
        {
            if (SyncedParameter.Count != 0)
            {
                await _dialogService.ParameterChangedDialogAsync(SyncedParameter);
            }
            if (ParameterDictionary is not null &&
                !string.IsNullOrWhiteSpace(FullPathXml))
            {
                await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
            }
            await _validationParameterDataService.ValidateAllParameterAsync();
            await SetModelStateAsync();
            Messenger.Send(new RefreshModelStateMessage(new ModelStateParameters()
            {
                IsCheckOut = CheckOut,
                LikeEditParameterEnabled = LikeEditParameter
            }));
        }
        else
        {
            await Task.CompletedTask;
        }
    }
    private void SetSyncedParameter(string parameterName, string? newValue)
    {
        var updatedParameter = ParameterDictionary[parameterName];
        var oldValue = updatedParameter.Value;
        if (newValue == oldValue ||
           (newValue is null && string.IsNullOrWhiteSpace(oldValue)))
        {
            return;
        }
        updatedParameter.AutoUpdateParameterValue(newValue);
        SyncedParameter?.Add(new(InfoCenterEntryState.None)
        {
            ParameterName = updatedParameter.DisplayName,
            UniqueName = updatedParameter.Name,
            OldValue = oldValue,
            NewValue = updatedParameter.Value,
        });
    }
    private HtmlDocument GetZaliftHtml()
    {
        var filePath = string.Empty;
        var zaliftHtml = new HtmlDocument();
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
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
                                                                        .InnerHtml.Split([Environment.NewLine], StringSplitOptions.None);
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

    private Dictionary<string, string> GetPKDictionary(HtmlDocument zaliftHtml)
    {
        var pKDictionary = new Dictionary<string, string>();
        if (zaliftHtml.Text is null)
        {
            return pKDictionary;
        }
        var pKData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- PK"))?
                                                                       .InnerHtml.Split(";", StringSplitOptions.None);
        if (pKData is null)
        {
            return pKDictionary;
        }

        for (int i = 0; i < pKData.Length - 1 ; i+=2)
        {
            pKDictionary.AddIfNotExists(pKData[i].Trim(), pKData[i + 1].Trim());
        }
        return pKDictionary;
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
        _pathDefaultAutoDeskTransfer = ProcessHelpers.GetDefaultAutodeskTransferPath(VaultDisabled);
        CheckCanOpenFiles();
    }

    public void OnNavigatedFrom()
    {
    }
}