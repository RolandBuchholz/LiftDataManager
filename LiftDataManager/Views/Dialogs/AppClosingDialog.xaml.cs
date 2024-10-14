using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views;

public sealed partial class AppClosingDialog : ContentDialog
{
    public AppClosingDialogViewModel ViewModel
    {
        get;
    }

    public bool IgnoreSaveWarning { get; set; }

    public AppClosingDialog()
    {
        ViewModel = App.GetService<AppClosingDialogViewModel>();
        InitializeComponent();
    }
}
