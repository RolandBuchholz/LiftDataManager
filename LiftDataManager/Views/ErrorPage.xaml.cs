namespace LiftDataManager.Views;

public sealed partial class ErrorPage : Page
{
    public ErrorViewModel ViewModel
    {
        get;
    }

    public ErrorPage()
    {
        ViewModel = App.GetService<ErrorViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
