using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class ImportLiftDataDialog : ContentDialog
{
    public string FullPathXml { get; set; }
    public string SpezifikationName { get; set; }

    public ImportLiftDataDialogViewModel ViewModel
    {
        get;
    }

    public ImportLiftDataDialog(string fullPathXml, string spezifikationName)
    {
        ViewModel = App.GetService<ImportLiftDataDialogViewModel>();
        FullPathXml = fullPathXml;
        SpezifikationName = spezifikationName;
        InitializeComponent();
    }
}
