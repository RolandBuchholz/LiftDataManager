namespace LiftDataManager.Views;

public sealed partial class KabinengewichtPage : Page
{
    public KabinengewichtViewModel ViewModel
    {
        get;
    }

    public KabinengewichtPage()
    {
        ViewModel = App.GetService<KabinengewichtViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
