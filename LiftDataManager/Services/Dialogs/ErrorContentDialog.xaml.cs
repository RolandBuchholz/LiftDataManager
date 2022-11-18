namespace LiftDataManager.Services.Dialogs;

public sealed partial class ErrorContentDialog : ContentDialog
{
    private string? errorMessage;

    public ErrorContentDialog()
    {
        InitializeComponent();
    }

    public string? ErrorMessage 
    {
        get => (errorMessage is not null) ? errorMessage : string.Empty;
        set => errorMessage = value; 
    }
}
