using Microsoft.UI.Xaml;

using SpeziInspector.Contracts.Views;
using SpeziInspector.Helpers;
using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellWindow.xaml.
    public sealed partial class ShellWindow : Window, IShellWindow
    {
        public ShellViewModel ViewModel { get; }

        public ShellWindow(ShellViewModel viewModel)
        {
            //Title = "AppDisplayName".GetLocalized();
            Title = "Spezifikations Inspector";
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
        }
    }
}
