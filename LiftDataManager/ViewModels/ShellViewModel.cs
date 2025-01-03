using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using RtfPipe.Tokens;

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
    public partial string? HeaderText { get; set; }

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

        var view = ((Frame)sender).FindParent<NavigationView>();
        var breadcrumbnavigator = ((Grid?)(view?.Header))?.FindChild<BreadcrumbNavigator>();
        var headerTextBlock = ((Grid?)(view?.Header))?.FindChild<TextBlock>();

        if (view is null ||
            breadcrumbnavigator is null ||
            headerTextBlock is null)
        {
            return;
        }

        if (BreadcrumbPageMappings.PageDictionary.ContainsKey(e.SourcePageType))
        {
            view.AlwaysShowHeader = true;
            ShowGlobalSearch = false;
            breadcrumbnavigator.Visibility = Visibility.Visible;
            headerTextBlock.Visibility = Visibility.Collapsed;
        }
        else
        {
            breadcrumbnavigator.Visibility = Visibility.Collapsed;
            headerTextBlock.Visibility = Visibility.Visible;
            var page = e.Parameter as DataItem;
            HeaderText = page?.Description;
            ShowGlobalSearch = page is null || !page.HideItem;
            view.AlwaysShowHeader = !string.IsNullOrWhiteSpace(page?.Description);
        }
    }
}