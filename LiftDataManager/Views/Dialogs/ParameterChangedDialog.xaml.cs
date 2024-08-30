using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;
public sealed partial class ParameterChangedDialog : ContentDialog
{
    public ParameterChangedDialogViewModel ViewModel { get; }
    public List<InfoCenterEntry>? ParameterChangedList { get; set; }
    public InfoCenterEntry? SelectedParameter { get; set; }
    public ParameterChangedDialog()
    {
        ViewModel = App.GetService<ParameterChangedDialogViewModel>();
        this.InitializeComponent();
    }
}
