using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class HomeViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly IVaultDataService _vaultDataService;
    private readonly ISettingService _settingService;
    private readonly IValidationParameterDataService _validationParameterDataService;
    private bool OpenReadOnly { get; set; } = true;

    public HomeViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, 
                         ISettingService settingsSelectorService, IVaultDataService vaultDataService, IValidationParameterDataService validationParameterDataService)
        : base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        _vaultDataService = vaultDataService;
        _validationParameterDataService = validationParameterDataService;

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
    private string? spezifikationStatusTyp;
    partial void OnSpezifikationStatusTypChanged(string? value)
    {
        SpezifikationName = string.Empty;
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
        if (FullPathXml is null) return;
        var data = await _parameterDataService!.LoadParameterAsync(FullPathXml);

        foreach (var item in data)
        {
            if (ParamterDictionary!.ContainsKey(item.Name))
            {
                var updatedParameter = ParamterDictionary[item.Name];
                updatedParameter.DataImport = true;
                updatedParameter.Value = item.Value;
                updatedParameter.Comment = item.Comment;
                updatedParameter.IsKey = item.IsKey;
                if (updatedParameter.Value is not null && updatedParameter.ParameterTyp == ParameterBase.ParameterTypValue.DropDownList)
                {
                    updatedParameter.DropDownListValue = updatedParameter.Value;
                }
                updatedParameter.DataImport = false;
            }
            else
            {
                InfoSidebarPanelText += $"----------\n";
                InfoSidebarPanelText += $"Parameter {item.Name} wird nicht unterstützt\n";
                InfoSidebarPanelText += $"Überprüfen Sie die AutodeskTransfer.XML Datei\n";
                InfoSidebarPanelText += $"----------\n";
            }
        }
        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.ParamterDictionary = ParamterDictionary;
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
        InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
        InfoSidebarPanelText += $"----------\n";
        LikeEditParameter = true;
        OpenReadOnly = true;
        CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        await SetCalculatedValuesAsync();
        await ValidateAllParameterAsync();
    }

    [RelayCommand(CanExecute = nameof(CanCheckOut))]
    private async Task CheckOutAsync()
    {
        OpenReadOnly = false;
        await LoadDataAsync();
    }

    [RelayCommand(CanExecute = nameof(CanClearData))]
    private async Task ClearDataAsync()
    {
        if (FullPathXml is null) return;
        var delete = true;

        if (CanSaveAllSpeziParameters || CheckOut)
        {
            delete = await _dialogService!.WarningDialogAsync(App.MainRoot!,
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
                    AuftragsbezogeneXml = false;
                    CanValidateAllParameter = false;
                    SpezifikationName = string.Empty;
                    CanLoadSpeziData = false;
                    CanSaveAllSpeziParameters = false;
                    CheckOut = false;
                    LikeEditParameter = true;
                    await LoadDataAsync();
                }
                else
                {
                    await _dialogService!.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                    InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                }
            }
            else
            {
                AuftragsbezogeneXml = false;
                CanValidateAllParameter = false;
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

    [RelayCommand(CanExecute = nameof(CanUpLoadSpeziData))]
    private async Task UploadDataAsync()
    {
        await Task.Delay(30);
        if (SpezifikationName is null) return;
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
                CanValidateAllParameter = false;
                CheckOut = false;
                LikeEditParameter = true;
                SpezifikationName = string.Empty;
                await LoadDataAsync();
            }
            else
            {
                await _dialogService!.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                InfoSidebarPanelText += $"----------\n";
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanValidateAllParameter))]
    private async Task ValidateAllParameterAsync()
    {
        _ = _validationParameterDataService.ValidateAllParameterAsync();
        if (!CheckoutDialogIsOpen)
        {
            _ = _dialogService!.MessageDialogAsync(App.MainRoot!, "Validation Result", $"Es wurden {ParamterDictionary!.Count} Parameter überprüft.\n" +
                                                                                       $"Es wurden {ParamterErrorDictionary!.Count} Fehler/Warnungen/Informationen gefunden");
        }
        await SetModelStateAsync();
    }

    public async Task InitializeParametereAsync()
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
        await SetCalculatedValuesAsync();
        await SetModelStateAsync();
    }

    protected override async Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            HasErrors = ParamterDictionary!.Values.Any(p => p.HasErrors);
            ParamterErrorDictionary ??= new();
            ParamterErrorDictionary.Clear();
            if (HasErrors)
            {
                var errors = ParamterDictionary.Values.Where(e => e.HasErrors);
                foreach (var error in errors)
                {
                    if (!ParamterErrorDictionary.ContainsKey(error.Name))
                    {
                        var errorList = new List<ParameterStateInfo>();
                        errorList.AddRange(error.parameterErrors["Value"].ToList());
                        ParamterErrorDictionary.Add(error.Name, errorList);
                    }
                    else
                    {
                        ParamterErrorDictionary[error.Name].AddRange(error.parameterErrors["Value"].ToList());
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
                CanClearData = CheckOut;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(App.MainRoot!,
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
                            await _dialogService!.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
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
                            InfoSidebarPanelText += $"Die Daten {searchPattern} wurden aus dem Arbeitsberech geladen\n";
                            AuftragsbezogeneXml = true;
                            CanValidateAllParameter = true;
                            CheckOut = true;
                        }
                        else
                        {
                            var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError &&  !string.Equals(downloadResult.CheckOutState,"CheckedOutByOtherUser"))
                            {
                                FullPathXml = downloadResult.FullFileName;
                                InfoSidebarPanelText += $"{FullPathXml!.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                                CanValidateAllParameter = true;
                                CheckOut = downloadResult.IsCheckOut;
                            }
                            else if (string.Equals(downloadResult.CheckOutState, "CheckedOutByOtherUser"))
                            {
                                await _dialogService!.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
                                FullPathXml = downloadResult.FullFileName;
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
                                await _dialogService!.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
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
                        InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                        InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";

                        var confirmed = await _dialogService!.ConfirmationDialogAsync(App.MainRoot!,
                                                $"Es wurden mehrere {searchPattern} Dateien gefunden?",
                                                 "XML aus Vault herunterladen",
                                                    "Abbrechen");
                        if (confirmed)
                        {
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
                                await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot!, downloadResult);
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
                ParamterDictionary!["var_Personen"].Value = Convert.ToString(person);
            }
            if (nutzflaecheKabine > 0)
            {
                ParamterDictionary!["var_A_Kabine"].Value = Convert.ToString(nutzflaecheKabine);
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
        if (CurrentSpeziProperties is null) SetSettings();
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        canValidateAllParameter = AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
        SpezifikationStatusTyp = (CurrentSpeziProperties.SpezifikationStatusTyp is not null) ? CurrentSpeziProperties.SpezifikationStatusTyp : "Auftrag";
        InfoSidebarPanelText = CurrentSpeziProperties.InfoSidebarPanelText;
        if (CurrentSpeziProperties.FullPathXml is not null)
        {
            FullPathXml = CurrentSpeziProperties.FullPathXml;
            SpezifikationName = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
        }
        ParamterDictionary ??= new();
        if (CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;
        if (ParamterDictionary.Values.Count == 0) _ = InitializeParametereAsync();
        _ = SetCalculatedValuesAsync();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();    
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}