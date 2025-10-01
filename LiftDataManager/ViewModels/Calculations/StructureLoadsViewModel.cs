using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class StructureLoadsViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public StructureLoadsViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                              ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
        {
            return;
        }
        if (!(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_v")
        {
            CalculateJumpHeight();
        }

        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    [ObservableProperty]
    public partial double JumpHeight { get; set; }

    private void CalculateJumpHeight()
    {
        double speed = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_v");
        JumpHeight = Math.Ceiling(speed * speed * 0.35 * 100) / 100;
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CalculateJumpHeight();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
