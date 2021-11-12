using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class TürenPage : Page
    {
        public TürenViewModel ViewModel { get; }

        public TürenPage()
        {
            ViewModel = Ioc.Default.GetService<TürenViewModel>();
            InitializeComponent();
        }
    }
}
