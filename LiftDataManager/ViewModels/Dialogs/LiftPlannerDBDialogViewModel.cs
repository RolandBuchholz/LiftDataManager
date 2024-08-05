using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class LiftPlannerDBDialogViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public LiftPlannerDBDialogViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService) :
     base(parameterDataService, dialogService, infoCenterService)
    {
    }

    [ObservableProperty]
    private string? company = "DDDDDDDDDDDDDD";

    public void OnNavigatedTo(object parameter)
    {
        //NavigatedToBaseActions();
    }

    public void OnNavigatedFrom()
    {
        //NavigatedFromBaseActions();
    }
}