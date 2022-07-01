using System.Threading.Tasks;
using LiftDataManager.Core.Models;

namespace LiftDataManager.Core.Contracts.Services;
public interface IVaultDataService
{
    Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly);
    Task<DownloadInfo> SetFileAsync(string auftragsnummer);
    Task<DownloadInfo> UndoFileAsync(string auftragsnummer);
}
