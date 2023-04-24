using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Services;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
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
        if (message is not null)
        {
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
            SetInfoSidebarPanelText(message);
            _ = SetModelStateAsync();
            //Task.Run(async () => await SetModelStateAsync());
        }
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
    private bool canUpLoadSpeziData;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ValidateAllParameterCommand))]
    private bool canValidateAllParameter;

    [RelayCommand(CanExecute = nameof(CanLoadSpeziData))]
    private async Task LoadDataAsync()
    {
        await SetFullPathXmlAsync(OpenReadOnly);
        if (string.IsNullOrWhiteSpace(FullPathXml))
        {
            _logger.LogWarning(61033, "FullPathXml is null or whiteSpace");
            return;
        }
        var data = await _parameterDataService!.LoadParameterAsync(FullPathXml);

        foreach (var item in data)
        {
            if (ParamterDictionary!.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
                updatedParameter.DataImport = true;
                updatedParameter.Value = item.Value is not null ? item.Value : string.Empty;
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
            var parameterList = ParamterDictionary!.Values.ToList();

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
            CurrentSpeziProperties.ParamterDictionary = ParamterDictionary;
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
        _logger.LogInformation(60136, "Data loaded from {FullPathXml}",FullPathXml);
        InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
        InfoSidebarPanelText += $"----------\n";
        LikeEditParameter = true;
        OpenReadOnly = true;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        
        if (AuftragsbezogeneXml & !string.IsNullOrWhiteSpace(SpezifikationName))
        {
            await SetCalculatedValuesAsync();
            await ValidateAllParameterAsync();
            if(_settingService.AutoSave && CheckOut) StartSaveTimer();
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
            if (CheckOut)
            {
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
            }
            else
            {
                var spezifikationRootPath = Path.GetDirectoryName(FullPathXml);

                if (Directory.Exists(spezifikationRootPath) && spezifikationRootPath.StartsWith("C:\\Work\\AUFTRÄGE NEU"))
                {
                    try
                    {
                        var directory = new DirectoryInfo(spezifikationRootPath) { Attributes = FileAttributes.Normal };

                        foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                        {
                            info.Attributes = FileAttributes.Normal;
                        }

                        directory.Delete(true);
                        _logger.LogInformation(60138, "Delete Folder {spezifikationRootPath}", spezifikationRootPath);
                    }
                    catch (Exception)
                    {
                        _logger.LogError(61037, "Delete Folder {spezifikationRootPath} failed", spezifikationRootPath);
                    }

                }
                ClearExpiredLiftData();
                await LoadDataAsync();
            }
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

        var pdfcreationResult = _pdfService.MakeDefaultSetofPdfDocuments(ParamterDictionary!, FullPathXml);
            
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
                        $"Es wurden {ParamterDictionary!.Count} Parameter überprüft.\n" +
                        $"Es wurden {ParamterErrorDictionary!.Count} Fehler/Warnungen/Informationen gefunden",
                         "Ok");
        }
        await SetModelStateAsync();
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
                if (ParamterDictionary!.ContainsKey(item.Name!))
                {
                    ParamterDictionary[item.Name!] = item;
                }
                else
                {
                    ParamterDictionary.Add(item.Name!, item);
                }
            }
            if (CurrentSpeziProperties is not null)
            {
                CurrentSpeziProperties.ParamterDictionary = ParamterDictionary;
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
            }
            LikeEditParameter = true;
            OpenReadOnly = true;
            CanCheckOut = !CheckOut && AuftragsbezogeneXml;

            return true;
        }
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            CanClearData = AuftragsbezogeneXml;
            HasErrors = ParamterDictionary!.Values.Any(p => p.HasErrors);
            ParamterErrorDictionary ??= new();
            ParamterErrorDictionary.Clear();
            if (HasErrors)
            {
                var errors = ParamterDictionary.Values.Where(e => e.HasErrors);
                foreach (var error in errors)
                {
                    if (!ParamterErrorDictionary.ContainsKey(error.Name!))
                    {
                        var errorList = new List<ParameterStateInfo>();
                        errorList.AddRange(error.parameterErrors["Value"].ToList());
                        ParamterErrorDictionary.Add(error.Name!, errorList);
                    }
                    else
                    {
                        ParamterErrorDictionary[error.Name!].AddRange(error.parameterErrors["Value"].ToList());
                    }
                }
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParamterDictionary!.Values.Any(p => p.IsDirty);

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
                    if (ParamterDictionary.Values.Where(x => x.IsDirty).Count() == 1)
                    {
                        storedParmeter = ParamterDictionary.Values.First(x => x.IsDirty);
                    }
                    await LoadDataAsync();
                    if (storedParmeter != null)
                    {
                        ParamterDictionary[storedParmeter.Name!] = storedParmeter;
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

    private async Task SetFullPathXmlAsync(bool ReadOnly = true)
    {
        if (!AuftragsbezogeneXml && string.IsNullOrEmpty(SpezifikationName))
        {
            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
            SpezifikationName = string.Empty;
        }
        else
        {
            CheckOut = false;
            var searchPattern = SpezifikationName + "-AutoDeskTransfer.xml";
            var watch = Stopwatch.StartNew();
            var workspaceSearch = await SearchWorkspaceAsync(searchPattern);
            var stopTimeMs = watch.ElapsedMilliseconds;

            switch (workspaceSearch.Length)
            {
                case 0:
                    {
                        _logger.LogWarning(60139, "{SpezifikationName}-AutoDeskTransfer.xml not found", SpezifikationName);
                        InfoSidebarPanelText += $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)\n";

                        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, ReadOnly);

                        if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                        {
                            FullPathXml = downloadResult.FullFileName;
                            InfoSidebarPanelText += $"{FullPathXml!.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                            AuftragsbezogeneXml = true;
                            CanValidateAllParameter = true;
                        }
                        else
                        {
                            await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                            _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName, downloadResult.ExitState);
                            InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            AuftragsbezogeneXml = false;
                            CanValidateAllParameter = false;
                        }
                        break;
                    }
                case 1:
                    {
                        InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                        var autoDeskTransferpath = workspaceSearch[0];
                        FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                        if (!AutoDeskTransferInfo.IsReadOnly)
                        {
                            FullPathXml = workspaceSearch[0];
                            _logger.LogInformation(60139, "Data {searchPattern} from workspace loaded", searchPattern);
                            InfoSidebarPanelText += $"Die Daten {searchPattern} wurden aus dem Arbeitsberech geladen\n";
                            AuftragsbezogeneXml = true;
                            CanValidateAllParameter = true;
                            CheckOut = true;
                        }
                        else
                        {
                            var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError && !string.Equals(downloadResult.CheckOutState, "CheckedOutByOtherUser"))
                            {
                                FullPathXml = downloadResult.FullFileName;
                                _logger.LogInformation(60139, "{FullPathXml} loaded", FullPathXml);
                                InfoSidebarPanelText += $"{FullPathXml!.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                                CanValidateAllParameter = true;
                                CheckOut = downloadResult.IsCheckOut;
                            }
                            else if (string.Equals(downloadResult.CheckOutState, "CheckedOutByOtherUser"))
                            {
                                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                                FullPathXml = downloadResult.FullFileName;
                                _logger.LogWarning(60139, "Data locked by {EditedBy}", downloadResult.EditedBy);
                                InfoSidebarPanelText += $"Achtung Datei wird von {downloadResult.EditedBy} bearbeitet\n";
                                InfoSidebarPanelText += $"Kein speichern möglich!\n";
                                InfoSidebarPanelText += $"{FullPathXml!.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                                CanValidateAllParameter = true;
                                CheckOut = false;
                                LikeEditParameter = false;
                            }
                            else
                            {
                                await _dialogService!.LiftDataManagerdownloadInfoAsync(downloadResult);
                                _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName, downloadResult.ExitState);
                                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                InfoSidebarPanelText += $"Standard Daten geladen\n";
                                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                AuftragsbezogeneXml = false;
                                CanValidateAllParameter = true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        _logger.LogError(61039, "Searchresult {searchPattern} with multimatching files", searchPattern);
                        InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                        InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";

                        var confirmed = await _dialogService!.ConfirmationDialogAsync(
                                                $"Es wurden mehrere {searchPattern} Dateien gefunden?",
                                                 "XML aus Vault herunterladen",
                                                    "Abbrechen");
                        if ((bool)confirmed)
                        {
                            var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                            {
                                FullPathXml = downloadResult.FullFileName;
                                _logger.LogInformation(60139, "{FullPathXml} loaded", FullPathXml);
                                InfoSidebarPanelText += $"{FullPathXml!.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                                CanValidateAllParameter = true;
                            }
                            else
                            {
                                await _dialogService.LiftDataManagerdownloadInfoAsync(downloadResult);
                                _logger.LogError(61039, "{SpezifikationName}-AutoDeskTransfer.xml failed {downloadResult.ExitState}", SpezifikationName, downloadResult.ExitState);
                                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                InfoSidebarPanelText += $"Standard Daten geladen\n";
                                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                AuftragsbezogeneXml = false;
                                CanValidateAllParameter = false;
                            }
                        }
                        else
                        {
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            _logger.LogInformation(60139, "Standarddata loaded");
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            SpezifikationName = string.Empty;
                            AuftragsbezogeneXml = false;
                            CanValidateAllParameter = false;
                        }
                        break;
                    }
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
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParamterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParamterDictionary!, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);

        var carWeightRequestMessageMessageResult = await WeakReferenceMessenger.Default.Send<CarWeightRequestMessageAsync>();

        if (carWeightRequestMessageMessageResult is not null)
        {
            CarDoorWeight = carWeightRequestMessageMessageResult.KabinenTuerenGewicht;
            CarFrameWeight = carWeightRequestMessageMessageResult.FangrahmenGewicht;
            CarWeight = carWeightRequestMessageMessageResult.KabinenGewicht;
            ShowCarWeightBorder = !string.IsNullOrWhiteSpace(ParamterDictionary!["var_Rahmengewicht"].Value);
        }
        await Task.CompletedTask;
    }

    private void StartSaveTimer()
    {
        int period = 5;
        var autoSavePeriod = _settingService.AutoSavePeriod;
        if (!string.IsNullOrWhiteSpace(autoSavePeriod))
        {
            period = Convert.ToInt32(autoSavePeriod.Replace(" min",""));
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
            var dirty = GetCurrentSpeziProperties().ParamterDictionary!.Values.Any(p => p.IsDirty);
            if (CheckOut && dirty)
            {
                var currentSpeziProperties = GetCurrentSpeziProperties();
                await _parameterDataService!.SaveAllParameterAsync(currentSpeziProperties.ParamterDictionary!, currentSpeziProperties.FullPathXml!, currentSpeziProperties.Adminmode);
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
        ParamterErrorDictionary?.Clear();
        HasErrors = false;
        CarWeight = 0;
        CarDoorWeight = 0;
        CarFrameWeight = 0;
        
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
        ParamterDictionary ??= new();

        //Refactor

        if (CurrentSpeziProperties.ParamterDictionary is not null)
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;


        if (ParamterDictionary.Values.Count == 0)
        {
            var success = InitializeParametereAsync();

            if (!success.Result)
            {
                _logger.LogCritical(61131,"Initialize LiftDataParameter.db failed");
                throw new Exception("Initialize LiftDataParameter.db failed");
            }
        }
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = SetCalculatedValuesAsync();
            _ = SetModelStateAsync();

            if (parameter is null) return;
            if (parameter.GetType().Equals(typeof(string)))
            {
                if (string.IsNullOrWhiteSpace(parameter as string)) return;

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