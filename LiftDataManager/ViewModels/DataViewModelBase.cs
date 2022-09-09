using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class DataViewModelBase : ObservableRecipient
{
    public readonly IParameterDataService? _parameterDataService;
    public readonly IDialogService? _dialogService;
    public readonly INavigationService? _navigationService;

    public bool Adminmode {get; set;}
    public bool CheckoutDialogIsOpen {get; set;}

    public CurrentSpeziProperties? CurrentSpeziProperties;
    public ObservableDictionary<string, Parameter>? ParamterDictionary {get; set;}
    public ObservableDictionary<string, List<ParameterStateInfo>>? ParamterErrorDictionary { get; set; }

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
            if (message is not null)
            {
                SetInfoSidebarPanelText(message);
            //TODO Make Async
                _ = SetModelStateAsync();
            }
    }

    [ObservableProperty]
    private string? fullPathXml;

    [ObservableProperty]
    private bool auftragsbezogeneXml;

    [ObservableProperty]
    private bool checkOut;

    [ObservableProperty]
    private bool likeEditParameter;

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private string? infoSidebarPanelText;
    partial void OnInfoSidebarPanelTextChanged(string? value)
    {
        if (CurrentSpeziProperties is not null)
            CurrentSpeziProperties.InfoSidebarPanelText = value;
        Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAllParameterCommand))]
    private bool canSaveAllSpeziParameters;

    [RelayCommand(CanExecute = nameof(CanSaveAllSpeziParameters))]
    public async Task SaveAllParameterAsync()
    {
        var infotext = await _parameterDataService!.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
        InfoSidebarPanelText += infotext;
        await SetModelStateAsync();
    }

    protected virtual void SynchronizeViewModelParameter()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.FullPathXml is not null)
        { FullPathXml = CurrentSpeziProperties.FullPathXml; }
        if (CurrentSpeziProperties.ParamterDictionary is not null)
        { ParamterDictionary = CurrentSpeziProperties.ParamterDictionary; }
        Adminmode = CurrentSpeziProperties.Adminmode;
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
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
                    ParamterErrorDictionary.Add(error.Name, error.parameterErrors[error.Name]);
                }
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParamterDictionary!.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty && Adminmode;
            }
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(App.MainRoot!,
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei aus zu checken?",
                                    "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                if (dialogResult)
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
                    }
                    _ = Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
                }
            }
        }
        await Task.CompletedTask;
    }

    protected void SetInfoSidebarPanelText(PropertyChangedMessage<string> message)
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
}