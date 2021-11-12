using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class SignalisationPage : Page
    {
        public SignalisationViewModel ViewModel { get; }

        public SignalisationPage()
        {
            ViewModel = Ioc.Default.GetService<SignalisationViewModel>();
            InitializeComponent();
        }
    }
}
