namespace LiftDataManager.ViewModels;

public class KabineDetailViewModel : DataViewModelBase, INavigationAware
{
    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
        {
            if (m is not null && m.Value.IsDirty)
            {
                SetInfoSidebarPanelText(m);
                await CheckUnsavedParametresAsync();
            }
        });
        GoToKabineCommand = new RelayCommand(GoToKabine);
    }

    public IRelayCommand GoToKabineCommand
    {
        get;
    }

    private void GoToKabine() => _navigationService.NavigateTo("LiftDataManager.ViewModels.KabineViewModel");

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties is not null && _CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
