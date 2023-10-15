using CommunityToolkit.Mvvm.Messaging.Messages;
namespace LiftDataManager.ViewModels;

public partial class AntriebSteuerungNotrufViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public AntriebSteuerungNotrufViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
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
        IsRopedrive = string.IsNullOrWhiteSpace(ParameterDictionary!["var_Getriebe"].Value) || ParameterDictionary!["var_Getriebe"].Value != "hydraulisch";
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
            SetDriveTyp();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
