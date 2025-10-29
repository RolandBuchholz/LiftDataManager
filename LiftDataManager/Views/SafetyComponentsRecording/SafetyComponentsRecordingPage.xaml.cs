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
}
