namespace LiftDataManager.Views.Dialogs;

public sealed partial class PasswortDialog : ContentDialog
{
    public PasswortDialogViewModel ViewModel
    {
        get;
    }
    public string? Condition { get; set; }
    public string? Description { get; set; }
    public PasswortDialog()
    {
        ViewModel = App.GetService<PasswortDialogViewModel>();
        this.InitializeComponent();
    }
}
