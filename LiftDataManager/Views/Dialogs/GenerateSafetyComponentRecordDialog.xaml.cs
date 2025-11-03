using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class GenerateSafetyComponentRecordDialog : ContentDialog
{
    public int LiftCommissionId { get; set; }
    public ObservableDBSafetyComponentRecord? SafetyComponentRecord { get; set; }

    public GenerateSafetyComponentRecordDialogViewModel ViewModel
    {
        get;
    }

    public GenerateSafetyComponentRecordDialog(int liftCommissionId)
    {
        ViewModel = App.GetService<GenerateSafetyComponentRecordDialogViewModel>();
        DataContext = ViewModel;
        LiftCommissionId = liftCommissionId;
        InitializeComponent();
    }
}
