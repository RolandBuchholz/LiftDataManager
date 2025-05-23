using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class ImportLiftDataDialog : ContentDialog
{
    public string FullPathXml { get; set; }
    public bool VaultDisabled { get; set; }
    public string SpezifikationName { get; set; }
    public SpezifikationTyp CurrentSpezifikationTyp { get; set; }
    public string? ImportSpezifikationName { get; set; }

    public IEnumerable<TransferData>? ImportPamameter { get; set; }

    public ImportLiftDataDialogViewModel ViewModel
    {
        get;
    }

    public ImportLiftDataDialog(string fullPathXml, string spezifikationName, SpezifikationTyp spezifikationTyp, bool vaultDisabled)
    {
        ViewModel = App.GetService<ImportLiftDataDialogViewModel>();
        DataContext = ViewModel;
        FullPathXml = fullPathXml;
        SpezifikationName = spezifikationName;
        CurrentSpezifikationTyp = spezifikationTyp;
        VaultDisabled = vaultDisabled;
        InitializeComponent();
    }
}
