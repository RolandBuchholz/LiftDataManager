using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class CheckOutDialog : ContentDialog
{
    public CheckOutDialogResult CheckOutDialogResult { get; set; }
    public string SpezifikationName { get; set; }
    public bool ForceCheckOut { get; set; }

    public CheckOutDialogViewModel ViewModel
    {
        get;
    }

    public CheckOutDialog(string spezifikationName, bool forceCheckOut)
    {
        ViewModel = App.GetService<CheckOutDialogViewModel>();
        DataContext = ViewModel;
        SpezifikationName = spezifikationName;
        ForceCheckOut = forceCheckOut;
        InitializeComponent();
    }
}
