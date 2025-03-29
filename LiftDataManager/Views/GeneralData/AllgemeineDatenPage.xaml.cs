namespace LiftDataManager.Views;

public sealed partial class AllgemeineDatenPage : Page
{
    public AllgemeineDatenViewModel ViewModel
    {
        get;
    }

    public AllgemeineDatenPage()
    {
        ViewModel = App.GetService<AllgemeineDatenViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
