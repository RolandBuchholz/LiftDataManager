using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.Collections;

namespace LiftDataManager.Views;

public sealed partial class TabellenansichtPage : Page
{
    public TabellenansichtViewModel ViewModel
    {
        get;
    }

    public TabellenansichtPage()
    {
        ViewModel = App.GetService<TabellenansichtViewModel>();
        InitializeComponent();
    }

    private void DataGrid_LoadingRowGroup(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridRowGroupHeaderEventArgs e)
    {
        ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
        e.RowGroupHeader.PropertyValue = ((ObservableGroup<string, Parameter>)group.Group).Key;
        e.RowGroupHeader.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
    }
}
