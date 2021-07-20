using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
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
