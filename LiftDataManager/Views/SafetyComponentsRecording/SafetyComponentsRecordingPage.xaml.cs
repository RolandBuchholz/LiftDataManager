namespace LiftDataManager.Views;

public sealed partial class SafetyComponentsRecordingPage : Page
{
    public SafetyComponentsRecordingViewModel ViewModel
    {
        get;
    }

    public SafetyComponentsRecordingPage()
    {
        ViewModel = App.GetService<SafetyComponentsRecordingViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    private async void Edit_SafetyComponent_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not null && sender.GetType() == typeof(Button))
        {
            await ViewModel.EditSafetyComponentAsync(((Button)sender).CommandParameter);
        }
        await Task.CompletedTask;
    }
}
