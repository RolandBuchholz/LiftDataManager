using CommunityToolkit.Mvvm.Messaging.Messages;
namespace LiftDataManager.ViewModels;

public partial class AntriebSteuerungNotrufViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public AntriebSteuerungNotrufViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                           ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_Getriebe")
        {
            SetDriveTyp();
        };
        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    [ObservableProperty]
    public partial bool IsRopedrive { get; set; }

    private void SetDriveTyp()
    {
        IsRopedrive = string.IsNullOrWhiteSpace(ParameterDictionary["var_Getriebe"].Value) || ParameterDictionary["var_Getriebe"].Value != "hydraulisch";
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            SetDriveTyp();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
