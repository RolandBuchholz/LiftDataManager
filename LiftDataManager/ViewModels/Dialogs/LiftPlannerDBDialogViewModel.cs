using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class LiftPlannerDBDialogViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ParameterEditContext _parameterEditContext;
    private readonly ILogger<LiftPlannerDBDialogViewModel> _logger;
    public LiftPlannerDBDialogViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ILogger<LiftPlannerDBDialogViewModel> logger, ParameterEditContext parameterEditContext) :
     base(parameterDataService, dialogService, infoCenterService)
    {
        _logger = logger;
        _parameterEditContext = parameterEditContext;
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
        if (_parameterEditContext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        _parameterEditContext.Database.CloseConnection();
    }
}