namespace LiftDataManager.Views;

public sealed partial class KabineDetailPage : Page
{
    public KabineDetailViewModel ViewModel
    {
        get;
    }

    public KabineDetailPage()
    {
        ViewModel = App.GetService<KabineDetailViewModel>();
        InitializeComponent();
    }
}
