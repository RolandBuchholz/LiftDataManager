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
                                       .ConfigureTitleBar(AppTitleBar)
                                       .ConfigureBreadcrumbBar(JsonBreadCrumbNavigator, BreadcrumbPageMappings.PageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions.BreadcrumbNavigatorOnly);
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeService.SetElementThemeWithoutSaveAsync();
    }
}
