namespace LiftDataManager.Contracts.Services;

public interface ISettingService
{
    public bool Adminmode
    {
        get; set;
    }

    public bool CustomAccentColor
    {
        get; set;
    }

    Task InitializeAsync();
    Task SetSettingsAsync(string key, bool setting);
}
