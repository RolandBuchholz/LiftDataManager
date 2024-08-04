namespace LiftDataManager.Views;

public sealed partial class CFPEditDialog : ContentDialog
{
    public CFPEditDialogViewModel ViewModel
    {
        get;
    }
    public string? FullPathXml { get; set; }
    public string? CarFrameTyp { get; set; }
    public CFPEditDialog()
    {
        ViewModel = App.GetService<CFPEditDialogViewModel>();
        InitializeComponent();
    }
}
