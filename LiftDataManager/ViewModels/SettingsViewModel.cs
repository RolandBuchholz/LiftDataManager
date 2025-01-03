namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    public IJsonNavigationViewService JsonNavigationViewService { get; }
    public SettingsViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
    }
}