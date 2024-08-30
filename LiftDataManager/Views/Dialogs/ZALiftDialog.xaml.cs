namespace LiftDataManager.Views.Dialogs;

public sealed partial class ZALiftDialog : ContentDialog
{
    public ZALiftDialogViewModel ViewModel
    {
        get;
    }
    public string? FullPathXml { get; set; }
    public ZALiftDialog()
    {
        ViewModel = App.GetService<ZALiftDialogViewModel>();
        InitializeComponent();
    }
}
