namespace LiftDataManager.Views.Dialogs;

public sealed partial class LiftPlannerDBDialog : ContentDialog
{
    public string? LiftPlanner { get; set; }
    public LiftPlannerDBDialogViewModel ViewModel
    {
        get;
    }
    public LiftPlannerDBDialog()
    {
        ViewModel = App.GetService<LiftPlannerDBDialogViewModel>();
        this.InitializeComponent();
    }
}
