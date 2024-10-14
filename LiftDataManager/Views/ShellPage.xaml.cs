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
        ViewModel.JsonNavigationViewService.Initialize(NavigationViewControl, NavigationFrame);
        ViewModel.JsonNavigationViewService.ConfigJson("Assets/NavViewMenu/NavigationViewControlData.json");
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow.Content is not FrameworkElement element)
        {
            return;
        }
        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }

    private void TitleBar_PaneToggleRequested(WinUIEx.TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }

    private void TitleBar_BackRequested(WinUIEx.TitleBar sender, object args)
    {
        if (NavigationFrame.CanGoBack)
        {
            NavigationFrame.GoBack();
        }
    }
}
