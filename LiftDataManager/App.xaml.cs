using LiftDataManager.Core.Services;
using LiftDataManager.Models;
using LiftDataManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiftDataManager;

public partial class App : Application
{
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static FrameworkElement? MainRoot
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
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
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}