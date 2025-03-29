namespace LiftDataManager.Views;

public sealed partial class NutzlastberechnungPage : Page
{
    public NutzlastberechnungViewModel ViewModel
    {
        get;
    }

    public NutzlastberechnungPage()
    {
        ViewModel = App.GetService<NutzlastberechnungViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
