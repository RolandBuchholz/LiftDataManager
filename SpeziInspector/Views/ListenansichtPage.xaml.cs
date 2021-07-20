using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
    public sealed partial class ListenansichtPage : Page
    {
        public ListenansichtViewModel ViewModel { get; }
        public ListenansichtPage()
        {
            ViewModel = Ioc.Default.GetService<ListenansichtViewModel>();
            InitializeComponent();
        }
        private void OnViewStateChanged(object sender, ListDetailsViewState e)
        {
            if (e == ListDetailsViewState.Both)
            {
                ViewModel.EnsureItemSelected();
            }
        }
    }
}
