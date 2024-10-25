using MvvmHelpers;

namespace LiftDataManager.Core.Contracts.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles infos in the infoCenters.
/// </summary>
public interface IInfoCenterService
{
    /// <summary>
    /// get infoCenterEntrys
    /// </summary>
    /// <returns>Task ObservableRangeCollection<InfoCenterEntry></returns>
    ObservableRangeCollection<InfoCenterEntry> GetInfoCenterEntrys();

    /// <summary>
    /// add a message to the infoCenter
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>Task</returns>
    Task AddInfoCenterMessageAsync(string message);

    /// <summary>
    /// add a warning to the infoCenter
    /// </summary>
    /// <param name="message">The warning</param>
    /// <returns>Task</returns>
    Task AddInfoCenterWarningAsync(string message);

    /// <summary>
    /// add a error to the infoCenter
    /// </summary>
    /// <param name="message">The error</param>
    /// <returns>Task</returns>
    Task AddInfoCenterErrorAsync(string error);

    /// <summary>
    /// add a parameter changed info to the infoCenter
    /// </summary>
    /// <param name="uniqueName">ParameterName.</param>
    /// <param name="parameterName">DisplayName.</param>
    /// <param name="oldValue">Old Value</param>
    /// <param name="newValue">New Value</param>
    /// <param name="autoUpdated">LDM updated</param>
    /// <returns>Task</returns>
    Task AddInfoCenterParameterChangedAsync(string uniqueName, string parameterName, string oldValue, string newValue, bool autoUpdated);

    /// <summary>
    /// add a saved parameter to the infoCenter
    /// </summary>
    /// <param name="savedParameter">*Tuple*ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    Task AddInfoCenterSaveInfoAsync(Tuple<string, string, string?> savedParameter);

    /// <summary>
    /// add a list of saved Parameter to the infoCenter
    /// </summary>
    /// <param name="savedParameters">IEnumerable*Tuple* ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    Task AddInfoCenterSaveAllInfoAsync(IEnumerable<Tuple<string, string, string?>> savedParameters);

    /// <summary>
    /// add a list of saved Parameter to the infoCenter
    /// </summary>
    /// <param name="newInfoCenterEntrys">IEnumerable infoCenterEntrys</param>
    /// <returns>Task</returns>
    Task AddListofInfoCenterEntrysAsync(IEnumerable<InfoCenterEntry> newInfoCenterEntrys);
}
