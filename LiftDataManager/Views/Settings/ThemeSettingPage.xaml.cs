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
    }
}
