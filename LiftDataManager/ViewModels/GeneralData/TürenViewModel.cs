using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class TürenViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public TürenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService) :
         base(parameterDataService, dialogService, infoCenterService, settingService)
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
                ShowShaftDoorDetails = !Convert.ToBoolean(message.NewValue);
        };

        if (message.PropertyName == "var_KabinentuereBestand")
        {
            if (!string.IsNullOrWhiteSpace(message.NewValue))
                ShowCarDoorDetails = !Convert.ToBoolean(message.NewValue);
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
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
    [NotifyCanExecuteChangedFor(nameof(SetVariableCarDoorDataCommand))]
    public partial bool CanSetVariableCarDoorData { get; set; }

    [RelayCommand(CanExecute = nameof(CanSetVariableCarDoorData))]
    private async Task SetVariableCarDoorDataAsync()
    {
        var currentVariableCarDoorData = Convert.ToBoolean(ParameterDictionary["var_Variable_Tuerdaten"].Value);
        ParameterDictionary["var_Variable_Tuerdaten"].Value = (currentVariableCarDoorData) ? "False" : "True";
        SetCarDoorDataVisibility();
        await Task.CompletedTask;
    }

    private void SetCarDoorDataVisibility()
    {
        var variableCarDoorData = Convert.ToBoolean(ParameterDictionary["var_Variable_Tuerdaten"].Value);
        var zugangB = Convert.ToBoolean(ParameterDictionary["var_ZUGANSSTELLEN_B"].Value);
        var zugangC = Convert.ToBoolean(ParameterDictionary["var_ZUGANSSTELLEN_C"].Value);
        var zugangD = Convert.ToBoolean(ParameterDictionary["var_ZUGANSSTELLEN_D"].Value);

        CanSetVariableCarDoorData = (zugangB || zugangC || zugangD);
        ShowCarDoorDataB = (variableCarDoorData && zugangB);
        ShowCarDoorDataC = (variableCarDoorData && zugangC);
        ShowCarDoorDataD = (variableCarDoorData && zugangD);

        ShowShaftDoorDetails = !Convert.ToBoolean(ParameterDictionary["var_SchachttuereBestand"].Value);
        ShowCarDoorDetails = !Convert.ToBoolean(ParameterDictionary["var_KabinentuereBestand"].Value);
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
