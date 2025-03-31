namespace LiftDataManager.Views;

public sealed partial class KabineDetailLayoutPage : Page
{
    public KabineDetailLayoutViewModel ViewModel
    {
        get;
    }
    public KabineDetailLayoutPage()
    {
        ViewModel = App.GetService<KabineDetailLayoutViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
