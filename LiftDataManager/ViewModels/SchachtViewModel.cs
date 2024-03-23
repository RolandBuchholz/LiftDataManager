using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class SchachtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public SchachtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {
    }
    [RelayCommand]
    private void GoToSchachtDetail() => _navigationService.NavigateTo("LiftDataManager.ViewModels.SchachtDetailViewModel");
    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
