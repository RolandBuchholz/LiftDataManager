using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class TürenViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public TürenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                          ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_SchachttuereBestand")
        {
            if (!string.IsNullOrWhiteSpace(message.NewValue))
            {
                ShowShaftDoorDetails = !Convert.ToBoolean(message.NewValue);
            }
        }
        
        if (message.PropertyName == "var_KabinentuereBestand")
        {
            if (!string.IsNullOrWhiteSpace(message.NewValue))
            {
                ShowCarDoorDetails = !Convert.ToBoolean(message.NewValue);
            }
        }

        if (message.PropertyName == "var_AdvancedDoorSelection")
        {
            if (!string.IsNullOrWhiteSpace(message.NewValue))
            {
                ShowAdvancedDoorSelection = Convert.ToBoolean(message.NewValue);
            }
        }

        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    [ObservableProperty]
    public partial bool ShowCarDoorDataB { get; set; }

    [ObservableProperty]
    public partial bool ShowCarDoorDataC { get; set; }

    [ObservableProperty]
    public partial bool ShowCarDoorDataD { get; set; }

    [ObservableProperty]
    public partial bool ShowShaftDoorDetails { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowCarDoorDetails { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowAdvancedDoorSelection { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetVariableCarDoorDataCommand))]
    public partial bool CanSetVariableCarDoorData { get; set; }

    [RelayCommand(CanExecute = nameof(CanSetVariableCarDoorData))]
    private async Task SetVariableCarDoorDataAsync()
    {
        bool currentVariableCarDoorData = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Variable_Tuerdaten");
        ParameterDictionary["var_Variable_Tuerdaten"].Value = LiftParameterHelper.FirstCharToUpperAsSpan((!currentVariableCarDoorData).ToString());
        SetCarDoorDataVisibility();
        await Task.CompletedTask;
    }

    private void SetCarDoorDataVisibility()
    {
        bool variableCarDoorData = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Variable_Tuerdaten");
        ShowAdvancedDoorSelection = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_AdvancedDoorSelection");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");

        CanSetVariableCarDoorData = zugangB || zugangC || zugangD;
        ShowCarDoorDataB = variableCarDoorData && zugangB;
        ShowCarDoorDataC = variableCarDoorData && zugangC;
        ShowCarDoorDataD = variableCarDoorData && zugangD;

        ShowShaftDoorDetails = !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SchachttuereBestand");
        ShowCarDoorDetails = !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_KabinentuereBestand");
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        SetCarDoorDataVisibility();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}