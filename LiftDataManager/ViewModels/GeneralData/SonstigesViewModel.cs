using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class SonstigesViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public SonstigesViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService) :
         base(parameterDataService, dialogService, infoCenterService, settingService)
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
