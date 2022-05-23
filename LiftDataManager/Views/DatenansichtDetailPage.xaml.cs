using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.UI.Animations;
using LiftDataManager.Contracts.Services;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace LiftDataManager.Views
{
    public sealed partial class DatenansichtDetailPage : Page
    {
        public DatenansichtDetailViewModel ViewModel { get; }

        public DatenansichtDetailPage()
        {
            ViewModel = Ioc.Default.GetService<DatenansichtDetailViewModel>();
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var navigationService = Ioc.Default.GetService<INavigationService>();
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}
