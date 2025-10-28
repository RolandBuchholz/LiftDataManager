namespace LiftDataManager.Views;

public sealed partial class CurrentSafetyComponentsPage : Page
{
    public CurrentSafetyComponentsViewModel ViewModel
    {
        get;
    }

    public CurrentSafetyComponentsPage()
    {
        ViewModel = App.GetService<CurrentSafetyComponentsViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not null && sender.GetType() == typeof(Button))
        {
           _ = ViewModel.DeleteSafetyComponentRecordAsync(((Button)sender).CommandParameter);
        }
    }
}
