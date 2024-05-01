using Microsoft.UI.Xaml.Navigation;

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
        InitializeComponent();
        AppTitleBar.Window = App.MainWindow;
        ViewModel.JsonNavigationViewService.Initialize(NavigationViewControl, NavigationFrame);
        ViewModel.JsonNavigationViewService.ConfigJson("Assets/NavViewMenu/NavigationViewControlData.json");
    }

    private void AppTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (NavigationFrame.CanGoBack)
        {
            NavigationFrame.GoBack();
        }
    }
    private void AppTitleBar_PaneButtonClick(object sender, RoutedEventArgs e)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }
    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow.Content is not FrameworkElement element)
            return;

        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }
}
    