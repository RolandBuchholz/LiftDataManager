using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
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
