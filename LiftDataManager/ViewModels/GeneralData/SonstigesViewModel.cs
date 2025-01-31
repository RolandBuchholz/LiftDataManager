using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public class SonstigesViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public SonstigesViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                              ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
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
