namespace LiftDataManager.Core.Contracts.Services;
public interface IVaultDataService
{
    Task<DownloadInfo> GetFileAsync(string liftNumber, bool readOnly = false, bool customFile = false);

    Task<DownloadInfo> SetFileAsync(string liftNumber, bool customFile = false);

    Task<DownloadInfo> UndoFileAsync(string liftNumber, bool customFile = false);

    Task<DownloadInfo> GetAutoDeskTransferAsync(string liftNumber, SpezifikationTyp spezifikationTyp, bool readOnly = true);
}
