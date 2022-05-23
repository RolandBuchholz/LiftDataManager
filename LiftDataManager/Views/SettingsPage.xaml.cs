using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
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
