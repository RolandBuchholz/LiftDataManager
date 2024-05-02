namespace LiftDataManager.Views;

public sealed partial class MaintenancePage : Page
{
    public MaintenanceViewModel ViewModel
    {
        get;
    }

    public MaintenancePage()
    {
        ViewModel = App.GetService<MaintenanceViewModel>();
        InitializeComponent();
    }
}
