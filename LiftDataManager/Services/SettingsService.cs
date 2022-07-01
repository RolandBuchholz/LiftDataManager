using System;
using System.Threading.Tasks;
using LiftDataManager.Contracts.Services;

namespace LiftDataManager.Services;

public class SettingsService : ISettingService
{
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }
    public bool Adminmode
    {
        get; set;
    }

    public async Task InitializeAsync()
    {
        await LoadAdminmodeFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetSettingsAsync(bool currentAdminmode)
    {
        Adminmode = currentAdminmode;

        await SaveAdminmodeInSettingsAsync(Adminmode);
    }

    private async Task LoadAdminmodeFromSettingsAsync()
    {
        var storedAdminmode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAdminmode);

        if (string.IsNullOrEmpty(storedAdminmode))
        {
            Adminmode = false;
        }
        else
        {
            Adminmode = Convert.ToBoolean(storedAdminmode);
        }
    }

    private async Task SaveAdminmodeInSettingsAsync(bool currentAdminmode)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKeyAdminmode, currentAdminmode.ToString());
    }
}

