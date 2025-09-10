namespace LiftDataManager.Views;

public sealed partial class StructureLoadsPage : Page
{
    public StructureLoadsViewModel ViewModel
    {
        get;
    }

    public StructureLoadsPage()
    {
        ViewModel = App.GetService<StructureLoadsViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
