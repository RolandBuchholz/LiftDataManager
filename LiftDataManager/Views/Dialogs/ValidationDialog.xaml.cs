using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class ValidationDialog : ContentDialog
{
    public ValidationDialogViewModel ViewModel
    {
        get;
    }
    public string? FullPathXml { get; set; }
    public ValidationDialog()
    {
        ViewModel = App.GetService<ValidationDialogViewModel>();
        InitializeComponent();
    }
}