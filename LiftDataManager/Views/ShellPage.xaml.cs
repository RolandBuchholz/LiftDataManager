using Microsoft.EntityFrameworkCore.Metadata;

namespace LiftDataManager.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.JsonNavigationService.Initialize(NavigationViewControl, NavigationFrame, NavigationPageMappings.PageDictionary)
                                       .ConfigureJsonFile("Assets/NavViewMenu/NavigationViewControlData.json")
                                       .ConfigureDefaultPage(typeof(HomePage))
                                       .ConfigureSettingsPage(typeof(SettingsPage))
                                       .ConfigureBreadcrumbBar(JsonBreadCrumbNavigator, BreadcrumbPageMappings.PageDictionary,BreadcrumbNavigatorHeaderVisibilityOptions.BreadcrumbNavigatorOnly);
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.ChangeThemeWithoutSave(App.MainWindow);
    }

    private void TitleBar_PaneToggleRequested(WinUIEx.TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }

    private void TitleBar_BackRequested(WinUIEx.TitleBar sender, object args)
    {
        if (ViewModel.JsonNavigationService.CanGoBack)
        {
            ViewModel.JsonNavigationService.GoBack();
        }
    }
}
