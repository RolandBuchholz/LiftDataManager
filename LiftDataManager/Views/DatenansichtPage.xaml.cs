using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using LiftDataManager.ViewModels;

namespace LiftDataManager.Views
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
