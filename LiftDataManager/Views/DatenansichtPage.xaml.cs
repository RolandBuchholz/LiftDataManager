namespace LiftDataManager.Views;

public sealed partial class DatenansichtPage : Page
{
    public DatenansichtViewModel ViewModel
    {
        get;
    }

    public DatenansichtPage()
    {
        ViewModel = App.GetService<DatenansichtViewModel>();
        InitializeComponent();
    }
}
