using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
    // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
    // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
    public sealed partial class TabellenansichtPage : Page
    {
        public TabellenansichtViewModel ViewModel { get; }

        public TabellenansichtPage()
        {
            ViewModel = Ioc.Default.GetService<TabellenansichtViewModel>();
            InitializeComponent();
        }
    }
}
