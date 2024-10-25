namespace LiftDataManager.ViewModels.Dialogs;

public partial class ValidationDialogViewModel : ObservableObject
{
    public ValidationDialogViewModel()
    {

    }

    [ObservableProperty]
    private int errorCount;

    [ObservableProperty]
    private int warningCount;

    [ObservableProperty]
    private int infoCount;
}