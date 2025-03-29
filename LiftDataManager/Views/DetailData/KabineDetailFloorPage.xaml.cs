namespace LiftDataManager.Views;

public sealed partial class KabineDetailFloorPage : Page
{
    public KabineDetailFloorViewModel ViewModel
    {
        get;
    }
    public KabineDetailFloorPage()
    {
        ViewModel = App.GetService<KabineDetailFloorViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
