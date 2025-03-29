namespace LiftDataManager.Views;

public sealed partial class SignalisationPage : Page
{
    public SignalisationViewModel ViewModel
    {
        get;
    }

    public SignalisationPage()
    {
        ViewModel = App.GetService<SignalisationViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
