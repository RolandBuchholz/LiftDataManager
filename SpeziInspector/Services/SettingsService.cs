using SpeziInspector.Contracts.Services;
using SpeziInspector.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpeziInspector.Services
{
    class SettingsService : ISettingService
    {

        private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
        public bool Adminmode { get; set; }

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

            string storedAdminmode = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKeyAdminmode);

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
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKeyAdminmode, currentAdminmode.ToString());
        }
    }

}

