using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class SchachtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public SchachtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService) :
         base(parameterDataService, dialogService, infoCenterService, settingService)
    {
    }
    [RelayCommand]
    private static void GoToSchachtDetail()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(SchachtDetailPage));
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
