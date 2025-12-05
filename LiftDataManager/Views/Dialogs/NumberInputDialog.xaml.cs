using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class NumberInputDialog : ContentDialog
{
    public NumberInputDialogViewModel ViewModel
    {
        get;
    }

    public string? Message { get; set; }
    public string? TextBoxName { get; set; }
    public int InputNumber { get; set; }
    public int MaxLength { get; set; }
    public int MinLength { get; set; }

    public NumberInputDialog()
    {
        ViewModel = App.GetService<NumberInputDialogViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }
}
