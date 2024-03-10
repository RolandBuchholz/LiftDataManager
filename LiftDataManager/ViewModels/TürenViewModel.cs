using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class TürenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public TürenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {

    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

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
    private bool showCarDoorDataB;

    [ObservableProperty]
    private bool showCarDoorDataC;

    [ObservableProperty]
    private bool showCarDoorDataD;

    [ObservableProperty]
    private bool showShaftDoorDetails = true;

    [ObservableProperty]
    private bool showCarDoorDetails = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetVariableCarDoorDataCommand))]
    private bool canSetVariableCarDoorData;

    [RelayCommand(CanExecute = nameof(CanSetVariableCarDoorData))]
    private async Task SetVariableCarDoorDataAsync()
    {
        var currentVariableCarDoorData = Convert.ToBoolean(ParameterDictionary!["var_Variable_Tuerdaten"].Value);
        ParameterDictionary["var_Variable_Tuerdaten"].Value = (currentVariableCarDoorData) ? "False" : "True";
        SetCarDoorDataVisibility();
        await Task.CompletedTask;
    }

    private void SetCarDoorDataVisibility()
    {
        var variableCarDoorData = Convert.ToBoolean(ParameterDictionary!["var_Variable_Tuerdaten"].Value);
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
