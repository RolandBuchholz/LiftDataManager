namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    public IJsonNavigationService JsonNavigationViewService { get; }
    public SettingsViewModel(IJsonNavigationService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
    }
}