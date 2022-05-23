using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

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
