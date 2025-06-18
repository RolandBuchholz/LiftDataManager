namespace LiftDataManager.Views;

public sealed partial class AbbreviationPage : Page
{
    public AbbreviationViewModel ViewModel
    {
        get;
    }

    public AbbreviationPage()
    {
        ViewModel = App.GetService<AbbreviationViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

}
