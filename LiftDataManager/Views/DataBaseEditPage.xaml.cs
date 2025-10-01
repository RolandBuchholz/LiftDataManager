using CommunityToolkit.WinUI.UI.Controls;

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
        DataContext = ViewModel;
        InitializeComponent();
    }

    //Workaround brocken datatemplete binding datagrid
    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        var commandParameter = ((Button)sender).CommandParameter;
        if (commandParameter != null)
        {
            ViewModel.RemoveRowFromDataBaseTableCommand.Execute(commandParameter);
        }
    }
}