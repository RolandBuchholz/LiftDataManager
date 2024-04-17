
namespace LiftDataManager.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        Loaded += SettingsPage_Loaded;
    }

    private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel._themeService.SetThemeComboBoxDefaultItem(CmbTheme);
        ViewModel._themeService.SetBackdropComboBoxDefaultItem(CmbBackdrop);
    }
}
