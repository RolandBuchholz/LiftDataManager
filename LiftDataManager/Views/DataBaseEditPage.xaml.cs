namespace LiftDataManager.Views;

public sealed partial class DataBaseEditPage : Page
{
    public DataBaseEditViewModel ViewModel
    {
        get;
    }

    public DataBaseEditPage()
    {
        ViewModel = App.GetService<DataBaseEditViewModel>();
        InitializeComponent();
    }
}