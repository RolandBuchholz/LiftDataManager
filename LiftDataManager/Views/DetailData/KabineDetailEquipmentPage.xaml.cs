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
        Loaded += KabineDetailEquipmentPage_Loaded;
        Unloaded += KabineDetailEquipmentPage_Unloaded;
    }

    private void KabineDetailEquipmentPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CarViewChanged += ViewModel_CarViewChanged;
    }

    private void KabineDetailEquipmentPage_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CarViewChanged -= ViewModel_CarViewChanged;
    }

    private void ViewModel_CarViewChanged()
    {
        CarEquipmentControlA.RefreshView();
        CarEquipmentControlB.RefreshView();
        CarEquipmentControlC.RefreshView();
        CarEquipmentControlD.RefreshView();
    }
}
