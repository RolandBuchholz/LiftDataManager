namespace LiftDataManager.ViewModels;

public class AntriebSteuerungNotrufViewModel : DataViewModelBase, INavigationAware
{
    public AntriebSteuerungNotrufViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
