using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

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
