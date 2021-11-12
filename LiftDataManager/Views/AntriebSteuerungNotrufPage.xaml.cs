using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class AntriebSteuerungNotrufPage : Page
    {
        public AntriebSteuerungNotrufViewModel ViewModel { get; }

        public AntriebSteuerungNotrufPage()
        {
            ViewModel = Ioc.Default.GetService<AntriebSteuerungNotrufViewModel>();
            InitializeComponent();
        }
    }
}
