using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
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
