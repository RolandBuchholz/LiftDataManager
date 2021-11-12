using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class BausatzPage : Page
    {
        public BausatzViewModel ViewModel { get; }

        public BausatzPage()
        {
            ViewModel = Ioc.Default.GetService<BausatzViewModel>();
            InitializeComponent();
        }
    }
}
