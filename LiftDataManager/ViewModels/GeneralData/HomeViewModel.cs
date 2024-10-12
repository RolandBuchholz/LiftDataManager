using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly IVaultDataService _vaultDataService;
    private readonly ISettingService _settingService;
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ILogger<HomeViewModel> _logger;
    private readonly IPdfService _pdfService;
    private bool OpenReadOnly { get; set; } = true;
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";

    public HomeViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                         ISettingService settingsSelectorService, IVaultDataService vaultDataService, ICalculationsModule calculationsModuleService,
                         IValidationParameterDataService validationParameterDataService, IPdfService pdfService, ILogger<HomeViewModel> logger)
        : base(parameterDataService, dialogService, infoCenterService)
    {
        _settingService = settingsSelectorService;
        _vaultDataService = vaultDataService;
        _validationParameterDataService = validationParameterDataService;
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;
        _logger = logger;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
        {
            return;
        }

        if (!(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_Rahmengewicht" ||
            message.PropertyName == "var_KabinengewichtCAD" ||
            message.PropertyName == "var_F_Korr" ||
            message.PropertyName == "var_Q" ||
            message.PropertyName == "var_KBI" ||
            message.PropertyName == "var_KTI" ||
            message.PropertyName == "var_KHLicht")
        {
            _ = SetCalculatedValuesAsync();
            //Task.Run(async () => await SetCalculatedValuesAsync().ConfigureAwait(false));
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
        //Task.Run(async () => await SetModelStateAsync());

    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private SpezifikationTyp? currentSpezifikationTyp;
    partial void OnCurrentSpezifikationTypChanged(SpezifikationTyp? value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<SpezifikationTyp?>.Default.Equals(CurrentSpeziProperties.CurrentSpezifikationTyp, value))
        {
            CurrentSpeziProperties.CurrentSpezifikationTyp = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
        _logger.LogInformation(60132, "SpezifikationTyp changed {Typ}", value);
    }

    [ObservableProperty]
    private bool importInfo;

    [ObservableProperty]
    private string carWeightDescription = "Kabinengewicht errechnet:";

    [ObservableProperty]
    private double carFrameWeight;

    [ObservableProperty]
    private double carDoorWeight;

    [ObservableProperty]
    private double carWeight;

    [ObservableProperty]
    private bool showFrameWeightBorder;

    [ObservableProperty]
    private bool showCarWeightBorder;

    [ObservableProperty]
    private double payloadTable6;

    [ObservableProperty]
    private double payloadTable7;

    [ObservableProperty]
    private string? customPayload;
    partial void OnCustomPayloadChanged(string? value)
    {
        var payload = string.IsNullOrWhiteSpace(value) ? 0 : Convert.ToDouble(value, CultureInfo.CurrentCulture);

        if (payload < PayloadTable6)
        {
            CustomPayloadInfo = "Gedrängelast muß größer/gleich Tabelle6 sein!";
            return;
        }

        ParameterDictionary!["var_Q1"].Value = payload.ToString();
        CustomPayload = string.Empty;
        CustomPayloadInfo = string.Empty;
        _logger.LogInformation(60132, "CustomPayload", value);
    }

    [ObservableProperty]
    private string customPayloadInfo = string.Empty;

    [ObservableProperty]
    private bool canEditCustomPayload;

    [ObservableProperty]
    private string? spezifikationName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadDataCommand))]
    private bool canLoadSpeziData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckOutCommand))]
    private bool canCheckOut;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClearDataCommand))]
    private bool canClearData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UploadDataCommand))]
    [NotifyCanExecuteChangedFor(nameof(DataImportCommand))]
    private bool canUpLoadSpeziData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ValidateAllParameterCommand))]
    private bool canValidateAllParameter;

    [RelayCommand(CanExecute = nameof(CanLoadSpeziData))]
    private async Task LoadDataAsync()
    {
        (long, DownloadInfo?) downloadResult;
        if (string.IsNullOrWhiteSpace(SpezifikationName) ||
            CurrentSpezifikationTyp is null)
        {
            SpezifikationName = string.Empty;
            downloadResult.Item1 = default;
            downloadResult.Item2 = null;
        }
        else
        {
            downloadResult = await _vaultDataService.GetAutoDeskTransferAsync(SpezifikationName, CurrentSpezifikationTyp, OpenReadOnly);
        }

        if (downloadResult.Item2 is not null)
        {
            var downloadInfo = downloadResult.Item2;
            if (downloadInfo.ExitState == ExitCodeEnum.NoError)
            {
                FullPathXml = downloadInfo.FullFileName;
                switch (downloadInfo.CheckOutState)
                {
                    case "CheckedOutByCurrentUser":
                        CheckOut = true;
                        _logger.LogInformation(60139, "{FullPathXml} loaded", downloadInfo.FullFileName);
                        break;
                    default:
                        CheckOut = false;
                        break;
                }
            }
            else if (downloadInfo.ExitState == ExitCodeEnum.CheckedOutByOtherUser || downloadInfo.ExitState == ExitCodeEnum.CheckedOutLinkedFilesByOtherUser)
            {
                FullPathXml = downloadInfo.FullFileName;
                await _dialogService.MessageDialogAsync($"Datei wird von {downloadInfo.EditedBy} bearbeitet",
                    "Kein speichern möglich!\n" +
                    "\n" +
                    "Datei kann nur schreibgeschützt geöffnet werden.");
                _logger.LogWarning(60139, "Data locked by {EditedBy}", downloadInfo.EditedBy);
                await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Achtung Datei wird von {downloadInfo.EditedBy} bearbeitet\n" +
                                                                                      "Kein speichern möglich!");
                AuftragsbezogeneXml = true;
                CanValidateAllParameter = true;
                CheckOut = false;
                LikeEditParameter = false;
            }
            else if (downloadInfo.ExitState == ExitCodeEnum.MultipleAutoDeskTransferXml)
            {
                await _infoCenterService.AddInfoCenterWarningAsync(InfoCenterEntrys, $"Mehrere Dateien mit dem Namen {downloadInfo.FileName} wurden gefunden");

                var confirmed = await _dialogService.ConfirmationDialogAsync(
                                        $"Es wurden mehrere {downloadInfo.FileName} Dateien gefunden?",
                                            "XML aus Vault herunterladen",
                                            "Abbrechen");
                if ((bool)confirmed)
                {
                    var vaultDownloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, true);

                    if (vaultDownloadResult.ExitState == ExitCodeEnum.NoError)
                    {
                        _logger.LogInformation(60139, "{FullPathXml} loaded", vaultDownloadResult.FullFileName);
                        FullPathXml = vaultDownloadResult.FullFileName;
                    }
                    else
                    {
                        await _dialogService.LiftDataManagerdownloadInfoAsync(vaultDownloadResult);
                        _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName, vaultDownloadResult.ExitState);
                        await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, $"Fehler: {vaultDownloadResult.ExitState}");
                        FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                    }
                }
                else
                {
                    _logger.LogInformation(60139, "Standarddata loaded");
                    FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                }
            }
            else
            {
                await _dialogService.LiftDataManagerdownloadInfoAsync(downloadInfo);
                _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadInfo.ExitState}", SpezifikationName, downloadInfo.ExitState);
                await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, $"Fehler: {downloadInfo.ExitState}");
                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
            }
        }
        else
        {
            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
        }

        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            _logger.LogWarning(61033, "FullPathXml is null or whiteSpace");
            return;
        }

        if (string.Equals(FullPathXml, @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml"))
        {
            SpezifikationName = string.Empty;
            AuftragsbezogeneXml = false;
            CanValidateAllParameter = false;
            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Standard Daten geladen");
        }
        else
        {
            AuftragsbezogeneXml = true;
            CanValidateAllParameter = true;
            InfoCenterEntrys.Clear();
            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Suche im Arbeitsbereich nach {downloadResult.Item1} ms beendet");
        }

        var data = await _parameterDataService.LoadParameterAsync(FullPathXml);
        var newInfoCenterEntrys = await _parameterDataService.UpdateParameterDictionary(FullPathXml, data, ParameterDictionary, true);
        await _infoCenterService.AddListofInfoCenterEntrysAsync(InfoCenterEntrys, newInfoCenterEntrys);

        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.ParameterDictionary = ParameterDictionary;
            _ = Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
        _logger.LogInformation(60136, "Data loaded from {FullPathXml}", FullPathXml);
        await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Daten aus {FullPathXml} geladen");

        LikeEditParameter = true;
        OpenReadOnly = true;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;

        if (AuftragsbezogeneXml & !string.IsNullOrWhiteSpace(SpezifikationName))
        {
            if (ParameterDictionary is not null && !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
            {
                ParameterDictionary["var_CFPdefiniert"].Value = LiftParameterHelper.FirstCharToUpperAsSpan(File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".dat")).ToString());
            }
            await ValidateAllParameterAsync();
            await SetCalculatedValuesAsync();
            _ = Messenger.Send(new QuicklinkControlMessage(new QuickLinkControlParameters()
            {
                SetDriveData = true,
                UpdateQuicklinks = true
            }));
            if (_settingService.AutoSave && CheckOut)
            {
                StartSaveTimer();
            }
        }
        InfoCenterIsOpen = _settingService.AutoOpenInfoCenter;
    }

    [RelayCommand(CanExecute = nameof(CanCheckOut))]
    private async Task CheckOutAsync()
    {
        var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber, true);
        //OpenReadOnly = false;
        //await LoadDataAsync();
        if (dialogResult == CheckOutDialogResult.SuccessfulIncreaseRevision)
        {
            IncreaseRevision();
        }
        StartSaveTimer();
        SetModifyInfos();
    }

    [RelayCommand(CanExecute = nameof(CanClearData))]
    private async Task ClearDataAsync()
    {
        if (FullPathXml is null)
        {
            return;
        }
        bool? delete;
        if (CanSaveAllSpeziParameters || CheckOut)
        {
            delete = await _dialogService!.WarningDialogAsync(
                        $"Warnung es droht Datenverlust",
                        $"Es sind nichtgespeicherte Parameter vorhanden!\n" +
                        $"Die Datei wurde noch nicht ins Vault hochgeladen!\n" +
                        $"Der Befehl >Auschecken Rückgänig< wird ausgeführt!\n" +
                        $"\n" +
                        $"Soll der Vorgang fortgesetzt werden?",
                        "Fortsetzen", "Abbrechen");
        }
        else
        {
            delete = true;
        }

        if (delete is not null && (bool)delete)
        {
            _logger.LogInformation(60137, "Reset Data");
            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, "Daten werden auf die Standardwerte zurückgesetzt");

            var downloadResult = await _vaultDataService.UndoFileAsync(Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", ""));
            if (downloadResult.ExitState == ExitCodeEnum.NoError)
            {
                if (File.Exists(FullPathXml))
                {
                    var FileInfo = new FileInfo(FullPathXml);
                    if (FileInfo.IsReadOnly)
                    {
                        FileInfo.IsReadOnly = false;
                    }
                    File.Delete(FullPathXml);
                }
            }
            else
            {
                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                _logger.LogError(61037, "Data reset failed ExitState {ExitState}", downloadResult.ExitState);
                await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, $"Fehler: {downloadResult.ExitState}");
            }
            ClearExpiredLiftData();
            await LoadDataAsync();
        }
        AutoSaveTimer?.Stop();
    }

    [RelayCommand(CanExecute = nameof(CanUpLoadSpeziData))]
    private async Task UploadDataAsync()
    {
        await Task.Delay(30);
        if (string.IsNullOrWhiteSpace(SpezifikationName))
        {
            _logger.LogError(61037, "SpezifikationName are null or empty");
            return;
        }

        var pdfcreationResult = _pdfService.MakeDefaultSetofPdfDocuments(ParameterDictionary!, FullPathXml);

        _logger.LogInformation(60137, "Pdf CreationResult: {pdfcreationResult}", pdfcreationResult);

        if (CheckOut)
        {
            var watch = Stopwatch.StartNew();
            var downloadResult = await _vaultDataService.SetFileAsync(SpezifikationName);
            var stopTimeMs = watch.ElapsedMilliseconds;

            if (downloadResult.ExitState == ExitCodeEnum.NoError)
            {
                await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Spezifikation wurde hochgeladen ({stopTimeMs} ms)");
                await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, "Standard Daten geladen");
                _logger.LogInformation(60137, "upload successful");
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else if (downloadResult.ExitState == ExitCodeEnum.UpdatePropertiesError)
            {
                await _infoCenterService.AddInfoCenterWarningAsync(InfoCenterEntrys, "Vault-Ordner-Eigenschaften konnten nicht aktualisiert werden");
                await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Spezifikation wurde hochgeladen ({stopTimeMs} ms)");
                await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, "Standard Daten geladen");
                _logger.LogInformation(60138, "upload successful / property matching failed");
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else
            {
                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, $"Fehler: {downloadResult.ExitState}");
            }
        }
        AutoSaveTimer?.Stop();
    }

    [RelayCommand(CanExecute = nameof(CanValidateAllParameter))]
    private async Task ValidateAllParameterAsync()
    {
        _logger.LogInformation(60138, "Validate all parameter startet");
        await _validationParameterDataService.ValidateAllParameterAsync();
        await _dialogService.MessageConfirmationDialogAsync("Validation Result",
                    $"Es wurden {ParameterDictionary.Count} Parameter überprüft.\n" +
                    $"Es wurden {ParameterErrorDictionary.Count} Fehler/Warnungen/Informationen gefunden",
                        "Ok");
        await SetModelStateAsync();
    }

    [RelayCommand]
    private void CreatePdf()
    {
        _pdfService.MakeSinglePdfDocument("Spezifikation", ParameterDictionary, null, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
    }

    public async Task<bool> InitializeParametereAsync()
    {
        if (!_parameterDataService.CanConnectDataBase())
        {
            return false;
        }
        else
        {
            var data = await _parameterDataService.InitializeParametereFromDbAsync();

            foreach (var item in data)
            {
                if (ParameterDictionary.ContainsKey(item.Name!))
                {
                    ParameterDictionary[item.Name!] = item;
                }
                else
                {
                    ParameterDictionary.Add(item.Name!, item);
                }
            }
            if (CurrentSpeziProperties is not null)
            {
                CurrentSpeziProperties.ParameterDictionary = ParameterDictionary;
                _ = Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
            }
            LikeEditParameter = true;
            OpenReadOnly = true;
            CanCheckOut = !CheckOut && AuftragsbezogeneXml;
            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, "Parameter erfolgreich aus Datenbank geladen");
            return true;
        }
    }

    [RelayCommand(CanExecute = nameof(CanUpLoadSpeziData))]
    private async Task DataImportAsync()
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) ||
            string.IsNullOrWhiteSpace(SpezifikationName) ||
            CurrentSpezifikationTyp is null)
        {
            _logger.LogError(61039, "Liftadataimport failed fullPathXml or spezifikationName is null");
            return;
        }
        var importResult = await _dialogService.ImportLiftDataDialogAsync(FullPathXml, SpezifikationName, CurrentSpezifikationTyp);

        if (importResult.Item1 is not null)
        {
            ParameterDictionary["var_ImportiertVon"].AutoUpdateParameterValue(importResult.Item1);
        }

        var importedParameter = (importResult.Item2)?.ToList();

        if (importedParameter is not null)
        {
            List<InfoCenterEntry> syncedParameter = [];
            foreach (var item in importedParameter)
            {
                if (ParameterDictionary.TryGetValue(item.Name, out Parameter value))
                {
                    var updatedParameter = value;
                    var oldValue = updatedParameter.Value;
                    if (item.Value == oldValue ||
                      (item.Value is null && string.IsNullOrWhiteSpace(oldValue)))
                    {
                        continue;
                    }
                    if (updatedParameter.ParameterTyp != ParameterTypValue.Boolean)
                    {
                        updatedParameter.Value = item.Value is not null ? item.Value : string.Empty;
                    }
                    else
                    {
                        updatedParameter.Value = string.IsNullOrWhiteSpace(item.Value) ? "False" : LiftParameterHelper.FirstCharToUpperAsSpan(item.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(item.Comment))
                    {
                        updatedParameter.Comment = item.Comment;
                    }
                    updatedParameter.IsKey = item.IsKey;
                    if (updatedParameter.ParameterTyp == ParameterTypValue.DropDownList)
                    {
                        updatedParameter.DropDownListValue = LiftParameterHelper.GetDropDownListValue(updatedParameter.DropDownList, updatedParameter.Value);
                    }
                    if (updatedParameter.HasErrors)
                    {
                        updatedParameter.HasErrors = false;
                    }
                    syncedParameter.Add(new(InfoCenterEntryState.None)
                    {
                        ParameterName = updatedParameter.DisplayName,
                        UniqueName = updatedParameter.Name,
                        OldValue = oldValue,
                        NewValue = updatedParameter.Value,
                    });
                }
            }

            if (importedParameter.Count != 0)
            {
                await _dialogService.ParameterChangedDialogAsync(syncedParameter);
            }

            if (ParameterDictionary is null)
            {
                return;
            }
            if (FullPathXml is null)
            {
                return;
            }
            var saveResult = await _parameterDataService.SaveAllParameterAsync(ParameterDictionary, FullPathXml, true);
            if (saveResult.Count != 0)
            {
                await _infoCenterService.AddInfoCenterSaveAllInfoAsync(InfoCenterEntrys, saveResult);
            }

            await SetModelStateAsync();

            if (AutoSaveTimer is not null)
            {
                var saveTimeIntervall = AutoSaveTimer.Interval;
                AutoSaveTimer.Stop();
                AutoSaveTimer.Interval = saveTimeIntervall;
                AutoSaveTimer.Start();
            }

            InfoCenterIsOpen = true;
            await SetCalculatedValuesAsync();
        }
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            CanClearData = AuftragsbezogeneXml;
            HasErrors = ParameterDictionary!.Values.Any(p => p.HasErrors);
            if (HasErrors)
            {
                SetErrorDictionary();
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParameterDictionary!.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty;
                CanUpLoadSpeziData = !dirty && AuftragsbezogeneXml;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogMessage = """
                                     Die AutodeskTransferXml wurde noch nicht ausgechecked!

                                     Änderung             => Änderungen mit Revisionserhöhung
                                     Kleine Änderung  => Änderungen ohne Revisionserhöhung
                                     Schreibgeschützt  => Es sind keine Änderungen möglich
                                     """;
                var dialogResult = await _dialogService!.ConfirmationDialogAsync(
                                    "Datei eingechecked (schreibgeschützt)", dialogMessage,
                                    "Änderung", "Kleine Änderung", "Schreibgeschützt");
                if (dialogResult is not null)
                {
                    IsBusy = true;
                    OpenReadOnly = false;
                    Parameter? storedParmeter = null;
                    if (ParameterDictionary.Values.Where(x => x.IsDirty).Count() == 1)
                    {
                        storedParmeter = ParameterDictionary.Values.First(x => x.IsDirty);
                    }
                    await LoadDataAsync();
                    if (storedParmeter != null)
                    {
                        ParameterDictionary[storedParmeter.Name!] = storedParmeter;
                        CanSaveAllSpeziParameters = dirty;
                    };
                    if ((bool)dialogResult)
                    {
                        IncreaseRevision();
                    }

                    CheckoutDialogIsOpen = false;
                    SetModifyInfos();
                    IsBusy = false;
                }
                else
                {
                    CheckoutDialogIsOpen = false;
                    LikeEditParameter = false;
                }
            }
        }
        _logger.LogInformation(60139, "Set ModelStateAsync finished");
    }

    protected override void SynchronizeViewModelParameter()
    {
        if (CurrentSpeziProperties is null)
        {
            SetSettings();
        }
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties is null)
        {
            return;
        }

        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        CanValidateAllParameter = AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
        Adminmode = CurrentSpeziProperties.Adminmode;
        HideInfoErrors = CurrentSpeziProperties.HideInfoErrors;

        if (CurrentSpeziProperties.FullPathXml is not null)
        {
            FullPathXml = CurrentSpeziProperties.FullPathXml;
            SpezifikationName = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        }

        CurrentSpezifikationTyp = (CurrentSpeziProperties.CurrentSpezifikationTyp is not null) ? CurrentSpeziProperties.CurrentSpezifikationTyp : SpezifikationTyp.Order;

        if (CurrentSpeziProperties.InfoCenterEntrys is not null)
            InfoCenterEntrys = CurrentSpeziProperties.InfoCenterEntrys;

        if (CurrentSpeziProperties.ParameterDictionary is not null)
            ParameterDictionary = CurrentSpeziProperties.ParameterDictionary;

        if (ParameterDictionary.Values.Count == 0)
        {
            var success = InitializeParametereAsync();

            if (!success.Result)
            {
                _logger.LogCritical(61131, "Initialize LiftDataParameter.db failed");
                throw new Exception("Initialize LiftDataParameter.db failed");
            }
        }
    }

    private void SetSettings()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        CurrentSpeziProperties.Adminmode = Adminmode;
        _ = Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
    }

    private async Task SetCalculatedValuesAsync()
    {
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParameterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParameterDictionary!, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);

        PayloadTable6 = payLoadResult.NennLastTabelle6;
        PayloadTable7 = payLoadResult.NennLastTabelle7;
        CanEditCustomPayload = string.Equals(payLoadResult.CargoTyp, "Lastenaufzug") && string.Equals(payLoadResult.DriveSystem, "Hydraulik");

        var carWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParameterDictionary);

        if (carWeightResult is not null)
        {
            CarDoorWeight = carWeightResult.KabinenTuerGewicht;
            CarFrameWeight = carWeightResult.FangrahmenGewicht;
            CarWeight = carWeightResult.KabinenGewichtGesamt;
            ShowFrameWeightBorder = !string.IsNullOrWhiteSpace(ParameterDictionary!["var_Rahmengewicht"].Value);
            ShowCarWeightBorder = !string.IsNullOrWhiteSpace(ParameterDictionary!["var_KabinengewichtCAD"].Value);
            ParameterDictionary!["var_F"].AutoUpdateParameterValue(Convert.ToString(carWeightResult.FahrkorbGewicht));
        }

        string? carTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Fahrkorbtyp");
        CarWeightDescription = string.IsNullOrWhiteSpace(carTyp) || !string.Equals(carTyp, "Fremdkabine") ? "Kabinengewicht errechnet:" : "Fremdkabine Gewicht:";
        await Task.CompletedTask;
    }

    private void StartSaveTimer()
    {
        int period = 5;
        var autoSavePeriod = _settingService.AutoSavePeriod;
        if (!string.IsNullOrWhiteSpace(autoSavePeriod))
        {
            period = Convert.ToInt32(autoSavePeriod.Replace(" min", ""));
        }
        AutoSaveTimer ??= new DispatcherTimer();
        if (!AutoSaveTimer.IsEnabled)
        {
            AutoSaveTimer.Interval = TimeSpan.FromMinutes(period);
            AutoSaveTimer.Tick += Timer_Tick;
            AutoSaveTimer.Start();
        }
    }

    private async void Timer_Tick(object? sender, object e)
    {
        if (!SaveAllParameterCommand.IsRunning)
        {
            _logger.LogInformation(61038, "Autosave started");
            var dirty = GetCurrentSpeziProperties().ParameterDictionary!.Values.Any(p => p.IsDirty);
            if (CheckOut && dirty)
            {
                var currentSpeziProperties = GetCurrentSpeziProperties();
                await _parameterDataService!.SaveAllParameterAsync(currentSpeziProperties.ParameterDictionary!, currentSpeziProperties.FullPathXml!, currentSpeziProperties.Adminmode);
            }
        }
    }

    private void ClearExpiredLiftData()
    {
        InfoCenterEntrys.Clear();
        AuftragsbezogeneXml = false;
        CurrentSpezifikationTyp = SpezifikationTyp.Order;
        SpezifikationName = string.Empty;
        CanValidateAllParameter = false;
        CanLoadSpeziData = false;
        CanSaveAllSpeziParameters = false;
        CheckOut = false;
        CanCheckOut = false;
        LikeEditParameter = true;
        ShowFrameWeightBorder = false;
        ShowCarWeightBorder = false;
        ParameterErrorDictionary?.Clear();
        HasErrors = false;
        CarWeight = 0;
        CarDoorWeight = 0;
        CarFrameWeight = 0;
        PayloadTable6 = 0;
        PayloadTable7 = 0;
    }

    private void SetModifyInfos()
    {
        ParameterDictionary["var_GeaendertVon"].AutoUpdateParameterValue(string.IsNullOrWhiteSpace(Environment.UserName) ? "Keine Angaben" : Environment.UserName);
        ParameterDictionary["var_GeaendertAm"].AutoUpdateParameterValue(DateTime.Now.ToShortDateString());
    }

    private void IncreaseRevision()
    {
        if (ParameterDictionary is not null)
        {
            var newRevision = RevisionHelper.GetNextRevision(ParameterDictionary["var_Index"].Value);
            ParameterDictionary["var_Index"].AutoUpdateParameterValue(newRevision);
            ParameterDictionary["var_StandVom"].AutoUpdateParameterValue(DateTime.Today.ToShortDateString());
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();

        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            _ = SetCalculatedValuesAsync();
            _ = SetModelStateAsync();
            ImportInfo = !string.IsNullOrWhiteSpace(ParameterDictionary["var_ImportiertVon"].Value);
            if (parameter is null)
                return;
            if (parameter.GetType().Equals(typeof(string)))
            {
                if (string.IsNullOrWhiteSpace(parameter as string))
                    return;

                switch (parameter as string)
                {
                    case "CheckOut":
                        if (CanCheckOut)
                            CheckOutCommand.ExecuteAsync(parameter as string);
                        break;
                    default:
                        return;
                }
            }
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}