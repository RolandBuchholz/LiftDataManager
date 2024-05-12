namespace LiftDataManager.Views;

public sealed partial class WartungMontageTüvPage : Page
{
    public WartungMontageTüvViewModel ViewModel
    {
        get;
    }

    public WartungMontageTüvPage()
    {
        ViewModel = App.GetService<WartungMontageTüvViewModel>();
        InitializeComponent();
    }
}
