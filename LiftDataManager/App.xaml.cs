using System.Runtime.CompilerServices;

using LiftDataManager.Core.Services;
using LiftDataManager.Models;
using LiftDataManager.Services;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;

using Windows.Storage;

namespace LiftDataManager;

public partial class App : Application
{
    public IHost Host { get; }
    public static WindowEx MainWindow { get; } = new MainWindow();
    public static UIElement? AppTitlebar { get; set; }
    private bool IgnoreSaveWarning { get; set; }
    public static FrameworkElement? MainRoot { get; set; }

    public static T GetService<T>()
    where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

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
                .MinimumLevel.ControlledBy(SetLogLevel());
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
            services.AddTransient<IPdfService, PdfService>();

            // DataBase Services
            services.AddDbContext<ParameterContext>(options => options.UseSqlite(GetConnectionString(true))
                                                                      .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            // Core Services
            services.AddSingleton<IParameterDataService, ParameterDataService>();
            services.AddSingleton<IValidationParameterDataService, ValidationParameterDataService>();
            services.AddSingleton<IVaultDataService, VaultDataService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ICalculationsModule, CalculationsModuleService>();

            // Views and ViewModels
            services.AddTransient<HelpViewModel>();
            services.AddTransient<HelpPage>();
            services.AddTransient<LiftHistoryViewModel>();
            services.AddTransient<LiftHistoryPage>();
            services.AddTransient<DataBaseEditViewModel>();
            services.AddTransient<DataBaseEditPage>();
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
            services.AddSingleton<QuickLinksViewModel>();
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

        MainWindow.Closed += MainWindow_Closed;
        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {

        var tempHomeViewModel = GetService<HomeViewModel>();
        var currentSpeziProperties = tempHomeViewModel.GetCurrentSpeziProperties();

        if (currentSpeziProperties != null && currentSpeziProperties.CheckOut && !IgnoreSaveWarning)
        {
            args.Handled = true;

            var dirty = currentSpeziProperties.ParameterDictionary!.Values.Any(p => p.IsDirty);

            var title = dirty ? "Warnung nicht gepeicherte Parameter gefunden" : "Warnung Autodesktranfer noch nicht hochgeladen";
            var message = dirty ? "Es sind nicht gespeicherte Parameter vorhanden.\n" +
                                  "Wollen Sie diese speichern?"
                                : "Der Auftrag wurde noch nicht ins Vault hochgeladen.\n" +
                                  "Wollen Sie den Auftrag ins Vault hochladen?";
            var yesButtonText = dirty ? "Parameter speichern" : "Hochladen und Schließen";
            var noButtonText = dirty ? "Ohne Speichern schließen" : "Ohne Hochladen Schließen";

            var dialogResult = await tempHomeViewModel._dialogService!.WarningDialogAsync(title, message, yesButtonText, noButtonText);

            if (dialogResult is not null && (bool)dialogResult)
            {
                if (dirty)
                {
                    await tempHomeViewModel._parameterDataService!.SaveAllParameterAsync(currentSpeziProperties.ParameterDictionary, currentSpeziProperties.FullPathXml!, currentSpeziProperties.Adminmode);
                }
                else
                {
                    var spezifikationName = Path.GetFileName(currentSpeziProperties.FullPathXml!).Replace("-AutoDeskTransfer.xml", "");
                    await tempHomeViewModel._vaultDataService.SetFileAsync(spezifikationName);
                    IgnoreSaveWarning = true;
                    Current.Exit();
                }
            }
            else
            {
                args.Handled = false;
                IgnoreSaveWarning = true;
                Current.Exit();
            }
        }
        Current.Exit();
    }

    private static LoggingLevelSwitch SetLogLevel()
    {
        LoggingLevelSwitch loglevel = new();

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppLoglevelRequested", out var currentLogLevel))
        {
            var logLevel = JsonConvert.DeserializeObject<string>((string)currentLogLevel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            loglevel.MinimumLevel = logLevel switch
            {
                "Verbose" => Serilog.Events.LogEventLevel.Verbose,
                "Debug" => Serilog.Events.LogEventLevel.Debug,
                "Information" => Serilog.Events.LogEventLevel.Information,
                "Warning" => Serilog.Events.LogEventLevel.Warning,
                "Error" => Serilog.Events.LogEventLevel.Error,
                "Fatal" => Serilog.Events.LogEventLevel.Fatal,
                _ => Serilog.Events.LogEventLevel.Information,
            };
        }
        else
        {
            loglevel.MinimumLevel = Serilog.Events.LogEventLevel.Information;
        }
        return loglevel;
    }

    public static string GetConnectionString(bool dbReadOnly)
    {
        const string workPathDb = @"C:\Work\Administration\DataBase\LiftDataParameter.db";
        string? dbPath = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db";

        if (!Directory.Exists(Path.GetDirectoryName(workPathDb)!))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workPathDb)!);
        }

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppPathDataBaseRequested", out var obj))
        {
            if (!string.IsNullOrWhiteSpace((string)obj))
                dbPath = JsonConvert.DeserializeObject<string>((string)obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }

        if (dbPath is not null && dbReadOnly)
            File.Copy(dbPath, workPathDb, true);
        
        var sqliteOpenMode = dbReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite;

        return new SqliteConnectionStringBuilder()
        {
            DataSource = dbReadOnly ? workPathDb : dbPath,
            Mode = sqliteOpenMode,
            ForeignKeys = true,
        }.ToString();
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

    private static void SwitchToErrorHandlingPage(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e, [CallerMemberName] string membername = "")
    {
        MainWindow.Activate();
        var _navigationService = GetService<INavigationService>();
        _navigationService?.NavigateTo("LiftDataManager.ViewModels.ErrorViewModel", new ErrorPageInfo(membername, sender, e), true);
    }
}
