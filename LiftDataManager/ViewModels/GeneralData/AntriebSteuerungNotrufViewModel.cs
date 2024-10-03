using CommunityToolkit.Mvvm.Messaging.Messages;
namespace LiftDataManager.ViewModels;

public partial class AntriebSteuerungNotrufViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public AntriebSteuerungNotrufViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService) :
         base(parameterDataService, dialogService, infoCenterService)
    {
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_Getriebe")
        {
            SetDriveTyp();
        };
        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    private bool isRopedrive;

    private void SetDriveTyp()
    {
        IsRopedrive = string.IsNullOrWhiteSpace(ParameterDictionary["var_Getriebe"].Value) || ParameterDictionary["var_Getriebe"].Value != "hydraulisch";
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            SetDriveTyp();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
