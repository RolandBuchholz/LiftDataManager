namespace LiftDataManager.Views;

public sealed partial class LiftHistoryPage : Page
{
    public LiftHistoryViewModel ViewModel
    {
        get;
    }

    public LiftHistoryPage()
    {
        ViewModel = App.GetService<LiftHistoryViewModel>();
        InitializeComponent();
    }

    private void EditComment_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not null && sender.GetType() == typeof(Button))
            _ = ViewModel.EditCommentAsync(((Button)sender).CommandParameter);
    }
}
