using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class LiftPlannerDBDialog : ContentDialog
{
    public int LiftPlannerId { get; set; }

    public LiftPlannerDBDialogViewModel ViewModel
    {
        get;
    }
    public LiftPlannerDBDialog()
    {
        ViewModel = App.GetService<LiftPlannerDBDialogViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }

    private void ComboBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null)
            {
                return;
            }
            if (comboBox.SelectedItem is Country country)
            {
                comboBox.SelectedIndex = country.Id - 1;
            }
        }
    }
}
