using CommunityToolkit.Mvvm.DependencyInjection;
using LiftDataManager.Activation;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly INavigationService _navigationService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISettingService _settingService;
        private UIElement _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
            _themeSelectorService = themeSelectorService;
            _settingService = settingsSelectorService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            // Initialize services that you need before app activation
            // take into account that the splash screen is shown while this code runs.
            await InitializeAsync();

            if (App.MainWindow.Content == null)
            {
                _shell = Ioc.Default.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
                App.MainRoot = App.MainWindow.Content as FrameworkElement;
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Ensure the current window is active
            App.MainWindow.Activate();

            // Tasks after activation
            await StartupAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
        }

        private async Task InitializeAsync()
        {
            await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
            await _settingService.InitializeAsync().ConfigureAwait(false);
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            await _themeSelectorService.SetRequestedThemeAsync();
            await Task.CompletedTask;
        }
    }
}
