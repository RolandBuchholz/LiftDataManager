using LiftDataManager.Core.Models;
using System.Threading.Tasks;

namespace LiftDataManager.Core.Contracts.Services
{
    public interface IVaultDataService
    {
        Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly);
        Task<DownloadInfo> SetFileAsync(string auftragsnummer);
        Task<DownloadInfo> UndoFileAsync(string auftragsnummer);
    }
}
