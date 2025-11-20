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

    private async void Edit_Equipment_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not null && sender.GetType() == typeof(Button))
        {
            await ViewModel.EditEquipmentAsync(((Button)sender).CommandParameter);
        }
    }
}
