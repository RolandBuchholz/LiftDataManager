namespace LiftDataManager.Views;

public sealed partial class KabinenLüftungPage : Page
{
    public KabinenLüftungViewModel ViewModel
    {
        get;
    }

    public KabinenLüftungPage()
    {
        ViewModel = App.GetService<KabinenLüftungViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
