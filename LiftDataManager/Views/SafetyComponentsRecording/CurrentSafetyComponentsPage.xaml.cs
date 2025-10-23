namespace LiftDataManager.Views;

public sealed partial class CurrentSafetyComponentsPage : Page
{
    public CurrentSafetyComponentsViewModel ViewModel
    {
        get;
    }

    public CurrentSafetyComponentsPage()
    {
        ViewModel = App.GetService<CurrentSafetyComponentsViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
