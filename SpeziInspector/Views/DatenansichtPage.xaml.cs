using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
    public sealed partial class DatenansichtPage : Page
    {
        public DatenansichtViewModel ViewModel { get; }

        public DatenansichtPage()
        {
            ViewModel = Ioc.Default.GetService<DatenansichtViewModel>();
            InitializeComponent();
        }
    }
}
