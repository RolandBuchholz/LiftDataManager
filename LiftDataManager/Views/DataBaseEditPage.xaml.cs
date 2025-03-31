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

    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.Column != null)
        {
            if (e.PropertyName == "Id" ||
                e.PropertyName == "Name" ||
                e.PropertyName == "LiftTypes" ||
                e.PropertyName == "GuideModelTypes" ||
                e.PropertyName == "DriveSystems" ||
                e.PropertyName == "LiftDoorGroups" ||
                e.PropertyName == "CarDoors" ||
                e.PropertyName == "ShaftDoors" ||
                e.PropertyName == "OverspeedGovernors" ||
                e.PropertyName == "LiftPositionSystems" ||
                e.PropertyName == "SafetyGearModelTypes" ||
                e.PropertyName == "CarFrameTypes")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "CarFrameBaseType" ||
                     e.PropertyName == "GuideType" ||
                     e.PropertyName == "DriveType" ||
                     e.PropertyName == "CargoType" ||
                     e.PropertyName == "SafetyGearType" ||
                     e.PropertyName == "TypeExaminationCertificate" ||
                     e.PropertyName == "LiftDoorOpeningDirection" ||
                     e.PropertyName == "DriveSystemType")
            {
                Binding binding = new()
                {
                    Path = new PropertyPath(e.PropertyName),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                    Converter = new NavigationPropertyToStringConverter()
                };
                DataGridTextColumn dataGridTextColumn = new()
                {
                    Header = e.PropertyName,
                    Binding = binding
                };
                e.Column = dataGridTextColumn;
                e.Column.IsReadOnly = true;
            }
            else
            {
                if (e.PropertyType == typeof(double?))
                {
                    Binding binding = new()
                    {
                        Path = new PropertyPath(e.PropertyName),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                        Converter = new DoubleToStringConverter()
                    };
                    DataGridTextColumn dataGridTextColumn = new()
                    {
                        Header = e.PropertyName,
                        Binding = binding
                    };
                    e.Column = dataGridTextColumn;
                }
            }
        }
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