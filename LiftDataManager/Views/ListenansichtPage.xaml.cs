namespace LiftDataManager.Views;

public sealed partial class ListenansichtPage : Page
{
    public ListenansichtViewModel ViewModel
    {
        get;
    }

    public ListenansichtPage()
    {
        ViewModel = App.GetService<ListenansichtViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel.EnsureItemSelected();
}
