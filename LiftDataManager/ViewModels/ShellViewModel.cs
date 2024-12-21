using Microsoft.UI.Xaml.Navigation;

namespace LiftDataManager.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IRecipient<SpeziPropertiesChangedMessage>, IRecipient<SpeziPropertiesRequestMessage>
{
    private CurrentSpeziProperties CurrentSpeziProperties = new();

    public IJsonNavigationViewService JsonNavigationViewService { get; }

    [ObservableProperty]
    public partial bool IsBackEnabled { get; set; }

    [ObservableProperty]
    public partial bool ShowGlobalSearch { get; set; } = true;

    [ObservableProperty]
    public partial string? GlobalSearchInput { get; set; }

    [ObservableProperty]
    public partial string? Header { get; set; }

    public ShellViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        JsonNavigationViewService.Navigated += OnNavigated;
        IsActive = true;
    }

    public void Receive(SpeziPropertiesRequestMessage message)
    {
        message.Reply(CurrentSpeziProperties);
    }

    public void Receive(SpeziPropertiesChangedMessage message)
    {
        CurrentSpeziProperties = message.Value;
    }

    [RelayCommand]
    private void StartGlobalSearch()
    {
        var searchInput = GlobalSearchInput;
        GlobalSearchInput = string.Empty;
        JsonNavigationViewService.NavigateTo(typeof(ListenansichtPage), searchInput);
    }

    [RelayCommand]
    private void GoToHelpViewModel() => JsonNavigationViewService.NavigateTo(typeof(HelpPage));

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        //workaround navigationview dispose
        GC.Collect();

        IsBackEnabled = JsonNavigationViewService.CanGoBack;
        if (e.SourcePageType == typeof(SettingsPage))
        {
            Header = "Einstellungen";
            ShowGlobalSearch = false;
        }
        var viewPage = JsonNavigationViewService.DataSource.GetItem(e.SourcePageType.FullName);
        if (viewPage != null)
        {
            Header = viewPage.Description;
            ShowGlobalSearch = !viewPage.HideItem;
        }
    }
}