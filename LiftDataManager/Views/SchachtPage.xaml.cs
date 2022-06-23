using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

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

        private void textbox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            e.Handled = true;
        }
    }
}
