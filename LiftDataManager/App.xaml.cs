using CommunityToolkit.Mvvm.DependencyInjection;

using LiftDataManager.Activation;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Services;
using LiftDataManager.Helpers;
using LiftDataManager.Services;
using LiftDataManager.ViewModels;
using LiftDataManager.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace LiftDataManager
{
    public partial class App : Application
    {
        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };
        public static FrameworkElement MainRoot { get; set; }
        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }
        private System.IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
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

            // Views and ViewModels
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<HomePage>();
            services.AddTransient<ListenansichtViewModel>();
            services.AddTransient<ListenansichtPage>();
            services.AddTransient<DatenansichtViewModel>();
            services.AddTransient<DatenansichtPage>();
            services.AddTransient<DatenansichtDetailViewModel>();
            services.AddTransient<DatenansichtDetailPage>();
            services.AddTransient<TabellenansichtViewModel>();
            services.AddTransient<TabellenansichtPage>();
            services.AddTransient<QuickLinksViewModel>();
            services.AddTransient<QuickLinksPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<KabineViewModel>();
            services.AddTransient<KabinePage>();
            services.AddTransient<AllgemeineDatenViewModel>();
            services.AddTransient<AllgemeineDatenPage>();
            services.AddTransient<SchachtViewModel>();
            services.AddTransient<SchachtPage>();
            services.AddTransient<BausatzViewModel>();
            services.AddTransient<BausatzPage>();
            services.AddTransient<TürenViewModel>();
            services.AddTransient<TürenPage>();
            services.AddTransient<AntriebSteuerungNotrufViewModel>();
            services.AddTransient<AntriebSteuerungNotrufPage>();
            services.AddTransient<SignalisationViewModel>();
            services.AddTransient<SignalisationPage>();
            services.AddTransient<WartungMontageTüvViewModel>();
            services.AddTransient<WartungMontageTüvPage>();
            services.AddTransient<SonstigesViewModel>();
            services.AddTransient<SonstigesPage>();
            return services.BuildServiceProvider();
        }
    }
}
