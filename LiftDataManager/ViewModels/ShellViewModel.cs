using Microsoft.UI.Xaml.Navigation;

namespace LiftDataManager.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IRecipient<SpeziPropertiesChangedMessage>, IRecipient<SpeziPropertiesRequestMessage>
{
    private CurrentSpeziProperties CurrentSpeziProperties = new();

    public IJsonNavigationViewService JsonNavigationViewService { get; }

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private bool showGlobalSearch = true;

    [ObservableProperty]
    private string? globalSearchInput;

    [ObservableProperty]
    private object? selected;

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
        IsBackEnabled = JsonNavigationViewService.CanGoBack;
        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = JsonNavigationViewService.SettingsItem;
            return;
        }
        var menuItems = JsonNavigationViewService.MenuItems;
        if (menuItems != null)
        {
            var selectedItem = JsonNavigationViewService.GetSelectedItem(menuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
                ShowGlobalSearch = selectedItem.Tag != null;
            }
        }
    }
}