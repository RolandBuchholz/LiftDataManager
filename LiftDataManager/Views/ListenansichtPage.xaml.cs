using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class ListenansichtPage : Page
    {
        public ListenansichtViewModel ViewModel { get; }
        public ListenansichtPage()
        {
            ViewModel = Ioc.Default.GetService<ListenansichtViewModel>();
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
