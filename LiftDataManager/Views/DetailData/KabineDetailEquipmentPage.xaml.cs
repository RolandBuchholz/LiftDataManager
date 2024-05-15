namespace LiftDataManager.Views;

public sealed partial class KabineDetailEquipmentPage : Page
{
    public KabineDetailEquipmentViewModel ViewModel
    {
        get;
    }

    public KabineDetailEquipmentPage()
    {
        ViewModel = App.GetService<KabineDetailEquipmentViewModel>();
        InitializeComponent();
    }
}
