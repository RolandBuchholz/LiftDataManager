using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class CheckOutDialog : ContentDialog
{
    public string? FullPathXml { get; set; }
    public string? CarFrameTyp { get; set; }

    public CheckOutDialogViewModel ViewModel
    {
        get;
    }

    public CheckOutDialog()
    {
        ViewModel = App.GetService<CheckOutDialogViewModel>();
        InitializeComponent();
    }
}
