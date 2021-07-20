using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }
        public SettingsPage()
        {
            ViewModel = Ioc.Default.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}
