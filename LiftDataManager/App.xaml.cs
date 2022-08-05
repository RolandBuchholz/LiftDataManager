using LiftDataManager.Core.Services;
using LiftDataManager.Models;
using LiftDataManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiftDataManager;

public partial class App : Application
{
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<ISettingService, SettingsService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Core Services
            services.AddSingleton<IParameterDataService, ParameterDataService>();
            services.AddSingleton<IAuswahlParameterDataService, AuswahlParameterDataService>();
            services.AddSingleton<IVaultDataService, VaultDataService>();
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<WartungMontageTüvViewModel>();
            services.AddTransient<WartungMontageTüvPage>();
            services.AddTransient<TürenViewModel>();
            services.AddTransient<TürenPage>();
            services.AddTransient<EinreichunterlagenViewModel>();
            services.AddTransient<EinreichunterlagenPage>();
            services.AddTransient<SonstigesViewModel>();
            services.AddTransient<SonstigesPage>();
            services.AddTransient<SignalisationViewModel>();
            services.AddTransient<SignalisationPage>();
            services.AddTransient<SchachtViewModel>();
            services.AddTransient<SchachtPage>();
            services.AddTransient<QuickLinksViewModel>();
            services.AddTransient<QuickLinksPage>();
            services.AddTransient<KabineViewModel>();
            services.AddTransient<KabinePage>();
            services.AddTransient<KabineDetailViewModel>();
            services.AddTransient<KabineDetailPage>();
            services.AddTransient<BausatzViewModel>();
            services.AddTransient<BausatzPage>();
            services.AddTransient<KabinengewichtViewModel>();
            services.AddTransient<KabinengewichtPage>();
            services.AddTransient<NutzlastberechnungViewModel>();
            services.AddTransient<NutzlastberechnungPage>();
            services.AddTransient<KabinenLüftungViewModel>();
            services.AddTransient<KabinenLüftungPage>();
            services.AddTransient<AntriebSteuerungNotrufViewModel>();
            services.AddTransient<AntriebSteuerungNotrufPage>();
            services.AddTransient<AllgemeineDatenViewModel>();
            services.AddTransient<AllgemeineDatenPage>();
            services.AddTransient<TabellenansichtViewModel>();
            services.AddTransient<TabellenansichtPage>();
            services.AddTransient<DatenansichtDetailViewModel>();
            services.AddTransient<DatenansichtDetailPage>();
            services.AddTransient<DatenansichtViewModel>();
            services.AddTransient<DatenansichtPage>();
            services.AddTransient<ListenansichtViewModel>();
            services.AddTransient<ListenansichtPage>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<HomePage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        })
        .Build();

    public static T GetService<T>()
        where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };
    public static FrameworkElement MainRoot
    {
        get; set;
    }
    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        var activationService = App.GetService<IActivationService>();
        await activationService.ActivateAsync(args);
    }
}
