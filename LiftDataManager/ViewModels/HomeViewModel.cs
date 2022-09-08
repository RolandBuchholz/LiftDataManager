using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : ObservableRecipient, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly IParameterDataService _parameterDataService;
    private readonly ISettingService _settingService;
    private readonly IVaultDataService _vaultDataService;
    private readonly IDialogService _dialogService;
    private CurrentSpeziProperties? _CurrentSpeziProperties;
    private bool Adminmode
    {
        get; set;
    }
    private bool OpenReadOnly { get; set; } = true;
    public bool CheckoutDialogIsOpen
    {
        get; private set;
    }
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();

    public HomeViewModel(IParameterDataService parameterDataService, ISettingService settingsSelectorService, IVaultDataService vaultDataService, IDialogService dialogService)
    {
        _parameterDataService = parameterDataService;
        _settingService = settingsSelectorService;
        _vaultDataService = vaultDataService;
        _dialogService = dialogService;
        CheckOutSpeziDataAsync = new AsyncRelayCommand(CheckOutAsync, () => CanCheckOut);
        ClearSpeziDataAsync = new AsyncRelayCommand(ClearDataAsync, () => CanClearData);
        LoadSpeziDataAsync = new AsyncRelayCommand(LoadDataAsync, () => CanLoadSpeziData);
        UploadSpeziDataAsync = new AsyncRelayCommand(UploadDataAsync, () => CanUpLoadSpeziData && AuftragsbezogeneXml);
        SaveAllParameterCommand = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
    }

    public void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null )
        {
            // ToDo Validation Service integrieren
            if (message.PropertyName == "var_Rahmengewicht" ||
                message.PropertyName == "var_F_Korr" ||
                message.PropertyName == "var_Q" ||
                message.PropertyName == "var_KBI" ||
                message.PropertyName == "var_KTI" ||
                message.PropertyName == "var_KHLicht")
            {
                _ = SetCalculatedValuesAsync();
            };

            SetInfoSidebarPanelText(message);
            _ = CheckUnsavedParametresAsync();
        }
    }

    public IAsyncRelayCommand CheckOutSpeziDataAsync { get; }
    public IAsyncRelayCommand ClearSpeziDataAsync { get; }
    public IAsyncRelayCommand LoadSpeziDataAsync{ get; }
    public IAsyncRelayCommand UploadSpeziDataAsync { get; }
    public IAsyncRelayCommand SaveAllParameterCommand { get;}
     
    private bool _CanCheckOut;
    public bool CanCheckOut
    {
        get => _CanCheckOut;
        set
        {
            SetProperty(ref _CanCheckOut, value);
            CheckOutSpeziDataAsync.NotifyCanExecuteChanged();
        }
    }

    private bool _CanClearData;
    public bool CanClearData
    {
        get => _CanClearData;
        set
        {
            SetProperty(ref _CanClearData, value);
            ClearSpeziDataAsync.NotifyCanExecuteChanged();
        }
    }

    private bool _CanLoadSpeziData;
    public bool CanLoadSpeziData
    {
        get => _CanLoadSpeziData;
        set
        {
            SetProperty(ref _CanLoadSpeziData, value);
            LoadSpeziDataAsync.NotifyCanExecuteChanged();
        }
    }

    private bool _CanUpLoadSpeziData;
    public bool CanUpLoadSpeziData
    {
        get => _CanUpLoadSpeziData;
        set
        {
            SetProperty(ref _CanUpLoadSpeziData, value);
            UploadSpeziDataAsync.NotifyCanExecuteChanged();
        }
    }

    private bool _CanSaveAllSpeziParameters;
    public bool CanSaveAllSpeziParameters
    {
        get => _CanSaveAllSpeziParameters;
        set
        {
            SetProperty(ref _CanSaveAllSpeziParameters, value);
            CanUpLoadSpeziData = !value;
            SaveAllParameterCommand.NotifyCanExecuteChanged();
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

    private async Task CheckUnsavedParametresAsync()
    {
        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService.WarningDialogAsync(App.MainRoot!,
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll die AutodeskTransferXml ausgechecked werden?",
                                    "Auschecken", "Schreibgeschützt bearbeiten");
                if (dialogResult)
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
                        ParamterDictionary[storedParmeter.Name] = storedParmeter;
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
    }

    private bool _AuftragsbezogeneXml;
    public bool AuftragsbezogeneXml
    {
        get => _AuftragsbezogeneXml;
        set
        {
            SetProperty(ref _AuftragsbezogeneXml, value);
            if (_CurrentSpeziProperties is not null)
            {
            _CurrentSpeziProperties.AuftragsbezogeneXml = value;
            }
            CanClearData = value;
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private bool _CheckOut;
    public bool CheckOut
    {
        get => _CheckOut;
        set
        {
            SetProperty(ref _CheckOut, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.CheckOut = value;
            }
            CanUpLoadSpeziData = value;
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private bool _LikeEditParameter;
    public bool LikeEditParameter
    {
        get => _LikeEditParameter;
        set
        {
            SetProperty(ref _LikeEditParameter, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.LikeEditParameter = value;
            }
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private string? _SpezifikationStatusTyp;
    public string? SpezifikationStatusTyp
    {
        get
        {
            _SpezifikationStatusTyp ??= "Auftrag";
            return _SpezifikationStatusTyp;
        }
        set
        {
            if (_SpezifikationStatusTyp != value) { SpezifikationName = string.Empty; }
            SetProperty(ref _SpezifikationStatusTyp, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.SpezifikationStatusTyp = value;
            }
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private string? _SpezifikationName;
    public string? SpezifikationName
    {
        get => _SpezifikationName;
        set
        {
            SetProperty(ref _SpezifikationName, value);
            if (SpezifikationName is not null)
            {
            CanLoadSpeziData = ((SpezifikationName.Length >= 6) && (SpezifikationStatusTyp == "Auftrag")) ||
                               ((SpezifikationName.Length == 10) && (SpezifikationStatusTyp == "Angebot")) ||
                               ((SpezifikationName.Length == 10) && (SpezifikationStatusTyp == "Vorplanung"));
            }
        }
    }

    private string? _FullPathXml;
    public string? FullPathXml
    {
        get => _FullPathXml;
        set
        {
            SetProperty(ref _FullPathXml, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.FullPathXml = value;
            }
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private string? _SearchInput;
    public string? SearchInput
    {
        get => _SearchInput;
        set
        {
            SetProperty(ref _SearchInput, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.SearchInput = value;
            }
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private string? _InfoSidebarPanelText;
    public string? InfoSidebarPanelText
    {
        get => _InfoSidebarPanelText;

        set
        {
            SetProperty(ref _InfoSidebarPanelText, value);
            if (_CurrentSpeziProperties is not null)
            {
                _CurrentSpeziProperties.InfoSidebarPanelText = value;
            }
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
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
                        InfoSidebarPanelText += $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)\n";

                        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                        if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                        {
                            FullPathXml = downloadResult.FullFileName;
                            InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                            AuftragsbezogeneXml = true;
                        }
                        else
                        {
                            await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                            InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            AuftragsbezogeneXml = false;
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
                            InfoSidebarPanelText += $"Die Daten {searchPattern} wurden aus dem Arbeitsberech geladen\n";
                            AuftragsbezogeneXml = true;
                            CheckOut = true;
                        }
                        else
                        {
                            var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                            {
                                FullPathXml = downloadResult.FullFileName;
                                InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                                CheckOut = downloadResult.IsCheckOut;
                            }
                            else
                            {
                                await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                InfoSidebarPanelText += $"Standard Daten geladen\n";
                                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                AuftragsbezogeneXml = false;
                            }
                        }
                        break;
                    }

                default:
                    {
                        InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                        InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";

                        var confirmed = await _dialogService.ConfirmationDialogAsync(App.MainRoot!,
                                                $"Es wurden mehrere {searchPattern} Dateien gefunden?",
                                                 "XML aus Vault herunterladen",
                                                    "Abbrechen");
                        if (confirmed)
                        {
                            var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                            {
                                FullPathXml = downloadResult.FullFileName;
                                InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                            }
                            else
                            {
                                await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                InfoSidebarPanelText += $"Standard Daten geladen\n";
                                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                AuftragsbezogeneXml = false;
                            }
                        }
                        else
                        {
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            SpezifikationName = string.Empty;
                            AuftragsbezogeneXml = false;
                        }
                        break;
                    }
            }
        }
    }

    private async Task CheckOutAsync()
    {
        OpenReadOnly = false;
        await LoadDataAsync();
    }

    private async Task ClearDataAsync()
    {
        var delete = true;

        if (CanSaveAllSpeziParameters || CheckOut)
        {
            delete = await _dialogService.WarningDialogAsync(App.MainRoot!,
                    $"Warnung es droht Datenverlust",
                    $"Es sind nichtgespeicherte Parameter vorhanden!\n" +
                    $"Die Datei wurde noch nicht ins Vault hochgeladen!\n" +
                    $"Der Befehl >Auschecken Rückgänig< wird ausgeführt!\n" +
                    $"\n" +
                    $"Soll der Vorgang fortgesetzt werden?",
                    "Fortsetzen", "Abbrechen");
        }

        if (delete)
        {
            InfoSidebarPanelText += $"Daten werden auf die Standardwerte zurückgesetzt\n";
            InfoSidebarPanelText += $"----------\n";
            if (CheckOut)
            {
                var downloadResult = await _vaultDataService.UndoFileAsync(Path.GetFileNameWithoutExtension(FullPathXml)?.Replace("-AutoDeskTransfer", ""));
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
                    AuftragsbezogeneXml = false;
                    SpezifikationName = string.Empty;
                    CanLoadSpeziData = false;
                    CanSaveAllSpeziParameters = false;
                    CheckOut = false;
                    LikeEditParameter = true;
                    await LoadDataAsync();
                }
                else
                {
                    await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                    InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                }
            }
            else
            {
                AuftragsbezogeneXml = false;
                CanLoadSpeziData = false;
                CanSaveAllSpeziParameters = false;
                CheckOut = false;
                CanCheckOut = false;
                LikeEditParameter = true;
                SpezifikationName = string.Empty;
                await LoadDataAsync();
            }
        }
    }

    private async Task UploadDataAsync()
    {
        await Task.Delay(30);
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
                AuftragsbezogeneXml = false;
                CheckOut = false;
                LikeEditParameter = true;
                SpezifikationName = string.Empty;
                await LoadDataAsync();
            }
            else
            {
                await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                InfoSidebarPanelText += $"----------\n";
            }
        }
    }

    private async Task LoadDataAsync()
    {
        await SetFullPathXmlAsync(OpenReadOnly);

        var data = await _parameterDataService.LoadParameterAsync(FullPathXml);

        foreach (var item in data)
        {
            if (ParamterDictionary.ContainsKey(item.Name))
            {
                ParamterDictionary[item.Name] = item;
            }
            else
            {
                ParamterDictionary.Add(item.Name, item);
            }
        }
        if (_CurrentSpeziProperties is not null)
        {
        _CurrentSpeziProperties.ParamterDictionary = ParamterDictionary;
        }
        _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
        InfoSidebarPanelText += $"----------\n";
        LikeEditParameter = true;
        OpenReadOnly = true;
        _ = SetCalculatedValuesAsync();
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
    }

    private void SetInfoSidebarPanelText(PropertyChangedMessage<string> message)
    {
        if (!(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        var Sender = (Parameter)message.Sender;

        if (Sender.ParameterTyp == Parameter.ParameterTypValue.Date)
        {
            string? datetimeOld;
            string? datetimeNew;
            try
            {
                if (message.OldValue is not null)
                {
                    var excelDateOld = Convert.ToDouble(message.OldValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    datetimeOld = DateTime.FromOADate(excelDateOld).ToShortDateString();
                }
                else
                {
                    datetimeOld = string.Empty;
                }

                if (message.NewValue is not null)
                {
                    var excelDateNew = Convert.ToDouble(message.NewValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    datetimeNew = DateTime.FromOADate(excelDateNew).ToShortDateString();
                }
                else
                {
                    datetimeNew = string.Empty;
                }
                InfoSidebarPanelText += $"{message.PropertyName} : {datetimeOld} => {datetimeNew} geändert \n";
            }
            catch
            {
                InfoSidebarPanelText += $"{message.PropertyName} : {message.OldValue} => {message.NewValue} geändert \n";
            }
        }
        else
        {
            InfoSidebarPanelText += $"{message.PropertyName} : {message.OldValue} => {message.NewValue} geändert \n";
        }
    }

    private async Task SaveAllParameterAsync()
    {
        var infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
        InfoSidebarPanelText += infotext;
        await CheckUnsavedParametresAsync();
    }

    private void SetSettings()
    {
        _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        _CurrentSpeziProperties.Adminmode = Adminmode;
        _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
    }

    private async Task<string[]> SearchWorkspaceAsync(string searchPattern)
    {
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
        var areaPersonsRequestMessageResult = await WeakReferenceMessenger.Default.Send<AreaPersonsRequestMessageAsync>();

        if (areaPersonsRequestMessageResult is not null)
        {
            var person = areaPersonsRequestMessageResult.Personen;
            var nutzflaecheKabine = areaPersonsRequestMessageResult.NutzflaecheKabine;

            if (person > 0)
            {
                ParamterDictionary["var_Personen"].Value = Convert.ToString(person);
            }
            if (nutzflaecheKabine > 0)
            {
                ParamterDictionary["var_A_Kabine"].Value = Convert.ToString(nutzflaecheKabine);
            }
        }

        var carWeightRequestMessageMessageResult = await WeakReferenceMessenger.Default.Send<CarWeightRequestMessageAsync>();

        if (carWeightRequestMessageMessageResult is not null)
        {
            CarDoorWeight = carWeightRequestMessageMessageResult.KabinenTuerenGewicht;
            CarFrameWeight = carWeightRequestMessageMessageResult.FangrahmenGewicht;
            CarWeight = carWeightRequestMessageMessageResult.KabinenGewicht;
        }
        await Task.CompletedTask;
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        if (_CurrentSpeziProperties is null) { SetSettings(); }
        _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = _CurrentSpeziProperties.CheckOut;
        LikeEditParameter = _CurrentSpeziProperties.LikeEditParameter;
        SpezifikationStatusTyp = _CurrentSpeziProperties.SpezifikationStatusTyp;
        SearchInput = _CurrentSpeziProperties.SearchInput;
        InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
        if (_CurrentSpeziProperties.FullPathXml is not null)
        {
            FullPathXml = _CurrentSpeziProperties.FullPathXml;
            _SpezifikationName = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        }
        if (_CurrentSpeziProperties.ParamterDictionary is not null) { ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary; }
        if (ParamterDictionary.Values.Count == 0) { _ = LoadDataAsync(); }

        _ =  SetCalculatedValuesAsync();

        if (_CurrentSpeziProperties is not null &&
            _CurrentSpeziProperties.ParamterDictionary is not null &&
            _CurrentSpeziProperties.ParamterDictionary.Values is not null) { _ = CheckUnsavedParametresAsync(); }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}