using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public BausatzDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
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
