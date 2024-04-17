using LiftDataManager.Contracts.Services;
using Windows.UI.ViewManagement;
using WinUICommunity;

namespace LiftDataManager.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly ISettingService _settingService;
    private readonly IThemeService _themeService;
    private UIElement? _shell = null;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IThemeService themeService,
        ISettingService settingService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _settingService = settingService;
        _themeService = themeService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
            App.MainRoot = App.MainWindow.Content as FrameworkElement;
        }
        await SetAccentColorAsync();
        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

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
        await _settingService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        _themeService.Initialize(App.MainWindow);
        _themeService.ConfigBackdrop();
        _themeService.ConfigElementTheme();
        await Task.CompletedTask;
    }

    private async Task SetAccentColorAsync()
    {
        var defaultAccentColor = Convert.ToBoolean(_settingService.CustomAccentColor);
        if (!defaultAccentColor)
        {
            var systemAccentColor = Color.FromArgb(255, 0, 85, 173);
            var systemAccentColorLight1 = Color.FromArgb(255, 0, 100, 190);
            var systemAccentColorLight2 = Color.FromArgb(255, 72, 178, 234);
            var systemAccentColorLight3 = Color.FromArgb(255, 29, 133, 215);
            var systemAccentColorDark1 = Color.FromArgb(255, 0, 69, 157);
            var systemAccentColorDark2 = Color.FromArgb(255, 0, 54, 140);
            var systemAccentColorDark3 = Color.FromArgb(255, 0, 39, 123);

            App.Current.Resources["SystemAccentColor"] = systemAccentColor;
            App.Current.Resources["SystemAccentColorLight1"] = systemAccentColorLight1;
            App.Current.Resources["SystemAccentColorLight2"] = systemAccentColorLight2;
            App.Current.Resources["SystemAccentColorLight3"] = systemAccentColorLight3;
            App.Current.Resources["SystemAccentColorDark1"] = systemAccentColorDark1;
            App.Current.Resources["SystemAccentColorDark2"] = systemAccentColorDark2;
            App.Current.Resources["SystemAccentColorDark3"] = systemAccentColorDark3;
        }
        await Task.CompletedTask;
    }
}
