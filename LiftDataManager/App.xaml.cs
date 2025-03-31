using LiftDataManager.Core.Services;
using LiftDataManager.Models;
using LiftDataManager.Services;
using LiftDataManager.ViewModels.Dialogs;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.Storage;

namespace LiftDataManager;

public partial class App : Application
{
    public IHost Host { get; }
    public static WindowEx MainWindow { get; } = new MainWindow();
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
    public string AppName { get; } = Package.Current.DisplayName;
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
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IJsonNavigationService, JsonNavigationService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<ISettingService, SettingsService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddSingleton<IInfoCenterService, InfoCenterService>();

            // DataBase Services
            services.AddDbContext<ParameterContext>(options => options.UseSqlite(GetConnectionString(true))
                                                                      .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddDbContext<ParameterEditContext>(options => options.UseSqlite(GetConnectionString(false)));
            // Core Services
            services.AddSingleton<IParameterDataService, ParameterDataService>();
            services.AddSingleton<IValidationParameterDataService, ValidationParameterDataService>();
            services.AddSingleton<IVaultDataService, VaultDataService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ICalculationsModule, CalculationsModuleService>();
            services.AddSingleton<IStorageService, StorageService>();

            // ViewModels
            services.AddTransient<AboutSettingViewModel>();
            services.AddTransient<GeneralSettingViewModel>();
            services.AddTransient<MaintenanceSettingViewModel>();
            services.AddTransient<ThemeSettingViewModel>();
            services.AddTransient<SchachtDetailViewModel>();
            services.AddTransient<BausatzDetailViewModel>();
            services.AddTransient<BausatzDetailRailBracketViewModel>();
            services.AddTransient<HelpViewModel>();
            services.AddTransient<LiftHistoryViewModel>();
            services.AddTransient<DataBaseEditViewModel>();
            services.AddTransient<ErrorViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<WartungMontageTüvViewModel>();
            services.AddTransient<TürenViewModel>();
            services.AddTransient<EinreichunterlagenViewModel>();
            services.AddTransient<SonstigesViewModel>();
            services.AddTransient<SignalisationViewModel>();
            services.AddTransient<SchachtViewModel>();
            services.AddSingleton<QuickLinksViewModel>();
            services.AddTransient<KabineViewModel>();
            services.AddTransient<KabineDetailViewModel>();
            services.AddTransient<KabineDetailEquipmentViewModel>();
            services.AddTransient<KabineDetailFloorViewModel>();
            services.AddTransient<KabineDetailLayoutViewModel>();
            services.AddTransient<KabineDetailCeilingViewModel>();
            services.AddTransient<KabineDetailControlPanelViewModel>();
            services.AddTransient<BausatzViewModel>();
            services.AddTransient<KabinengewichtViewModel>();
            services.AddTransient<NutzlastberechnungViewModel>();
            services.AddTransient<KabinenLüftungViewModel>();
            services.AddTransient<AntriebSteuerungNotrufViewModel>();
            services.AddTransient<AllgemeineDatenViewModel>();
            services.AddTransient<TabellenansichtViewModel>();
            services.AddTransient<DatenansichtDetailViewModel>();
            services.AddTransient<DatenansichtViewModel>();
            services.AddTransient<ListenansichtViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // DialogViewModels
            services.AddTransient<LiftPlannerDBDialogViewModel>();
            services.AddTransient<PasswortDialogViewModel>();
            services.AddTransient<ZALiftDialogViewModel>();
            services.AddTransient<CFPEditDialogViewModel>();
            services.AddTransient<ParameterChangedDialogViewModel>();
            services.AddTransient<AppClosingDialogViewModel>();
            services.AddTransient<ImportLiftDataDialogViewModel>();
            services.AddTransient<CheckOutDialogViewModel>();
            services.AddTransient<ValidationDialogViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        MainWindow.Closed += MainWindow_Closed;
        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    public void DisableSaveWarning() => IgnoreSaveWarning = true;

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        var _dialogService = GetService<IDialogService>();
        if (_dialogService is null)
        {
            IgnoreSaveWarning = true;
        }
        if (IgnoreSaveWarning)
        {
            args.Handled = false;
            Current.Exit();
            return;
        }
        args.Handled = !IgnoreSaveWarning;
    CheckOut:
        var dialogResult = await _dialogService!.AppClosingDialogAsync(IgnoreSaveWarning);
        switch (dialogResult.Item1)
        {
            case ContentDialogResult.None:
                if (dialogResult.Item2)
                {
                    args.Handled = false;
                    IgnoreSaveWarning = true;
                    Current.Exit();
                }
                else
                {
                    args.Handled = true;
                }
                break;
            case ContentDialogResult.Primary:
                if (dialogResult.Item2)
                {
                    args.Handled = false;
                    IgnoreSaveWarning = true;
                    Current.Exit();
                    break;
                }
                else
                {
                    args.Handled = true;
                    goto CheckOut;
                }
            case ContentDialogResult.Secondary:
                args.Handled = false;
                IgnoreSaveWarning = true;
                Current.Exit();
                break;
            default:
                break;
        }
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
        var dbConnectionStringLogger = GetService<ILogger<App>>();
        var installationPath = AppDomain.CurrentDomain.BaseDirectory;
        dbConnectionStringLogger.LogInformation(00104, "DbConnectionString InstallationPath: {installationPath} ", installationPath);
        string dbPath = @"\\Bauer\aufträge neu\Vorlagen\DataBase\LiftDataParameter.db";
        bool vaultDisabled = false;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppVaultDisabledRequested", out var vaultSettingValue))
        {
            if (!string.IsNullOrWhiteSpace((string)vaultSettingValue))
            {
                vaultDisabled = JsonConvert.DeserializeObject<bool>((string)vaultSettingValue, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                dbConnectionStringLogger.LogInformation(00101, "DbConnectionString VaultDisabled: {vaultDisabled} ", vaultDisabled);
            }
        }

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppPathDataBaseRequested", out var dbPathSettingValue))
        {
            if (!string.IsNullOrWhiteSpace((string)dbPathSettingValue))
            {
                var dbPathValue = JsonConvert.DeserializeObject<string>((string)dbPathSettingValue, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                dbConnectionStringLogger.LogInformation(00102, "DbConnectionString SettingsDBPath: {dbPathValue} ", dbPathValue);
                dbPath = string.IsNullOrWhiteSpace(dbPathValue) ? dbPath : dbPathValue;
                dbConnectionStringLogger.LogInformation(00103, "DbConnectionString selected DBPath: {dbPath} ", dbPath);
            }
        }

        if (!File.Exists(dbPath) || vaultDisabled)
        {
            dbPath = Path.Combine(installationPath, "LiftDataManager.Core", "Assets", "DataComponents", "LiftDataParameter.db");
            dbConnectionStringLogger.LogInformation(00104, "DbConnectionString DBPath LocalMode: {dbPath} ", dbPath);
        }

        //TODO UseForLocalModeSpecialFolder
        
        string workPathDb = @"C:\Work\Administration\DataBase\LiftDataParameter.db";

        if (!Directory.Exists(Path.GetDirectoryName(workPathDb)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workPathDb)!);
        }
        dbConnectionStringLogger.LogInformation(00106, "DbConnectionString workPathDb: {workPathDb} ", workPathDb);

        if (!Directory.Exists(Path.GetDirectoryName(workPathDb)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workPathDb)!);
        }
        if (!string.IsNullOrWhiteSpace(dbPath) && dbReadOnly)
        {
            if (File.Exists(workPathDb))
            {
                FileInfo workPathDbFileInfo = new(workPathDb);
                if (workPathDbFileInfo.IsReadOnly)
                {
                    workPathDbFileInfo.IsReadOnly = false;
                }
            }
            File.Copy(dbPath, workPathDb, true);
        }

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
        var navigationService = GetService<IJsonNavigationService>();
        navigationService?.NavigateTo(typeof(ErrorPage), new ErrorPageInfo(membername, sender, e), true);
    }
}
