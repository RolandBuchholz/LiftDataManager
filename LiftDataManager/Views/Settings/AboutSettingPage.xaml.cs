namespace LiftDataManager.Views;

public sealed partial class AboutSettingPage : Page
{
    public AboutSettingViewModel ViewModel
    {
        get;
    }

    public AboutSettingPage()
    {
        ViewModel = App.GetService<AboutSettingViewModel>();
        InitializeComponent();
    }
}
