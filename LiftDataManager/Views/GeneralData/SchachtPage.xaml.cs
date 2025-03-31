namespace LiftDataManager.Views;

public sealed partial class SchachtPage : Page
{
    public SchachtViewModel ViewModel
    {
        get;
    }

    public SchachtPage()
    {
        ViewModel = App.GetService<SchachtViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
