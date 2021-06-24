using System.Threading.Tasks;

namespace SpeziInspector.Contracts.Services
{
    public interface ISettingService
    {
        public bool Adminmode { get; set; }

        Task InitializeAsync();

        Task SetSettingsAsync(bool setting);
    }
}
