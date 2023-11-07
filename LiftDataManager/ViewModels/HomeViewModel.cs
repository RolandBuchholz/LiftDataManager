using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

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

    public HomeViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                         ISettingService settingsSelectorService, IVaultDataService vaultDataService, ICalculationsModule calculationsModuleService,
                         IValidationParameterDataService validationParameterDataService, IPdfService pdfService, ILogger<HomeViewModel> logger)
        : base(parameterDataService, dialogService, navigationService)
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
            message.PropertyName == "var_F_Korr" ||
            message.PropertyName == "var_Q" ||
            message.PropertyName == "var_KBI" ||
            message.PropertyName == "var_KTI" ||
            message.PropertyName == "var_KHLicht")
        {
            _ = SetCalculatedValuesAsync();
            //Task.Run(async () => await SetCalculatedValuesAsync().ConfigureAwait(false));
        };

        if (message.PropertyName == "var_Index")
        {
            if (ParameterDictionary is not null)
            ParameterDictionary["var_StandVom"].Value = DateTime.Today.ToShortDateString();
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
        //Task.Run(async () => await SetModelStateAsync());

    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private double carFrameWeight;

    [ObservableProperty]
    private double carDoorWeight;

    [ObservableProperty]
    private double carWeight;

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
    private string? spezifikationStatusTyp;
    partial void OnSpezifikationStatusTypChanged(string? value)
    {
        SpezifikationName = string.Empty;
        _logger.LogInformation(60132, "SpezifikationStatusTyp changed {Typ}", value);
    }

    [ObservableProperty]
    private string? spezifikationName;
    partial void OnSpezifikationNameChanged(string? value)
    {
        if (value is not null)
        {
            CanLoadSpeziData = ((value.Length >= 6) && (SpezifikationStatusTyp == "Auftrag")) ||
                               ((value.Length == 10) && (SpezifikationStatusTyp == "Angebot")) ||
                               ((value.Length == 10) && (SpezifikationStatusTyp == "Vorplanung"));
        }
    }

    [ObservableProperty]
    private string? importSpezifikationStatusTyp;
    partial void OnImportSpezifikationStatusTypChanged(string? value)
    {
        ImportSpezifikationName = string.Empty;
        _logger.LogInformation(60132, "ImportSpezifikationStatusTyp changed {Typ}", value);
    }

    [ObservableProperty]
    private string? importSpezifikationName;
    partial void OnImportSpezifikationNameChanged(string? value)
    {
        if (value is not null)
        {
            CanImportSpeziData = ((value.Length >= 6) && (ImportSpezifikationStatusTyp == "Auftrag")) ||
                               ((value.Length == 10) && (ImportSpezifikationStatusTyp == "Angebot")) ||
                               ((value.Length == 10) && (ImportSpezifikationStatusTyp == "Vorplanung"));
        }
    }

    [ObservableProperty]
    private string? dataImportStatusText = "Keine Daten für Import vorhanden" ;

    [ObservableProperty]
    private InfoBarSeverity dataImportStatus = InfoBarSeverity.Informational;

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

    [RelayCommand(CanExecute = nameof(CanLoadSpeziData))]
    private async Task LoadDataAsync()
    {
        var downloadInfo = await GetAutoDeskTransferAsync(SpezifikationName, OpenReadOnly);
        if (downloadInfo is not null)
        {
            if (downloadInfo.ExitState == DownloadInfo.ExitCodeEnum.NoError)
            {
                FullPathXml = downloadInfo.FullFileName;
                switch (downloadInfo.CheckOutState)
                {
                    case "CheckedOutByCurrentUser":
                        CheckOut = true;
                        SetModifyInfos();
                        _logger.LogInformation(60139, "{FullPathXml} loaded", downloadInfo.FullFileName);
                        break;
                    case "CheckedOutByOtherUser":
                        await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadInfo);
                        _logger.LogWarning(60139, "Data locked by {EditedBy}", downloadInfo.EditedBy);
                        InfoSidebarPanelText += $"Achtung Datei wird von {downloadInfo.EditedBy} bearbeitet\n";
                        InfoSidebarPanelText += $"Kein speichern möglich!\n";
                        InfoSidebarPanelText += $"{downloadInfo.FullFileName?.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                        AuftragsbezogeneXml = true;
                        CanValidateAllParameter = true;
                        CheckOut = false;
                        LikeEditParameter = false;
                        break;
                    default:
                        CheckOut = false;
                        break;
                }
            }
            else if (downloadInfo.ExitState == DownloadInfo.ExitCodeEnum.MultipleAutoDeskTransferXml)
            {
                InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {downloadInfo.FileName} wurden gefunden\n";

                var confirmed = await _dialogService!.ConfirmationDialogAsync(
                                        $"Es wurden mehrere {downloadInfo.FileName} Dateien gefunden?",
                                            "XML aus Vault herunterladen",
                                            "Abbrechen");
                if ((bool)confirmed)
                {
                    var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, true);

                    if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                    {
                        _logger.LogInformation(60139, "{FullPathXml} loaded", downloadResult.FullFileName);
                        FullPathXml = downloadResult.FullFileName;
                    }
                    else
                    {
                        await _dialogService.LiftDataManagerdownloadInfoAsync(downloadResult);
                        _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName!, downloadResult.ExitState);
                        InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
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
                _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName, downloadInfo.ExitState);
                InfoSidebarPanelText += $"Fehler: {downloadInfo.ExitState}\n";
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
            InfoSidebarPanelText += $"Standard Daten geladen\n";
        }
        else
        {
            AuftragsbezogeneXml = true;
            CanValidateAllParameter = true;
            InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
        }

        var data = await _parameterDataService!.LoadParameterAsync(FullPathXml);

        foreach (var item in data)
        {
            if (ParameterDictionary!.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
                updatedParameter.DataImport = true;
                if (updatedParameter.ParameterTyp == ParameterBase.ParameterTypValue.Boolean)
                {
                    updatedParameter.Value = string.IsNullOrWhiteSpace(item.Value) ? "False" : LiftParameterHelper.FirstCharToUpperAsSpan(item.Value);
                }
                else if (updatedParameter.ParameterTyp == ParameterBase.ParameterTypValue.Date)
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
                            InfoSidebarPanelText += $"{updatedParameter.Name} => Exceldatum in String konvertiert!\n";
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
                if (updatedParameter.ParameterTyp == ParameterBase.ParameterTypValue.DropDownList)
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
                InfoSidebarPanelText += $"----------\n";
                InfoSidebarPanelText += $"Parameter {item.Name} wird nicht unterstützt\n";
                InfoSidebarPanelText += $"Überprüfen Sie die AutodeskTransfer.XML Datei\n";
                InfoSidebarPanelText += $"----------\n";
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
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
        _logger.LogInformation(60136, "Data loaded from {FullPathXml}", FullPathXml);
        InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
        InfoSidebarPanelText += $"----------\n";
        LikeEditParameter = true;
        OpenReadOnly = true;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;

        if (AuftragsbezogeneXml & !string.IsNullOrWhiteSpace(SpezifikationName))
        {
            await ValidateAllParameterAsync();
            await SetCalculatedValuesAsync();

            if (_settingService.AutoSave && CheckOut)
                StartSaveTimer();
        }
    }

    [RelayCommand(CanExecute = nameof(CanCheckOut))]
    private async Task CheckOutAsync()
    {
        OpenReadOnly = false;
        await LoadDataAsync();
        StartSaveTimer();
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
            InfoSidebarPanelText += $"Daten werden auf die Standardwerte zurückgesetzt\n";
            InfoSidebarPanelText += $"----------\n";

            var downloadResult = await _vaultDataService.UndoFileAsync(Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", ""));
            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
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
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else
            {
                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                _logger.LogError(61037, "Data reset failed ExitState {ExitState}", downloadResult.ExitState);
                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
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

            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
            {
                InfoSidebarPanelText += $"Spezifikation wurde hochgeladen ({stopTimeMs} ms)\n";
                InfoSidebarPanelText += $"----------\n";
                InfoSidebarPanelText += $"Standard Daten geladen\n";
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
            else
            {
                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                InfoSidebarPanelText += $"----------\n";
            }
        }
        AutoSaveTimer?.Stop();
    }

    [RelayCommand(CanExecute = nameof(CanValidateAllParameter))]
    private async Task ValidateAllParameterAsync()
    {
        _logger.LogInformation(60138, "Validate all parameter startet");
        _ = _validationParameterDataService.ValidateAllParameterAsync();
        if (!CheckoutDialogIsOpen)
        {
            _ = await _dialogService!.MessageConfirmationDialogAsync("Validation Result",
                        $"Es wurden {ParameterDictionary!.Count} Parameter überprüft.\n" +
                        $"Es wurden {ParameterErrorDictionary!.Count} Fehler/Warnungen/Informationen gefunden",
                         "Ok");
        }
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
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
            }
            LikeEditParameter = true;
            OpenReadOnly = true;
            CanCheckOut = !CheckOut && AuftragsbezogeneXml;

            return true;
        }
    }

    [RelayCommand(CanExecute = nameof(CanUpLoadSpeziData))]
    private async Task DataImportAsync(ContentDialog LiftDataImportDialog)
    {
        await LiftDataImportDialog.ShowAsync();
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

        var downloadInfo = await GetAutoDeskTransferAsync(ImportSpezifikationName, true);
        if (downloadInfo is null)
        {
            DataImportStatus = InfoBarSeverity.Error;
            DataImportStatusText = "Datenimport fehlgeschlagen";
            return;
        }
        if (downloadInfo.ExitState is not DownloadInfo.ExitCodeEnum.NoError)
        {
            DataImportStatus = InfoBarSeverity.Warning;
            DataImportStatusText = downloadInfo.DownloadInfoEnumToString();
            return;
        }
        if (downloadInfo.FullFileName is null)
        {
            DataImportStatus = InfoBarSeverity.Error;
            DataImportStatusText = "Datenimport fehlgeschlagen Dateipfad der Importdatei konnte nicht gefunden werden";
            return;
        }

        CheckOut = true;
        var importParameter = await _parameterDataService!.LoadParameterAsync(downloadInfo.FullFileName);

        string[] ignoreImportParameters =
        {
            "var_Index",
            "var_FabrikNummer",
            "var_AuftragsNummer",
            "var_Kennwort",
            "var_ErstelltVon",
            "var_ErstelltAm",
            "var_FabriknummerBestand",
            "var_FreigabeErfolgtAm",
            "var_Demontage",
            "var_AuslieferungAm",
            "var_FertigstellungAm",
            "var_GeaendertAm",
            "var_GeaendertVon"
        };

        foreach (var item in importParameter)
        {
            if (ignoreImportParameters.Contains(item.Name))
            {
                continue;
            }
            if (ParameterDictionary!.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
                if (updatedParameter.ParameterTyp != ParameterBase.ParameterTypValue.Boolean)
                {
                    updatedParameter.Value = item.Value is not null ? item.Value : string.Empty;
                }
                else
                {
                    updatedParameter.Value = string.IsNullOrWhiteSpace(item.Value) ? "False" : LiftParameterHelper.FirstCharToUpperAsSpan(item.Value);
                }
                updatedParameter.Comment = item.Comment;
                updatedParameter.IsKey = item.IsKey;
                if (updatedParameter.ParameterTyp == ParameterBase.ParameterTypValue.DropDownList)
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
                InfoSidebarPanelText += $"----------\n";
                InfoSidebarPanelText += $"Parameter {item.Name} wird nicht unterstützt\n";
                InfoSidebarPanelText += $"Überprüfen Sie die AutodeskTransfer.XML Datei\n";
                InfoSidebarPanelText += $"----------\n";
            }
        }

        if (ParameterDictionary is null)
            return;
        if (FullPathXml is null)
            return;
        var infotext = await _parameterDataService!.SaveAllParameterAsync(ParameterDictionary, FullPathXml, true);
        InfoSidebarPanelText += infotext;
        await SetModelStateAsync();
        if (AutoSaveTimer is not null)
        {
            var saveTimeIntervall = AutoSaveTimer.Interval;
            AutoSaveTimer.Stop();
            AutoSaveTimer.Interval = saveTimeIntervall;
            AutoSaveTimer.Start();
        }

        DataImportStatus = InfoBarSeverity.Success;
        DataImportStatusText = $"Daten von {ImportSpezifikationName} erfolgreich importiert";
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            CanClearData = AuftragsbezogeneXml;
            HasErrors = ParameterDictionary!.Values.Any(p => p.HasErrors);
            ParameterErrorDictionary ??= new();
            ParameterErrorDictionary.Clear();
            if (HasErrors)
            {
                var errors = ParameterDictionary.Values.Where(e => e.HasErrors);
                foreach (var error in errors)
                {
                    if (!ParameterErrorDictionary.ContainsKey(error.Name!))
                    {
                        var errorList = new List<ParameterStateInfo>();
                        errorList.AddRange(error.parameterErrors["Value"].ToList());
                        ParameterErrorDictionary.Add(error.Name!, errorList);
                    }
                    else
                    {
                        ParameterErrorDictionary[error.Name!].AddRange(error.parameterErrors["Value"].ToList());
                    }
                }
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
                var dialogResult = await _dialogService!.WarningDialogAsync(
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll die AutodeskTransferXml ausgechecked werden?",
                                    "Auschecken", "Schreibgeschützt bearbeiten");
                if ((bool)dialogResult)
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
                    CheckoutDialogIsOpen = false;
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
                InfoSidebarPanelText += $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)\n";
                return await _vaultDataService.GetFileAsync(liftNumber!, ReadOnly);
            }
            case 1:
            {
                InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                var autoDeskTransferpath = workspaceSearch[0];
                FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                if (!AutoDeskTransferInfo.IsReadOnly)
                {
                        _logger.LogInformation(60139, "Data {searchPattern} from workspace loaded", searchPattern);
                        return new DownloadInfo() 
                        {
                            ExitCode = 0,
                            CheckOutState = "CheckedOutByCurrentUser",
                            ExitState = DownloadInfo.ExitCodeEnum.NoError,
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
                InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                _logger.LogError(61039, "Searchresult {searchPattern} with multimatching files", searchPattern);
                return new DownloadInfo()
                {
                    ExitCode = 5,
                    FileName = searchPattern,
                    FullFileName = searchPattern,
                    ExitState = DownloadInfo.ExitCodeEnum.MultipleAutoDeskTransferXml
                };
            }
        }
    }

    private void SetSettings()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        CurrentSpeziProperties.Adminmode = Adminmode;
        _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
    }

    private async Task<string[]> SearchWorkspaceAsync(string searchPattern)
    {
        _logger.LogInformation(60139, "Workspacesearch started");
        InfoSidebarPanelText += $"Suche im Arbeitsbereich gestartet\n";

        string? path;

        if (SpezifikationStatusTyp == "Auftrag")
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
            ShowCarWeightBorder = !string.IsNullOrWhiteSpace(ParameterDictionary!["var_Rahmengewicht"].Value);
            ParameterDictionary!["var_F"].Value = Convert.ToString(carWeightResult.FahrkorbGewicht);
        }

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
        InfoSidebarPanelText = string.Empty;
        AuftragsbezogeneXml = false;
        CanValidateAllParameter = false;
        CanLoadSpeziData = false;
        CanSaveAllSpeziParameters = false;
        CheckOut = false;
        CanCheckOut = false;
        LikeEditParameter = true;
        SpezifikationName = string.Empty;
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
        ParameterDictionary!["var_GeaendertVon"].Value = string.IsNullOrWhiteSpace(Environment.UserName)? "Keine Angaben" : Environment.UserName;
        ParameterDictionary!["var_GeaendertAm"].Value = DateTime.Now.ToShortDateString();
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        if (CurrentSpeziProperties is null)
            SetSettings();
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        canValidateAllParameter = AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
        HideInfoErrors = CurrentSpeziProperties.HideInfoErrors;
        SpezifikationStatusTyp = (CurrentSpeziProperties.SpezifikationStatusTyp is not null) ? CurrentSpeziProperties.SpezifikationStatusTyp : "Auftrag";
        InfoSidebarPanelText = CurrentSpeziProperties.InfoSidebarPanelText;
        if (CurrentSpeziProperties.FullPathXml is not null)
        {
            FullPathXml = CurrentSpeziProperties.FullPathXml;
            SpezifikationName = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        }
        ParameterDictionary ??= new();

        //Refactor

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
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            if (CheckOut) SetModifyInfos();
            
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
        IsActive = false;
    }

    [LoggerMessage(61035, LogLevel.Warning,
    "Unsupported Parameter: {parameterName}")]
    partial void LogUnsupportedParameter(string parameterName);
}