namespace LiftDataManager.Views;

public sealed partial class ZALiftDialog : ContentDialog
{
    public ZALiftDialogViewModel ViewModel
    {
        get;
    }

    public ZALiftDialog()
    {
        ViewModel = App.GetService<ZALiftDialogViewModel>();
        this.InitializeComponent();
    }
}
