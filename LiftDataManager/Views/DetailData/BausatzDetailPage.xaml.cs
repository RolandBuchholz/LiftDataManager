namespace LiftDataManager.Views;

public sealed partial class BausatzDetailPage : Page
{
    public BausatzDetailViewModel ViewModel
    {
        get;
    }

    public BausatzDetailPage()
    {
        ViewModel = App.GetService<BausatzDetailViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
