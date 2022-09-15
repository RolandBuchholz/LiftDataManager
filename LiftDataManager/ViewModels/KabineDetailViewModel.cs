using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    [RelayCommand]
    private void GoToKabine() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineViewModel");

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
