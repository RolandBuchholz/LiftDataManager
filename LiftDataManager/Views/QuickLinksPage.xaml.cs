using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using LiftDataManager.ViewModels;

namespace LiftDataManager.Views
{
    public sealed partial class QuickLinksPage : Page
    {
        public QuickLinksViewModel ViewModel { get; }

        public QuickLinksPage()
        {
            ViewModel = Ioc.Default.GetService<QuickLinksViewModel>();
            InitializeComponent();
        }
    }
}
