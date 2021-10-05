using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.UI.Xaml.Controls;

using SpeziInspector.ViewModels;

namespace SpeziInspector.Views
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
