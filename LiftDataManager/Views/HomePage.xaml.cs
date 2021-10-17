using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using LiftDataManager.ViewModels;

namespace LiftDataManager.Views
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel { get; }
        public HomePage()
        {
            ViewModel = Ioc.Default.GetService<HomeViewModel>();
            InitializeComponent();
        }
    }
}
