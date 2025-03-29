namespace LiftDataManager.Views;

public sealed partial class ThemeSettingPage : Page
{
    public ThemeSettingViewModel ViewModel
    {
        get;
    }

    public ThemeSettingPage()
    {
        ViewModel = App.GetService<ThemeSettingViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
        Loaded += ThemeSettingPage_Loaded;
    }

    private void ThemeSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.SetDefaultTheme(CmbTheme);
        ViewModel.SetDefaultBackdrop(CmbBackdrop);
    }
}
