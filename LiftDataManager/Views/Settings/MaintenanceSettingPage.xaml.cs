namespace LiftDataManager.Views;

public sealed partial class MaintenanceSettingPage : Page
{
    public MaintenanceSettingViewModel ViewModel
    {
        get;
    }

    public MaintenanceSettingPage()
    {
        ViewModel = App.GetService<MaintenanceSettingViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
