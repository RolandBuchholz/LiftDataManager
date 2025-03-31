using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class CFPEditDialog : ContentDialog
{
    public string? FullPathXml { get; set; }
    public string? CarFrameTyp { get; set; }

    public CFPEditDialogViewModel ViewModel
    {
        get;
    }

    public CFPEditDialog()
    {
        ViewModel = App.GetService<CFPEditDialogViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
