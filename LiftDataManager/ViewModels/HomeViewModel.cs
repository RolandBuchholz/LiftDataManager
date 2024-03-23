using CommunityToolkit.Mvvm.Messaging.Messages;
using Humanizer;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUICommunity;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public readonly IVaultDataService _vaultDataService;
    private readonly ISettingService _settingService;
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ILogger<HomeViewModel> _logger;
    private readonly IPdfService _pdfService;
    private bool OpenReadOnly { get; set; } = true;
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";

    public HomeViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService,
                         ISettingService settingsSelectorService, IVaultDataService vaultDataService, ICalculationsModule calculationsModuleService,
                         IValidationParameterDataService validationParameterDataService, IPdfService pdfService, ILogger<HomeViewModel> logger)
        : base(parameterDataService, dialogService, navigationService, infoCenterService)
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
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

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

        if (payload < payloadTable6)
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
    private string? dataImportDescription = "Daten aus einer vorhandenen Spezifikation importieren.";

    [ObservableProperty]
    private string? dataImportDescriptionImage = "/Images/TonerSaveOFF.png";

    [ObservableProperty]
    private SpezifikationTyp? importSpezifikationTyp = SpezifikationTyp.Order;
    partial void OnImportSpezifikationTypChanged(SpezifikationTyp? value)
    {
        ImportSpezifikationName = string.Empty;

        if (value is null)
            return;
        value
            .When(SpezifikationTyp.Order).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = "/Images/TonerSaveOFF.png";
            })
            .When(SpezifikationTyp.Offer).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = "/Images/TonerSaveOFF.png";
            })
            .When(SpezifikationTyp.Planning).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = "/Images/TonerSaveOFF.png";
            })
            .When(SpezifikationTyp.Request).Then(() =>
            {
                DataImportDescription = "Daten aus einem Anfrage Formular importieren.";
                DataImportDescriptionImage = "/Images/PdfTransparent.png";
            })
            .Default(() =>
            {
                DataImportDescription = "Daten aus einer vorhandenen Spezifikation importieren.";
                DataImportDescriptionImage = "/Images/TonerSaveOFF.png";
            });
        _logger.LogInformation(60132, "ImportSpezifikationTyp changed {Typ}", value);
    }

    [ObservableProperty]
    private string? importSpezifikationName;

    [ObservableProperty]
    private string? dataImportStatusText = "Keine Daten für Import vorhanden";

    [ObservableProperty]
    private InfoBarSeverity dataImportStatus = InfoBarSeverity.Informational;

    [ObservableProperty]
    private bool showImportCarFrames;

    [ObservableProperty]
    private string? selectedImportCarFrame;

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartDataImportCommand))]
    private bool canImportSpeziData;
    partial void OnCanImportSpeziDataChanged(bool value)
    {
        DataImportStatusText = value ? $"{ImportSpezifikationName} kann importiert werden." : "Keine Daten für Import vorhanden";
    }

    [RelayCommand(CanExecute = nameof(CanLoadSpeziData))]
    private async Task LoadDataAsync()
    {
        var downloadInfo = await GetAutoDeskTransferAsync(SpezifikationName, OpenReadOnly);
        if (downloadInfo is not null)
        {
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
                await _dialogService!.MessageDialogAsync($"Datei wird von {downloadInfo.EditedBy} bearbeitet",
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

                var confirmed = await _dialogService!.ConfirmationDialogAsync(
                                        $"Es wurden mehrere {downloadInfo.FileName} Dateien gefunden?",
                                            "XML aus Vault herunterladen",
                                            "Abbrechen");
                if ((bool)confirmed)
                {
                    var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, true);

                    if (downloadResult.ExitState == ExitCodeEnum.NoError)
                    {
                        _logger.LogInformation(60139, "{FullPathXml} loaded", downloadResult.FullFileName);
                        FullPathXml = downloadResult.FullFileName;
                    }
                    else
                    {
                        await _dialogService.LiftDataManagerdownloadInfoAsync(downloadResult);
                        _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName!, downloadResult.ExitState);
                        await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, $"Fehler: {downloadResult.ExitState}");
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
                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadInfo);
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
            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen");
        }

        var data = await _parameterDataService!.LoadParameterAsync(FullPathXml);

        foreach (var item in data)
        {
            if (ParameterDictionary!.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
                updatedParameter.DataImport = true;
                if (updatedParameter.ParameterTyp == ParameterTypValue.Boolean)
                {
                    updatedParameter.Value = string.IsNullOrWhiteSpace(item.Value) ? "False" : LiftParameterHelper.FirstCharToUpperAsSpan(item.Value);
                }
                else if (updatedParameter.ParameterTyp == ParameterTypValue.Date)
                {
                    if (string.IsNullOrWhiteSpace(item.Value) || item.Value == "0")
                    {
                        updatedParameter.Value = string.Empty;
                    }
                    else if (item.Value.Contains('.'))
                    {
                        updatedParameter.Value = item.Value;
                    }
                    else
                    {
                        try
                        {
                            updatedParameter.Value = DateTime.FromOADate(Convert.ToDouble(item.Value, CultureInfo.GetCultureInfo("de-DE").NumberFormat)).ToShortDateString();
                            await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"{updatedParameter.Name} => Exceldatum in String konvertiert");
                        }
                        catch
                        {
                            updatedParameter.Value = string.Empty;
                        }
                        updatedParameter.IsDirty = true;
                    }
                }
                else
                {
                    updatedParameter.Value = item.Value is not null ? item.Value : string.Empty;
                }

                updatedParameter.Comment = item.Comment;
                updatedParameter.IsKey = item.IsKey;
                if (updatedParameter.ParameterTyp == ParameterTypValue.DropDownList)
                {
                    updatedParameter.DropDownListValue = updatedParameter.Value;
                }
                if (updatedParameter.HasErrors)
                {
                    updatedParameter.HasErrors = false;
                }
                updatedParameter.DataImport = false;
            }
            else
            {
                LogUnsupportedParameter(item.Name);
                await _infoCenterService.AddInfoCenterWarningAsync(InfoCenterEntrys, $"Parameter {item.Name} wird nicht unterstützt\n" +
                                                                                      "Überprüfen Sie die AutodeskTransfer.XML Datei");
            }
        }

        FileInfo AutoDeskTransferInfo = new(FullPathXml);
        if (!AutoDeskTransferInfo.IsReadOnly)
        {
            var parameterList = ParameterDictionary!.Values.ToList();

            XElement? doc = null;
            bool isXmlOutdated = false;
            var dataParameterList = data.Select(x => x.Name).ToList();

            for (int i = 0; i < parameterList.Count; i++)
            {
                if (!dataParameterList.Contains(parameterList[i].Name!))
                {
                    isXmlOutdated = true;
                    doc ??= XElement.Load(FullPathXml);

                    XElement? previousxmlparameter = (from para in doc.Elements("parameters").Elements("ParamWithValue")
                                                      where para.Element("name")!.Value == parameterList[i - 1].Name
                                                      select para).SingleOrDefault();
                    if (previousxmlparameter is not null)
                    {
                        var newXmlTree = new XElement("ParamWithValue",
                                    new XElement("name", parameterList[i].Name),
                                    new XElement("typeCode", parameterList[i].TypeCode.ToString()),
                                    new XElement("value", parameterList[i].Value),
                                    new XElement("comment", parameterList[i].Comment),
                                    new XElement("isKey", parameterList[i].IsKey));

                        previousxmlparameter.AddAfterSelf(new XElement(newXmlTree));
                    }
                }
            }
            if (isXmlOutdated && doc is not null)
                doc.Save(FullPathXml);
        }

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
            await ValidateAllParameterAsync();
            await SetCalculatedValuesAsync();
            _ = Messenger.Send(new QuicklinkControlMessage(new QuickLinkControlParameters()
            {
                SetDriveData = true,
                UpdateQuicklinks = true
            }));
            if (_settingService.AutoSave && CheckOut)
                StartSaveTimer();

            if (ParameterDictionary is not null && !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != pathDefaultAutoDeskTransfer))
                ParameterDictionary["var_CFPdefiniert"].Value = LiftParameterHelper.FirstCharToUpperAsSpan(File.Exists(Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".dat")).ToString());
        }
        InfoCenterIsOpen = _settingService.AutoOpenInfoCenter;
    }

    [RelayCommand(CanExecute = nameof(CanCheckOut))]
    private async Task CheckOutAsync()
    {
        OpenReadOnly = false;
        var dialogMessage = """
                                Änderung             => Änderungen mit Revisionserhöhung
                                Kleine Änderung  => Änderungen ohne Revisionserhöhung
                                """;
        var dialogResult = await _dialogService!.WarningDialogAsync(
                            "Datei eingechecked (schreibgeschützt)", dialogMessage,
                            "Änderung", "Kleine Änderung");
        await LoadDataAsync();
        if (dialogResult != null)
        {
            if ((bool)dialogResult)
                IncreaseRevision();
        }
        StartSaveTimer();
        SetModifyInfos();
    }

    [RelayCommand(CanExecute = nameof(CanClearData))]
    private async Task ClearDataAsync()
    {
        if (FullPathXml is null)
            return;

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
        _ = _validationParameterDataService.ValidateAllParameterAsync();

        _ = await _dialogService!.MessageConfirmationDialogAsync("Validation Result",
                    $"Es wurden {ParameterDictionary!.Count} Parameter überprüft.\n" +
                    $"Es wurden {ParameterErrorDictionary!.Count} Fehler/Warnungen/Informationen gefunden",
                        "Ok");
        await SetModelStateAsync();
    }

    [RelayCommand]
    private void CreatePdf()
    {
        _pdfService.MakeSinglePdfDocument("Spezifikation", ParameterDictionary!, null, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
    }

    public async Task<bool> InitializeParametereAsync()
    {
        if (!_parameterDataService!.CanConnectDataBase())
        {
            return false;
        }
        else
        {
            var data = await _parameterDataService!.InitializeParametereFromDbAsync();

            foreach (var item in data)
            {
                if (ParameterDictionary!.ContainsKey(item.Name!))
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
    private async Task DataImportAsync(ContentDialog LiftDataImportDialog)
    {
        await LiftDataImportDialog.ShowAsyncQueueDraggable();
    }

    [RelayCommand]
    private async Task PickFilePathAsync()
    {
        var filePicker = App.MainWindow.CreateOpenFilePicker();
        filePicker.ViewMode = PickerViewMode.Thumbnail;
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.FileTypeFilter.Add(".pdf");
        StorageFile file = await filePicker.PickSingleFileAsync();

        ImportSpezifikationName = (file is not null) ? file.Path : string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanImportSpeziData))]
    private async Task StartDataImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SpezifikationName))
            return;
        if (string.IsNullOrWhiteSpace(ImportSpezifikationName))
            return;

        if (string.Equals(SpezifikationName, ImportSpezifikationName))
        {
            DataImportStatus = InfoBarSeverity.Error;
            DataImportStatusText = "Datenimport kann nicht in sich selbst importiert werden!";
            return;
        }
        DataImportStatus = InfoBarSeverity.Informational;
        DataImportStatusText = "Datenimport gestartet";

        var ignoreImportParameters = new List<string>
        {
            "var_Index",
            "var_FabrikNummer",
            "var_AuftragsNummer",
            "var_Kennwort",
            "var_ErstelltVon",
            "var_FabriknummerBestand",
            "var_FreigabeErfolgtAm",
            "var_Demontage",
            "var_FertigstellungAm",
            "var_GeaendertAm",
            "var_GeaendertVon"
        };

        IEnumerable<TransferData>? importParameter;
        if (importSpezifikationTyp != SpezifikationTyp.Request)
        {
            ignoreImportParameters.Add("var_ErstelltAm");
            ignoreImportParameters.Add("var_AuslieferungAm");
            var downloadInfo = await GetAutoDeskTransferAsync(ImportSpezifikationName, true);
            if (downloadInfo is null)
            {
                DataImportStatus = InfoBarSeverity.Error;
                DataImportStatusText = "Datenimport fehlgeschlagen";
                return;
            }
            if (downloadInfo.ExitState is not ExitCodeEnum.NoError)
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = downloadInfo.ExitState.Humanize();
                return;
            }
            if (downloadInfo.FullFileName is null)
            {
                DataImportStatus = InfoBarSeverity.Error;
                DataImportStatusText = "Datenimport fehlgeschlagen Dateipfad der Importdatei konnte nicht gefunden werden";
                return;
            }

            importParameter = await _parameterDataService!.LoadParameterAsync(downloadInfo.FullFileName);
        }
        else
        {
            var importParameterPdf = await _parameterDataService!.LoadPdfOfferAsync(ImportSpezifikationName);

            if (!importParameterPdf.Any())
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = $"Die ausgewählte PDF-Datei enthält keine Daten für den Import.\n" +
                                       $"{ImportSpezifikationName}";
                return;
            }

            var isMultiCarframe = importParameterPdf.Any(x => x.Name == "var_Firma_TG2");
            ShowImportCarFrames = isMultiCarframe;
            if (!isMultiCarframe)
                SelectedImportCarFrame = null;

            if (isMultiCarframe && SelectedImportCarFrame is null)
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = "Mehrere Bausatztypen für DatenImport vorhanden.\n" +
                                       "Wählen sie den gewünschten Bausatztyp aus!";
                return;
            }

            var carTypPrefix = SelectedImportCarFrame switch
            {
                "Tiger TG2" => "_TG2",
                "BR1 1:1" => "_BR1",
                "BR2 2:1" => "_BR2",
                "Jupiter BT1" => "_BT1",
                "Jupiter BT2" => "_BT2",
                "Seil-Rucksack BRR" => "_BRR",
                "Seil-Zentral ZZE-S" => "_ZZE_S",
                _ => "_EZE_SR"
            };

            var cleanImport = new List<TransferData>();

            foreach (var item in importParameterPdf)
            {
                if (item.Name.EndsWith(carTypPrefix) || item.Name == "var_CFPOption")
                {
                    item.Name = item.Name.Replace(carTypPrefix, "");
                    cleanImport.Add(item);
                }
            }

            var carTyp = carTypPrefix switch
            {
                "_TG2" => "TG2-15 MK2",
                "_BR1" => "BR1-15 MK2",
                "_BR2" => "BR2-15 MK2",
                "_BT1" => "BT1-40",
                "_BT2" => "BT2-40",
                "_BRR" => "BRR-15 MK2",
                "_ZZE_S" => "ZZE-S1600",
                _ => "EZE-SR3200 SAO"
            };

            cleanImport.Add(new TransferData("var_Bausatz", carTyp, string.Empty, false));
            cleanImport.Add(new TransferData("var_Fahrkorbtyp", "Fremdkabine", string.Empty, false));
            importParameter = cleanImport;

            CopyPdfOffer(ImportSpezifikationName);
        }

        if (importParameter is null)
            return;

        foreach (var item in importParameter)
        {
            if (ignoreImportParameters.Contains(item.Name))
            {
                continue;
            }
            if (ParameterDictionary!.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
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
                    updatedParameter.DropDownListValue = updatedParameter.Value;
                }
                if (updatedParameter.HasErrors)
                {
                    updatedParameter.HasErrors = false;
                }
            }
            else
            {
                LogUnsupportedParameter(item.Name);
                await _infoCenterService.AddInfoCenterWarningAsync(InfoCenterEntrys, $"Parameter {item.Name} wird nicht unterstützt\n" +
                                                                                      "Überprüfen Sie die AutodeskTransfer.XML Datei");
            }
        }

        if (ParameterDictionary is null)
            return;
        if (FullPathXml is null)
            return;
        var saveResult = await _parameterDataService!.SaveAllParameterAsync(ParameterDictionary, FullPathXml, true);
        if (saveResult.Any())
            await _infoCenterService.AddInfoCenterSaveAllInfoAsync(InfoCenterEntrys, saveResult);

        await SetModelStateAsync();
        if (AutoSaveTimer is not null)
        {
            var saveTimeIntervall = AutoSaveTimer.Interval;
            AutoSaveTimer.Stop();
            AutoSaveTimer.Interval = saveTimeIntervall;
            AutoSaveTimer.Start();
        }

        DataImportStatus = InfoBarSeverity.Success;
        DataImportStatusText = $"Daten von {ImportSpezifikationName} erfolgreich importiert.\n" +
                               $"Detailinformationen im Info Sidebar Panel.\n" +
                               $"Importdialog kann geschlossen werden.";
    }

    [RelayCommand]
    private async Task FinishDataImportAsync()
    {
        ShowImportCarFrames = false;
        SelectedImportCarFrame = null;
        DataImportStatus = InfoBarSeverity.Informational;
        ImportSpezifikationTyp = SpezifikationTyp.Order;
        ImportSpezifikationName = string.Empty;
        InfoCenterIsOpen = true;
        await SetCalculatedValuesAsync();
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            CanClearData = AuftragsbezogeneXml;
            HasErrors = ParameterDictionary!.Values.Any(p => p.HasErrors);
            if (HasErrors)
                SetErrorDictionary();
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
                        IncreaseRevision();

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
            SetSettings();
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties is null)
            return;

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

    private async Task<DownloadInfo?> GetAutoDeskTransferAsync(string? liftNumber, bool ReadOnly = true)
    {
        if (!AuftragsbezogeneXml && string.IsNullOrEmpty(liftNumber))
        {
            SpezifikationName = string.Empty;
            return null;
        }

        CheckOut = false;
        var searchPattern = liftNumber + "-AutoDeskTransfer.xml";
        var watch = Stopwatch.StartNew();
        var workspaceSearch = await SearchWorkspaceAsync(searchPattern);
        var stopTimeMs = watch.ElapsedMilliseconds;

        switch (workspaceSearch.Length)
        {
            case 0:
                {
                    _logger.LogInformation(60139, "{SpezifikationName}-AutoDeskTransfer.xml not found in workspace", liftNumber);
                    await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)");
                    return await _vaultDataService.GetFileAsync(liftNumber!, ReadOnly);
                }
            case 1:
                {
                    await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Suche im Arbeitsbereich beendet {stopTimeMs} ms");
                    var autoDeskTransferpath = workspaceSearch[0];
                    FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                    if (!AutoDeskTransferInfo.IsReadOnly)
                    {
                        _logger.LogInformation(60139, "Data {searchPattern} from workspace loaded", searchPattern);
                        return new DownloadInfo()
                        {
                            ExitCode = 0,
                            CheckOutState = "CheckedOutByCurrentUser",
                            ExitState = ExitCodeEnum.NoError,
                            FullFileName = workspaceSearch[0],
                            Success = true,
                            IsCheckOut = true
                        };
                    }
                    else
                    {
                        return await _vaultDataService.GetFileAsync(liftNumber!, ReadOnly);
                    }
                }
            default:
                {
                    await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"Suche im Arbeitsbereich beendet {stopTimeMs} ms");
                    _logger.LogError(61039, "Searchresult {searchPattern} with multimatching files", searchPattern);
                    return new DownloadInfo()
                    {
                        ExitCode = 5,
                        FileName = searchPattern,
                        FullFileName = searchPattern,
                        ExitState = ExitCodeEnum.MultipleAutoDeskTransferXml
                    };
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

    private async Task<string[]> SearchWorkspaceAsync(string searchPattern)
    {
        _logger.LogInformation(60139, "Workspacesearch started");
        await _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, "Suche im Arbeitsbereich gestartet");

        string? path;

        if (CurrentSpezifikationTyp is not null &&
            CurrentSpezifikationTyp.Equals(SpezifikationTyp.Order))
        {
            path = @"C:\Work\AUFTRÄGE NEU\Konstruktion";
            if (!Directory.Exists(path))
            {
                return Array.Empty<string>();
            }
        }
        else
        {
            path = @"C:\Work\AUFTRÄGE NEU\Angebote";
            if (!Directory.Exists(path))
            {
                return Array.Empty<string>();
            }
        }

        var searchResult = await Task.Run(() => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories));
        return searchResult;
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

    private void CopyPdfOffer(string fullPath)
    {
        if (!File.Exists(fullPath))
            return;

        var currentFileName = Path.GetFileName(fullPath);
        var newFileName = currentFileName.StartsWith($"{SpezifikationName}-") ? currentFileName : $"{SpezifikationName}-{currentFileName}";
        var newFullPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "SV", newFileName);

        if (string.Equals(fullPath, newFullPath, StringComparison.CurrentCultureIgnoreCase))
            return;

        if (!string.IsNullOrWhiteSpace(newFullPath) && Path.IsPathFullyQualified(newFullPath))
        {
            if (File.Exists(newFullPath))
            {
                FileInfo pdfOfferFileInfo = new(newFullPath);
                if (pdfOfferFileInfo.IsReadOnly)
                {
                    pdfOfferFileInfo.IsReadOnly = false;
                }
            }
            File.Copy(fullPath, newFullPath, true);
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

    [LoggerMessage(61035, LogLevel.Warning,
    "Unsupported Parameter: {parameterName}")]
    partial void LogUnsupportedParameter(string parameterName);
}