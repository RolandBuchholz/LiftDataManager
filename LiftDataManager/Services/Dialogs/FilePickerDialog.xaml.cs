namespace LiftDataManager.Services.Dialogs;

public sealed partial class FilePickerDialog : ContentDialog
{
    public FilePickerDialog()
    {
        InitializeComponent();
    }

    public string Message { get; set; }
}
