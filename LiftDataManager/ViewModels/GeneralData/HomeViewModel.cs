using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly IVaultDataService _vaultDataService;
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ILogger<HomeViewModel> _logger;
    private readonly IPdfService _pdfService;
    private bool OpenReadOnly { get; set; } = true;
    private string _pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";

    public HomeViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                         ISettingService settingService, IVaultDataService vaultDataService, ICalculationsModule calculationsModuleService,
                         IValidationParameterDataService validationParameterDataService, IPdfService pdfService, ILogger<HomeViewModel> logger)
        : base(parameterDataService, dialogService, infoCenterService, settingService)
    {
        _vaultDataService = vaultDataService;
        _validationParameterDataService = validationParameterDataService;
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;
        _logger = logger;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_SkipRatedLoad")
        {
            ValidateCustomPayload(CustomPayload);
            _ = SetCalculatedValuesAsync();
        };

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
    public partial string? LoadButtonLabel { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial SpezifikationTyp? CurrentSpezifikationTyp { get; set; }
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
    public partial bool ImportInfo { get; set; }

    [ObservableProperty]
    public partial string CarWeightDescription { get; set; } = "Kabinengewicht errechnet:";

    [ObservableProperty]
    public partial double CarFrameWeight { get; set; }

    [ObservableProperty]
    public partial double CarDoorWeight { get; set; }

    [ObservableProperty]
    public partial double CarWeight { get; set; }

    [ObservableProperty]
    public partial bool ShowFrameWeightBorder { get; set; }

    [ObservableProperty]
    public partial bool ShowCarWeightBorder { get; set; }

    [ObservableProperty]
    public partial double PayloadTable6 { get; set; }

    [ObservableProperty]
    public partial double PayloadTable7 { get; set; }

    [ObservableProperty]
    public partial string? CustomPayload { get; set; }
    partial void OnCustomPayloadChanged(string? value)
    {
        if (value.IsDecimal() || string.IsNullOrWhiteSpace(value))
        {
            ValidateCustomPayload(value);
        }
    }

    [ObservableProperty]
    public partial string CustomPayloadInfo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool CanEditCustomPayload { get; set; }

    [ObservableProperty]
    public partial string? SpezifikationName { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadDataCommand))]
    public partial bool CanLoadSpeziData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckOutCommand))]
    public partial bool CanCheckOut { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClearDataCommand))]
    public partial bool CanClearData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RestoreDataCommand))]
    public partial bool CanRestoreData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UploadDataCommand))]
    [NotifyCanExecuteChangedFor(nameof(DataImportCommand))]
    public partial bool CanUpLoadSpeziData { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ValidateAllParameterCommand))]
    public partial bool CanValidateAllParameter { get; set; }

    [RelayCommand]
    private async Task PickFilePathAsync()
    {
        if (!VaultDisabled)
        {
            return;
        }
        var filePicker = App.MainWindow.CreateOpenFilePicker();
        filePicker.ViewMode = PickerViewMode.Thumbnail;
        filePicker.CommitButtonText = "Auslegung laden";
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.FileTypeFilter.Add(".xml");
        StorageFile orderFile = await filePicker.PickSingleFileAsync();
        if (orderFile is null)
        {
            return;
        }
        var dataPath = _settingService.PathDataStorage;
        if (string.IsNullOrWhiteSpace(dataPath) ||
            !orderFile.Path.StartsWith(dataPath))
        {
            await _dialogService.MessageDialogAsync("Auslegungspfad nicht gültig!", """
                Der gewählte Pfad befindet sich nicht im aktuellen Arbeitsbereich.
                Überprüfen Sie den Pfad in den Einstellungen oder verschieben 
                Sie die Auslegung in den aktuellen Arbeitsbereich.
                """);
            return;
        }
        if (!orderFile.Name.EndsWith("AutoDeskTransfer.xml"))
        {
            await _dialogService.MessageDialogAsync("Datei wird nicht unterstützt!", """
                Wählen Sie eine gültige AutoDeskTransfer.xml Datei!
                """);
            return;
        }
        FullPathXml = orderFile.Path;
        SpezifikationName = orderFile.DisplayName.Replace("-AutoDeskTransfer", "");
        await LoadDataFromXmlFile(0);
    }

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
            await _parameterDataService.ResetAsync();
            await _validationParameterDataService.ResetAsync();
            await _calculationsModuleService.ResetAsync();
            await _infoCenterService.ResetAsync();
            LikeEditParameter = true;
            if (!VaultDisabled)
            {
                downloadResult = await _vaultDataService.GetAutoDeskTransferAsync(SpezifikationName, CurrentSpezifikationTyp, OpenReadOnly);
            }
            else
            {
                var path = _settingService.PathDataStorage;
                if (string.IsNullOrWhiteSpace(path))
                {
                    _logger.LogWarning(60139, "DataStoragePath is null or whiteSpace");
                    return;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Stopwatch stopWatch = new();
                stopWatch.Start();
                var searchResult = await Task.Run(() => Directory.GetFiles(path, $"{SpezifikationName}-AutoDeskTransfer.xml", SearchOption.AllDirectories));
                stopWatch.Stop();
                downloadResult.Item1 = stopWatch.ElapsedMilliseconds;
                downloadResult.Item2 = new DownloadInfo();
                switch (searchResult.Length)
                {
                    case 0:
                        {
                            var createOrder = await _dialogService.ConfirmationDialogAsync($"Es wurde keine Auslegung {SpezifikationName} gefunden? \n" +
                                $" Soll eine neue Auslegung erstellt werden?",
                                "Neue Auslegung erstellen", "Abbrechen");
                            if (!(bool)createOrder)
                            {
                                return;
                            }
                            var orderFileName = await ProcessHelpers.CreateOrderFolderStructure(path, SpezifikationName, true);
                            if (string.IsNullOrWhiteSpace(orderFileName))
                            {
                                return;
                            }
                            downloadResult.Item2.ExitCode = 0;
                            downloadResult.Item2.CheckOutState = "CheckedOutByCurrentUser";
                            downloadResult.Item2.ExitState = ExitCodeEnum.NoError;
                            downloadResult.Item2.FullFileName = orderFileName;
                            downloadResult.Item2.Success = true;
                            downloadResult.Item2.IsCheckOut = true;
                            Parameter orderParameter = new Parameter(SpezifikationName, 1, 1, "Auftrags-Nr", _validationParameterDataService)
                            {
                                Name = "var_AuftragsNummer",
                                DisplayName = "Auftragsnummer"


                            };
                            await _parameterDataService.SaveParameterAsync(orderParameter, downloadResult.Item2.FullFileName);
                            CreateOrderBackup(orderFileName);
                            _logger.LogInformation(60139, "New Order created {SpezifikationName}-AutoDeskTransfer.xml.", SpezifikationName);
                            break;
                        }
                    case 1:
                        {
                            var autoDeskTransferpath = searchResult[0];
                            FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                            if (!AutoDeskTransferInfo.IsReadOnly)
                            {
                                _logger.LogInformation(60139, "Data {SpezifikationName} from workspace loaded", SpezifikationName);
                                downloadResult.Item2.ExitCode = 0;
                                downloadResult.Item2.CheckOutState = "CheckedOutByCurrentUser";
                                downloadResult.Item2.ExitState = ExitCodeEnum.NoError;
                                downloadResult.Item2.FullFileName = searchResult[0];
                                downloadResult.Item2.Success = true;
                                downloadResult.Item2.IsCheckOut = true;
                                CreateOrderBackup(searchResult[0]);
                            }
                            else
                            {
                                await _dialogService.MessageDialogAsync("Datei ist scheibgeschützt!", """
                                     Überprüfen Sie die schreibrechte der AutoDeskTransfer.xml Datei!
                                     """);
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            await _dialogService.MessageDialogAsync($"Duplikate gefunden!", $"""
                                     Es wurden mehrere {SpezifikationName}-AutoDeskTransfer.xml Dateien gefunden?
                                     Überprüfen Sie din Arbeitsbereich und löschen die ungültigen Duplikate!
                                     """);
                            return;
                        }
                }
            }
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
                AuftragsbezogeneXml = true;
                CanValidateAllParameter = true;
                CheckOut = false;
                LikeEditParameter = false;
                await _dialogService.MessageDialogAsync($"Datei wird von {downloadInfo.EditedBy} bearbeitet",
                    "Kein speichern möglich!\n" +
                    "\n" +
                    "Datei kann nur schreibgeschützt geöffnet werden.");
                _logger.LogWarning(60139, "Data locked by {EditedBy}", downloadInfo.EditedBy);
                await _infoCenterService.AddInfoCenterMessageAsync($"Achtung Datei wird von {downloadInfo.EditedBy} bearbeitet\n" +
                                                                                      "Kein speichern möglich!");
            }
            else if (downloadInfo.ExitState == ExitCodeEnum.MultipleAutoDeskTransferXml)
            {
                await _infoCenterService.AddInfoCenterWarningAsync($"Mehrere Dateien mit dem Namen {downloadInfo.FileName} wurden gefunden");

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
                        await _infoCenterService.AddInfoCenterErrorAsync($"Fehler: {vaultDownloadResult.ExitState}");
                        FullPathXml = _pathDefaultAutoDeskTransfer;
                    }
                }
                else
                {
                    _logger.LogInformation(60139, "Standarddata loaded");
                    FullPathXml = _pathDefaultAutoDeskTransfer;
                }
            }
            else
            {
                await _dialogService.LiftDataManagerdownloadInfoAsync(downloadInfo);
                _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadInfo.ExitState}", SpezifikationName, downloadInfo.ExitState);
                await _infoCenterService.AddInfoCenterErrorAsync($"Fehler: {downloadInfo.ExitState}");
                FullPathXml = _pathDefaultAutoDeskTransfer;
            }
        }
        else
        {
            FullPathXml = _pathDefaultAutoDeskTransfer;
        }
        await LoadDataFromXmlFile(downloadResult.Item1);
    }

    private async Task LoadDataFromXmlFile(long searchTime)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            _logger.LogWarning(61033, "FullPathXml is null or whiteSpace");
            return;
        }

        if (string.Equals(FullPathXml, _pathDefaultAutoDeskTransfer))
        {
            SpezifikationName = string.Empty;
            AuftragsbezogeneXml = false;
            CanValidateAllParameter = false;
            await _infoCenterService.AddInfoCenterMessageAsync($"Standard Daten geladen");
        }
        else
        {
            AuftragsbezogeneXml = true;
            CanValidateAllParameter = true;
            InfoCenterEntrys.Clear();
            if (searchTime > 0)
            {
                await _infoCenterService.AddInfoCenterMessageAsync($"Suche im Arbeitsbereich nach {searchTime} ms beendet");
            }
        }
        var data = await _parameterDataService.LoadParameterAsync(FullPathXml);
        var newInfoCenterEntrys = await _parameterDataService.UpdateParameterDictionary(FullPathXml, data, true);
        await _infoCenterService.AddListofInfoCenterEntrysAsync(newInfoCenterEntrys);
        _logger.LogInformation(60136, "Data loaded from {FullPathXml}", FullPathXml);
        await _infoCenterService.AddInfoCenterMessageAsync($"Daten aus {FullPathXml} geladen");

        if (AuftragsbezogeneXml & !string.IsNullOrWhiteSpace(SpezifikationName))
        {
            if (ParameterDictionary is not null && !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != _pathDefaultAutoDeskTransfer))
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
        }
        InfoCenterIsOpen = _settingService.AutoOpenInfoCenter;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        OpenReadOnly = true;
    }

    [RelayCommand(CanExecute = nameof(CanCheckOut))]
    private async Task CheckOutAsync()
    {
        var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber, true);
        switch (dialogResult)
        {
            case CheckOutDialogResult.SuccessfulIncreaseRevision:
                IncreaseRevision();
                OpenReadOnly = false;
                LikeEditParameter = true;
                CheckOut = true;
                break;
            case CheckOutDialogResult.SuccessfulNoRevisionChange:
                OpenReadOnly = false;
                LikeEditParameter = true;
                CheckOut = true;
                break;
            case CheckOutDialogResult.CheckOutFailed:
                goto default;
            case CheckOutDialogResult.ReadOnly:
                LikeEditParameter = false;
                OpenReadOnly = true;
                CheckOut = false;
                break;
            default:
                break;
        }
        CanCheckOut = !CheckOut;
        if (CheckOut)
        {
            SetModifyInfos();
        }
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
            delete = await _dialogService.WarningDialogAsync(
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
            await _infoCenterService.AddInfoCenterMessageAsync("Daten werden auf die Standardwerte zurückgesetzt");

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
                await _dialogService.LiftDataManagerdownloadInfoAsync(downloadResult);
                _logger.LogError(61037, "Data reset failed ExitState {ExitState}", downloadResult.ExitState);
                await _infoCenterService.AddInfoCenterErrorAsync($"Fehler: {downloadResult.ExitState}");
            }
            ClearExpiredLiftData();
            await LoadDataAsync();
        }
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

        var pdfcreationResult = _pdfService.MakeDefaultSetofPdfDocuments(ParameterDictionary, FullPathXml);

        _logger.LogInformation(60137, "Pdf CreationResult: {pdfcreationResult}", pdfcreationResult);

        if (CheckOut)
        {
            var watch = Stopwatch.StartNew();
            var downloadResult = await _vaultDataService.SetFileAsync(SpezifikationName);
            var stopTimeMs = watch.ElapsedMilliseconds;

            if (downloadResult.ExitState == ExitCodeEnum.NoError)
            {
                await _infoCenterService.AddInfoCenterMessageAsync($"Spezifikation wurde hochgeladen ({stopTimeMs} ms)");
                await _infoCenterService.AddInfoCenterMessageAsync("Standard Daten geladen");
                _logger.LogInformation(60137, "upload successful");
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else if (downloadResult.ExitState == ExitCodeEnum.UpdatePropertiesError)
            {
                await _infoCenterService.AddInfoCenterWarningAsync("Vault-Ordner-Eigenschaften konnten nicht aktualisiert werden");
                await _infoCenterService.AddInfoCenterMessageAsync($"Spezifikation wurde hochgeladen ({stopTimeMs} ms)");
                await _infoCenterService.AddInfoCenterMessageAsync("Standard Daten geladen");
                _logger.LogInformation(60138, "upload successful / property matching failed");
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else
            {
                await _dialogService.LiftDataManagerdownloadInfoAsync(downloadResult);
                await _infoCenterService.AddInfoCenterErrorAsync($"Fehler: {downloadResult.ExitState}");
            }
        }
        await _parameterDataService.StopAutoSaveTimerAsync();
    }

    [RelayCommand]
    private async Task NewLiftDataAsync()
    {
        await Task.Delay(30);
        if (string.IsNullOrWhiteSpace(SpezifikationName) || string.IsNullOrWhiteSpace(FullPathXml))
        {
            _logger.LogError(61037, "SpezifikationName or FullPathXml are null or empty");
            return;
        }

        if (ParameterDictionary!.Values.Any(p => p.IsDirty))
        {
            var savedData = await _dialogService.ConfirmationDialogAsync("Wollen Sie die nicht gespeicherten Daten speichern?",
                "Daten speichern", "Daten verwerfen");
            if ((bool)savedData)
            {
                await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
            }
        }

        var pdfcreationResult = _pdfService.MakeDefaultSetofPdfDocuments(ParameterDictionary, FullPathXml);
        _logger.LogInformation(60137, "Pdf CreationResult: {pdfcreationResult}", pdfcreationResult);
        await _parameterDataService.StopAutoSaveTimerAsync();
        ClearExpiredLiftData();
        await LoadDataAsync();
    }

    [RelayCommand(CanExecute = nameof(CanRestoreData))]
    private async Task RestoreDataAsync()
    {
        var orderDic = Path.GetDirectoryName(FullPathXml);
        if (!string.IsNullOrWhiteSpace(orderDic))
        {
            var backupFileName = string.Concat(orderDic, "-LDM-Backup.zip");
            if (File.Exists(backupFileName))
            {
                Directory.Delete(orderDic, true);
                ZipFile.ExtractToDirectory(backupFileName, orderDic);
            }
            await LoadDataFromXmlFile(0);
            _logger.LogInformation(60139, "Backup restored {SpezifikationName}-LDM-Backup.zip.", SpezifikationName);
        }
    }

    [RelayCommand(CanExecute = nameof(CanValidateAllParameter))]
    private async Task ValidateAllParameterAsync()
    {
        _logger.LogInformation(60138, "Validate all parameter startet");
        await _validationParameterDataService.ValidateAllParameterAsync();
        await SetModelStateAsync();
        await _dialogService.ValidationDialogAsync(ParameterDictionary.Count, ParameterErrorDictionary);
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
            LikeEditParameter = true;
            OpenReadOnly = true;
            CanCheckOut = !CheckOut && AuftragsbezogeneXml;
            await _infoCenterService.AddInfoCenterMessageAsync("Parameter erfolgreich aus Datenbank geladen");
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
        var importResult = await _dialogService.ImportLiftDataDialogAsync(FullPathXml, SpezifikationName, CurrentSpezifikationTyp, VaultDisabled);

        if (importResult.Item1 is not null)
        {
            ParameterDictionary["var_ImportiertVon"].AutoUpdateParameterValue(importResult.Item1);
        }

        var importedParameter = importResult.Item2?.ToList();

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
            var saveResult = await _parameterDataService.SaveAllParameterAsync(FullPathXml, true);
            if (saveResult.Count != 0)
            {
                await _infoCenterService.AddInfoCenterSaveAllInfoAsync(saveResult);
            }
            await SetModelStateAsync();
            InfoCenterIsOpen = true;
            await SetCalculatedValuesAsync();
        }
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            CanClearData = AuftragsbezogeneXml;
            HasErrors = false;
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
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber);
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
                CheckoutDialogIsOpen = false;
                SetModifyInfos();
            }
        }
        ImportInfo = !string.IsNullOrWhiteSpace(ParameterDictionary["var_ImportiertVon"].Value);
        _logger.LogInformation(60139, "Set ModelStateAsync finished");
        await Task.CompletedTask;
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

    protected override void SetFullpathAutodeskTransfer(string? value)
    {
        base.SetFullpathAutodeskTransfer(value);
        _validationParameterDataService.SetFullPathXmlAsync(value).SafeFireAndForget();
    }

    private void SetSettings()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        VaultDisabled = _settingService.VaultDisabled;
        CurrentSpeziProperties.Adminmode = Adminmode;
        CurrentSpeziProperties.VaultDisabled = VaultDisabled;
        _ = Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
    }

    private void ValidateCustomPayload(string? customPayload)
    {
        var payload = string.IsNullOrWhiteSpace(customPayload) ? 0 : Convert.ToDouble(customPayload, CultureInfo.CurrentCulture);
        bool skipRatedLoad = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SkipRatedLoad");
        if (payload == 0 && !skipRatedLoad)
        {
            CustomPayloadInfo = string.Empty;
            return;
        }
        if (payload < PayloadTable6 && !skipRatedLoad)
        {
            CustomPayloadInfo = "Gedrängelast muß größer/gleich Tabelle6 sein!";
            return;
        }
        double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
        if (payload < load)
        {
            CustomPayloadInfo = "Gedrängelastberechnung nach EN81:20 deaktiviert! (Gedrängelast >= Nutzlast)";
            return;
        }

        ParameterDictionary["var_Q1"].Value = payload.ToString();
        CustomPayload = string.Empty;
        if (!skipRatedLoad)
        {
            CustomPayloadInfo = string.Empty;
        }
        _logger.LogInformation(60132, "Set CustomPayload: {customPayload}", customPayload);
    }

    private async Task SetCalculatedValuesAsync()
    {
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParameterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParameterDictionary, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);

        PayloadTable6 = payLoadResult.NennLastTabelle6;
        PayloadTable7 = payLoadResult.NennLastTabelle7;
        CanEditCustomPayload = string.Equals(payLoadResult.CargoTyp, "Lastenaufzug") && string.Equals(payLoadResult.DriveSystem, "Hydraulik");

        var carWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParameterDictionary);

        if (carWeightResult is not null)
        {
            CarDoorWeight = carWeightResult.KabinenTuerGewicht;
            CarFrameWeight = carWeightResult.FangrahmenGewicht;
            CarWeight = carWeightResult.KabinenGewichtGesamt;
            ShowFrameWeightBorder = !string.IsNullOrWhiteSpace(ParameterDictionary["var_Rahmengewicht"].Value);
            ShowCarWeightBorder = !string.IsNullOrWhiteSpace(ParameterDictionary["var_KabinengewichtCAD"].Value);
            ParameterDictionary["var_F"].AutoUpdateParameterValue(Convert.ToString(carWeightResult.FahrkorbGewicht));
        }

        string? carTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Fahrkorbtyp");
        CarWeightDescription = string.IsNullOrWhiteSpace(carTyp) || !string.Equals(carTyp, "Fremdkabine") ? "Kabinengewicht errechnet:" : "Fremdkabine Gewicht:";
        await Task.CompletedTask;
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

    private void CreateOrderBackup(string orderFileName)
    {
        var orderDic = Path.GetDirectoryName(orderFileName);
        if (!string.IsNullOrWhiteSpace(orderDic))
        {
            var backupFileName = string.Concat(orderDic, "-LDM-Backup.zip");
            if (File.Exists(backupFileName))
            {
                File.Delete(backupFileName);
            }
            ZipFile.CreateFromDirectory(orderDic, backupFileName);
            CanRestoreData = true;
            _logger.LogInformation(60139, "Backup created {SpezifikationName}-LDM-Backup.zip.", SpezifikationName);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();

        if (CurrentSpeziProperties is not null)
        {
            _ = SetCalculatedValuesAsync();
            _ = SetModelStateAsync();
            if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SkipRatedLoad"))
            {
                CustomPayloadInfo = "Gedrängelastberechnung nach EN81:20 deaktiviert! (Gedrängelast >= Nutzlast)";
            }
            LoadButtonLabel = VaultDisabled ? "Load or Create" : "Load Data";
            _pathDefaultAutoDeskTransfer = ProcessHelpers.GetDefaultAutodeskTransferPath(VaultDisabled);
            if (!string.IsNullOrWhiteSpace(FullPathXml) && !string.Equals(FullPathXml,_pathDefaultAutoDeskTransfer))
            { 
                CanRestoreData = File.Exists(FullPathXml?.Replace($"\\{SpezifikationName}-AutoDeskTransfer.xml", "-LDM-Backup.zip"));
            }
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}