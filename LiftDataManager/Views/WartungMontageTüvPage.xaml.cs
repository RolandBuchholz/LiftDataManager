using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class WartungMontageTüvPage : Page
    {
        public WartungMontageTüvViewModel ViewModel { get; }

        public WartungMontageTüvPage()
        {
            ViewModel = Ioc.Default.GetService<WartungMontageTüvViewModel>();
            InitializeComponent();
        }
    }
}
