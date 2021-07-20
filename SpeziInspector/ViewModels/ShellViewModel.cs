using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Navigation;
using SpeziInspector.Contracts.Services;
using SpeziInspector.Core.Messenger;
using SpeziInspector.Core.Messenger.Messages;
using SpeziInspector.Views;

namespace SpeziInspector.ViewModels
{
    public class ShellViewModel : ObservableRecipient
    {
        private bool _isBackEnabled;
        private object _selected;
        private CurrentSpeziProperties _CurrentSpeziProperties = new();
        public INavigationService NavigationService { get; }
        public INavigationViewService NavigationViewService { get; }
        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }
        public object Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
        {
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;
            NavigationViewService = navigationViewService;
            Messenger.Register<SpeziPropertiesRequestMessage>(this, (r, m) =>
            {
                m.Reply(_CurrentSpeziProperties);
            });

            Messenger.Register<SpeziPropertiesChangedMassage>(this, (r, m) =>
            {
                _CurrentSpeziProperties = m.Value;
            });
        }
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = NavigationViewService.SettingsItem;
                return;
            }
            var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }
    }
}
