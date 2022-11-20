using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.DataAccessLayer;

namespace LiftDataManager.ViewModels;

public class ErrorViewModel : DataViewModelBase, INavigationAware
{
    public ErrorViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    public void OnNavigatedFrom()
    {
        App.Current.Exit();
    }
    public void OnNavigatedTo(object parameter)
    {
    }
}
