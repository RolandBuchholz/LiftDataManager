using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Contracts.Services;
using LiftDataManager.ViewModels;
using LiftDataManager.Views;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<HomeViewModel, HomePage>();
        Configure<ListenansichtViewModel, ListenansichtPage>();
        Configure<DatenansichtViewModel, DatenansichtPage>();
        Configure<DatenansichtDetailViewModel, DatenansichtDetailPage>();
        Configure<TabellenansichtViewModel, TabellenansichtPage>();
        Configure<AllgemeineDatenViewModel, AllgemeineDatenPage>();
        Configure<AntriebSteuerungNotrufViewModel, AntriebSteuerungNotrufPage>();
        Configure<BausatzViewModel, BausatzPage>();
        Configure<KabineViewModel, KabinePage>();
        Configure<QuickLinksViewModel, QuickLinksPage>();
        Configure<SchachtViewModel, SchachtPage>();
        Configure<SignalisationViewModel, SignalisationPage>();
        Configure<SonstigesViewModel, SonstigesPage>();
        Configure<TürenViewModel, TürenPage>();
        Configure<WartungMontageTüvViewModel, WartungMontageTüvPage>();
        Configure<SettingsViewModel, SettingsPage>();
    }

    public Type GetPageType(string key)
    {
        Type pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
