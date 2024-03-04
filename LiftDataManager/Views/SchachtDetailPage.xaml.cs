namespace LiftDataManager.Views;

public sealed partial class SchachtDetailPage : Page
{
    public SchachtDetailViewModel ViewModel
    {
        get;
    }

    public SchachtDetailPage()
    {
        ViewModel = App.GetService<SchachtDetailViewModel>();
        InitializeComponent();
    }
}
