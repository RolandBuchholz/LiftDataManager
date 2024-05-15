namespace LiftDataManager.Views;

public sealed partial class KabineDetailCeilingPage : Page
{
    public KabineDetailCeilingViewModel ViewModel
    {
        get;
    }
    public KabineDetailCeilingPage()
    {
        ViewModel = App.GetService<KabineDetailCeilingViewModel>();
        InitializeComponent();
    }
}
