using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.Core.Models;
using LiftDataManager.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.Common.Collections;

namespace LiftDataManager.Views
{
    public sealed partial class TabellenansichtPage : Page
    {
        public TabellenansichtViewModel ViewModel { get; }

        private SolidColorBrush GroupColor { get; } = new(Colors.DarkRed);

        public TabellenansichtPage()
        {
            ViewModel = Ioc.Default.GetService<TabellenansichtViewModel>();
            InitializeComponent();
        }

        private void DataGrid_LoadingRowGroup(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            e.RowGroupHeader.PropertyValue = ((ObservableGroup<string, Parameter>)group.Group).Key;
            e.RowGroupHeader.Foreground = GroupColor;
        }
    }
}
