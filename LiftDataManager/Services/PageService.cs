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
        Configure<KabineDetailViewModel, KabineDetailPage>();
        Configure<KabinengewichtViewModel, KabinengewichtPage>();
        Configure<NutzlastberechnungViewModel, NutzlastberechnungPage>();
        Configure<KabinenLüftungViewModel, KabinenLüftungPage>();
        Configure<QuickLinksViewModel, QuickLinksPage>();
        Configure<SchachtViewModel, SchachtPage>();
        Configure<SignalisationViewModel, SignalisationPage>();
        Configure<SonstigesViewModel, SonstigesPage>();
        Configure<TürenViewModel, TürenPage>();
        Configure<WartungMontageTüvViewModel, WartungMontageTüvPage>();
        Configure<EinreichunterlagenViewModel, EinreichunterlagenPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<ErrorViewModel, ErrorPage>();
        Configure<DataBaseEditViewModel, DataBaseEditPage>();
        Configure<LiftHistoryViewModel, LiftHistoryPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
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
            var key = typeof(VM).FullName!;
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
