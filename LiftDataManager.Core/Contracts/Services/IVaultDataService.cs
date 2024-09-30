namespace LiftDataManager.Core.Contracts.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles Autodesk Vault APIs.
/// </summary>
public interface IVaultDataService
{
    /// <summary>
    /// Checkout a file from the AutodeskVaultServer
    /// </summary>
    /// <param name="liftNumber">Number of the lift or customFile</param>
    /// <param name="readOnly">get file readonly</param>
    /// <param name="customFile">ia a customFile</param>
    /// <returns>Task<DownloadInfo></returns>
    Task<DownloadInfo> GetFileAsync(string liftNumber, bool readOnly = false, bool customFile = false);

    /// <summary>
    /// Checkin a file on the AutodeskVaultServer
    /// </summary>
    /// <param name="liftNumber">Number of the lift or customFile</param>
    /// <param name="customFile">ia a customFile</param>
    /// <returns>Task<DownloadInfo></returns>
    Task<DownloadInfo> SetFileAsync(string liftNumber, bool customFile = false);

    /// <summary>
    /// Undo a file from the AutodeskVaultServer
    /// </summary>
    /// <param name="liftNumber">Number of the lift or customFile</param>
    /// <param name="customFile">ia a customFile</param>
    /// <returns>Task<DownloadInfo></returns>
    Task<DownloadInfo> UndoFileAsync(string liftNumber, bool customFile = false);

    /// <summary>
    /// Search in the lokal workspace
    /// </summary>
    /// <param name="liftNumber">Number of the lift or customFile</param>
    /// <param name="spezifikationTyp">ia a customFile</param>
    /// <param name="readOnly">get file readonly</param>
    /// <returns>Task< (long, DownloadInfo)></returns>
    Task<(long, DownloadInfo?)> GetAutoDeskTransferAsync(string liftNumber, SpezifikationTyp spezifikationTyp, bool readOnly = true);
}