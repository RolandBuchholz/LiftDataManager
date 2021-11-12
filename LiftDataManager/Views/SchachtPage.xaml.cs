using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class SchachtPage : Page
    {
        public SchachtViewModel ViewModel { get; }

        public SchachtPage()
        {
            ViewModel = Ioc.Default.GetService<SchachtViewModel>();
            InitializeComponent();
        }
    }
}
