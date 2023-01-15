namespace LiftDataManager.Views;

public sealed partial class DataBaseEditPage : Page
{
    public DataBaseEditViewModel ViewModel
    {
        get;
    }

    public DataBaseEditPage()
    {
        ViewModel = App.GetService<DataBaseEditViewModel>();
        InitializeComponent();
    }

    private void DataGrid_AutoGeneratingColumn(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.Column != null)
        {
            if (e.PropertyName == "Id" || e.PropertyName == "Name")
            { e.Cancel = true; };
        }
    }
}