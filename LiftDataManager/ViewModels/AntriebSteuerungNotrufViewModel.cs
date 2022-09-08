using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class AntriebSteuerungNotrufViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public AntriebSteuerungNotrufViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null && CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
