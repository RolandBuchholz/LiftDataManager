namespace LiftDataManager.Core.Contracts.Services;
public interface IVaultDataService
{
    Task<DownloadInfo> GetFileAsync(string auftragsnummer, bool readOnly = false, bool customFile = false);

    Task<DownloadInfo> SetFileAsync(string auftragsnummer, bool customFile = false);

    Task<DownloadInfo> UndoFileAsync(string auftragsnummer, bool customFile = false);
}
