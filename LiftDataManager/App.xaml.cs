using LiftDataManager.Contracts.Services;
using LiftDataManager.Core.Services;
using LiftDataManager.Models;
using LiftDataManager.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Formatting.Compact;

namespace LiftDataManager;

public partial class App : Application
{
    public IHost Host { get; }

    public static T GetService<T>()
        where T : class
    {
        if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static FrameworkElement? MainRoot { get; set; }
    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        UseSerilog((host, loggerConfiguration) =>
        {
            loggerConfiguration.WriteTo.File(new CompactJsonFormatter(), Path.Combine(Path.GetTempPath(), "LiftDataManager", "log_.json"), rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .MinimumLevel.Information();
        }).
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

            // DataBase Services
            services.AddDbContext<ParameterContext>(options => options.UseSqlite(GetConnectionString())
                                                                      .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            // Core Services
            services.AddSingleton<IParameterDataService, ParameterDataService>();
            services.AddSingleton<IValidationParameterDataService, ValidationParameterDataService>();
            services.AddSingleton<IVaultDataService, VaultDataService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ICalculationsModule, CalculationsModuleService>();

            // Views and ViewModels
            services.AddTransient<ErrorViewModel>();
            services.AddTransient<ErrorPage>();
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
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private static string GetConnectionString()
    {
        string? dbPath;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppPathDataBaseRequested", out var obj))
        {
            if (string.IsNullOrWhiteSpace((string)obj))
            {
                dbPath = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db";
            }
            else
            {
                dbPath = JsonConvert.DeserializeObject<string>((string)obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            }
        }
        else
        {
            dbPath = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db";
        }

        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadOnly,
            ForeignKeys = true,
        }.ToString();

        return connectionString;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        SwitchToErrorHandlingPage(sender, e);
    }
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {

    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await GetService<IActivationService>().ActivateAsync(args);
    }

    private void SwitchToErrorHandlingPage(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e, [CallerMemberName] string membername = "")
    {
        MainWindow.Activate();
        var _navigationService = GetService<INavigationService>();
        if (_navigationService != null)
        {
            _navigationService.NavigateTo("LiftDataManager.ViewModels.ErrorViewModel", new ErrorPageInfo(membername, sender, e), true);
        }
    }
}
