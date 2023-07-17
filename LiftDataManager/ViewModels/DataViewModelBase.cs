using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class DataViewModelBase : ObservableRecipient
{
    public readonly IParameterDataService? _parameterDataService;
    public readonly IDialogService? _dialogService;
    public readonly INavigationService? _navigationService;

    public bool Adminmode { get; set; }
    public bool CheckoutDialogIsOpen { get; set; }
    public string SpezifikationsNumber => !string.IsNullOrWhiteSpace(FullPathXml) ? Path.GetFileNameWithoutExtension(FullPathXml!).Replace("-AutoDeskTransfer", "") : string.Empty;

    public DispatcherTimer? AutoSaveTimer { get; set; }

    public CurrentSpeziProperties? CurrentSpeziProperties;
    public ObservableDictionary<string, Parameter>? ParamterDictionary { get; set; }
    public ObservableDictionary<string, List<ParameterStateInfo>>? ParamterErrorDictionary { get; set; } = new();

    public DataViewModelBase()
    {
    }

    public DataViewModelBase(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService)
    {
        _parameterDataService = parameterDataService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    public virtual void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();

    }

    public virtual void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        SetInfoSidebarPanelHighlightText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private bool hideInfoErrors;
    partial void OnHideInfoErrorsChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.HideInfoErrors, value))
        {
            CurrentSpeziProperties.HideInfoErrors = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private string? fullPathXml;
    partial void OnFullPathXmlChanged(string? value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<string>.Default.Equals(CurrentSpeziProperties.FullPathXml, value))
        {
            CurrentSpeziProperties.FullPathXml = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool auftragsbezogeneXml;
    partial void OnAuftragsbezogeneXmlChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.AuftragsbezogeneXml, value))
        {
            CurrentSpeziProperties.AuftragsbezogeneXml = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool checkOut;
    partial void OnCheckOutChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.CheckOut, value))
        {
            CurrentSpeziProperties.CheckOut = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool likeEditParameter;
    partial void OnLikeEditParameterChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.LikeEditParameter, value))
        {
            CurrentSpeziProperties.LikeEditParameter = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private string? infoSidebarPanelText;
    partial void OnInfoSidebarPanelTextChanged(string? value)
    {
        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.InfoSidebarPanelText = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAllParameterCommand))]
    private bool canSaveAllSpeziParameters;

    [RelayCommand(CanExecute = nameof(CanSaveAllSpeziParameters))]
    public async Task SaveAllParameterAsync()
    {
        if (ParamterDictionary is null)
            return;
        if (FullPathXml is null)
            return;
        var infotext = await _parameterDataService!.SaveAllParameterAsync(ParamterDictionary, FullPathXml, Adminmode);
        InfoSidebarPanelText += infotext;
        await SetModelStateAsync();
        if (AutoSaveTimer is not null)
        {
            var saveTimeIntervall = AutoSaveTimer.Interval;
            AutoSaveTimer.Stop();
            AutoSaveTimer.Interval = saveTimeIntervall;
            AutoSaveTimer.Start();
        }
    }

    protected virtual void SynchronizeViewModelParameter()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.FullPathXml is not null)
            FullPathXml = CurrentSpeziProperties.FullPathXml;
        if (CurrentSpeziProperties.ParamterDictionary is not null)
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;
        Adminmode = CurrentSpeziProperties.Adminmode;
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        HideInfoErrors = CurrentSpeziProperties.HideInfoErrors;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
        InfoSidebarPanelText = CurrentSpeziProperties.InfoSidebarPanelText;
    }

    protected async virtual Task SetModelStateAsync()
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
            }
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei auszuchecken?",
                                    "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                if ((bool)dialogResult)
                {
                    CheckoutDialogIsOpen = false;
                    _navigationService!.NavigateTo("LiftDataManager.ViewModels.HomeViewModel");
                }
                else
                {
                    CheckoutDialogIsOpen = false;
                    LikeEditParameter = false;
                    if (CurrentSpeziProperties is not null)
                    {
                        CurrentSpeziProperties.LikeEditParameter = LikeEditParameter;
                        _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
                    }
                }
            }
        }
        await Task.CompletedTask;
    }

    protected void SetInfoSidebarPanelText(PropertyChangedMessage<string> message)
    {
        var sender = (Parameter)message.Sender;

        if (sender.ParameterTyp == ParameterBase.ParameterTypValue.Date)
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

    protected void SetInfoSidebarPanelHighlightText(PropertyChangedMessage<bool> message)
    {
        var sender = (Parameter)message.Sender;

        if (message.NewValue)
        {
            InfoSidebarPanelText += $"|{sender.DisplayName}| Markierung hinzugefügt\n";
        }
        else
        {
            InfoSidebarPanelText += $"|{sender.DisplayName}| Markierung entfernt\n";
        }
    }

    public CurrentSpeziProperties GetCurrentSpeziProperties()
    {
        return Messenger.Send<SpeziPropertiesRequestMessage>();
    }
}