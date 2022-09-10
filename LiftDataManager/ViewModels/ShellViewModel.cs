﻿using Microsoft.UI.Xaml.Navigation;

namespace LiftDataManager.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    private CurrentSpeziProperties _CurrentSpeziProperties = new();
    public INavigationService NavigationService {get;}
    public INavigationViewService NavigationViewService{get;}

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

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