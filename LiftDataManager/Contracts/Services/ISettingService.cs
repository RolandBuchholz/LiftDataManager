using System.Threading.Tasks;

namespace LiftDataManager.Contracts.Services;

public interface ISettingService
{
    public bool Adminmode
    {
        get; set;
    }
    Task InitializeAsync();
    Task SetSettingsAsync(bool setting);
}
