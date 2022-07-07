using System;
using System.Threading.Tasks;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Helpers;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace LiftDataManager.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string SettingsKey = "AppBackgroundRequestedTheme";
    private const string SettingsKeyDefaultAccentColor = "AppDefaultAccentColorRequested";

    public ElementTheme Theme { get; set; } = ElementTheme.Default;

    private readonly ILocalSettingsService _localSettingsService;

    public ThemeSelectorService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        await SetAccentColorAsync();
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    private async Task SetAccentColorAsync()
    {
        var defaultAccentColor = Convert.ToBoolean(await _localSettingsService.ReadSettingAsync<string>(SettingsKeyDefaultAccentColor));

        if (!defaultAccentColor)
        {
            var systemAccentColor = Color.FromArgb(255, 0, 85, 173);
            var systemAccentColorLight1 = Color.FromArgb(255, 0, 100, 190);
            var systemAccentColorLight2 = Color.FromArgb(255, 0, 115, 207);
            var systemAccentColorLight3 = Color.FromArgb(255, 29, 133, 215);
            var systemAccentColorDark1 = Color.FromArgb(255, 0, 69, 157);
            var systemAccentColorDark2 = Color.FromArgb(255, 0, 54, 140);
            var systemAccentColorDark3 = Color.FromArgb(255, 0, 39, 123);

            Application.Current.Resources["SystemAccentColor"] = systemAccentColor;
            Application.Current.Resources["SystemAccentColorLight1"] = systemAccentColorLight1;
            Application.Current.Resources["SystemAccentColorLight2"] = systemAccentColorLight2;
            Application.Current.Resources["SystemAccentColorLight3"] = systemAccentColorLight3;
            Application.Current.Resources["SystemAccentColorDark1"] = systemAccentColorDark1;
            Application.Current.Resources["SystemAccentColorDark2"] = systemAccentColorDark2;
            Application.Current.Resources["SystemAccentColorDark3"] = systemAccentColorDark3;
        }
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (Enum.TryParse(themeName, out ElementTheme cacheTheme))
        {
            return cacheTheme;
        }

        return ElementTheme.Default;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
    }
}
