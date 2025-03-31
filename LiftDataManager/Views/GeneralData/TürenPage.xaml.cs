namespace LiftDataManager.Views;

public sealed partial class TürenPage : Page
{
    public TürenViewModel ViewModel
    {
        get;
    }

    public TürenPage()
    {
        ViewModel = App.GetService<TürenViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
