namespace LiftDataManager.Views;

public sealed partial class SafetyComponentsEquipmentsPage : Page
{
    public SafetyComponentsEquipmentsViewModel ViewModel
    {
        get;
    }

    public SafetyComponentsEquipmentsPage()
    {
        ViewModel = App.GetService<SafetyComponentsEquipmentsViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
