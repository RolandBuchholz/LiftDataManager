using MvvmHelpers;

namespace LiftDataManager.Core.Contracts.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles infos in the infoCenters.
/// </summary>
public interface IInfoCenterService
{
    /// <summary>
    /// add a message to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The message</param>
    /// <returns>Task</returns>
    Task AddInfoCenterMessageAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message);

    /// <summary>
    /// add a warning to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The warning</param>
    /// <returns>Task</returns>
    Task AddInfoCenterWarningAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message);

    /// <summary>
    /// add a error to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The error</param>
    /// <returns>Task</returns>
    Task AddInfoCenterErrorAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string error);

    /// <summary>
    /// add a parameter changed info to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="uniqueName">ParameterName.</param>
    /// <param name="parameterName">DisplayName.</param>
    /// <param name="oldValue">Old Value</param>
    /// <param name="newValue">New Value</param>
    /// <param name="autoUpdated">LDM updated</param>
    /// <returns>Task</returns>
    Task AddInfoCenterParameterChangedAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string uniqueName, string parameterName, string oldValue, string newValue, bool autoUpdated);

    /// <summary>
    /// add a saved parameter to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="savedParameter">*Tuple*ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    Task AddInfoCenterSaveInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, Tuple<string, string, string?> savedParameter);

    /// <summary>
    /// add a list of saved Parameter to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="savedParameters">IEnumerable*Tuple* ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    Task AddInfoCenterSaveAllInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<Tuple<string, string, string?>> savedParameters);

    /// <summary>
    /// add a list of saved Parameter to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="newInfoCenterEntrys">IEnumerable infoCenterEntrys</param>
    /// <returns>Task</returns>
    Task AddListofInfoCenterEntrysAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<InfoCenterEntry> newInfoCenterEntrys);
}
