using System;
using System.Threading.Tasks;
using LiftDataManager.Contracts.Services;

namespace LiftDataManager.Services;

public class SettingsService : ISettingService
{
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private const string SettingsKeyDefaultAccentColor = "AppDefaultAccentColorRequested";
    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public bool Adminmode
    {
        get; set;
    }

    public bool CustomAccentColor
    {
        get; set;
    }

    public async Task InitializeAsync()
    {
        await LoadSettingsAsync("Adminmode");
        await LoadSettingsAsync("AccentColor");
        await Task.CompletedTask;
    }

    public async Task SetSettingsAsync(string key, bool value)
    {
        switch (key)
        {
            case "Adminmode":
                Adminmode = value;
                await SaveSettingsAsync(key, Adminmode);
                break;
            case "AccentColor":
                CustomAccentColor = value;
                await SaveSettingsAsync(key, CustomAccentColor);
                break;
            default:
                return;
        }
    }

    private async Task LoadSettingsAsync(string key)
    {
        switch (key)
        {
            case "Adminmode":
                var storedAdminmode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAdminmode);

                if (string.IsNullOrEmpty(storedAdminmode))
                {
                    Adminmode = false;
                }
                else
                {
                    Adminmode = Convert.ToBoolean(storedAdminmode);
                }

                break;
            case "AccentColor":
                var storedDefaultAccentColor = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyDefaultAccentColor);

                if (string.IsNullOrEmpty(storedDefaultAccentColor))
                {
                    CustomAccentColor = false;
                }
                else
                {
                    CustomAccentColor = Convert.ToBoolean(storedDefaultAccentColor);
                }

                break;
            default:
                return;
        }
    }

    private async Task SaveSettingsAsync(string key, bool value)
    {
        switch (key)
        {
            case "Adminmode":
                await _localSettingsService.SaveSettingAsync(SettingsKeyAdminmode, value.ToString());
                break;
            case "AccentColor":
                await _localSettingsService.SaveSettingAsync(SettingsKeyDefaultAccentColor, value.ToString());
                break;
            default:
                return;
        }
    }
}





