using LiftDataManager.Core.Contracts.Services;
using MvvmHelpers;

namespace LiftDataManager.Core.Services;

public class InfoCenterService : IInfoCenterService
{
    /// <summary>
    /// add a message to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The message</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterMessageAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterMessage)
        {
            Message = await Task.FromResult(message)
        });
    }

    /// <summary>
    /// add a warning to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The warning</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterWarningAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string warning)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterWarning)
        {
            Message = await Task.FromResult(warning)
        });
    }

    /// <summary>
    /// add a error to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="message">The error</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterErrorAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string error)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterError)
        {
            Message = await Task.FromResult(error)
        });
    }

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
    public async Task AddInfoCenterParameterChangedAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string uniqueName, string parameterName, string oldValue, string newValue, bool autoUpdated)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(autoUpdated ? InfoCenterEntryState.InfoCenterAutoUpdate : InfoCenterEntryState.InfoCenterParameterChanged)
        {
            UniqueName = uniqueName,
            ParameterName = parameterName,
            OldValue = oldValue,
            NewValue = newValue
        });
        await Task.CompletedTask;
    }

    /// <summary>
    /// add a parameter changed info to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="savedParameter">*Tuple*ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterSaveInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, Tuple<string, string, string?> savedParameter)
    {
        var obsoleteEntrys = infoCenterEntrys.Where(x => x.UniqueName == savedParameter.Item1).ToList();
        infoCenterEntrys.RemoveRange(obsoleteEntrys);
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
        {
            UniqueName = savedParameter.Item1,
            ParameterName = savedParameter.Item2,
            NewValue = savedParameter.Item3
        });
        await Task.CompletedTask;
    }

    /// <summary>
    /// add a parameter changed info to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="savedParameters">IEnumerable*Tuple* ParameterName-DisplayName-New Value</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterSaveAllInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<Tuple<string, string, string?>> savedParameters)
    {
        List<InfoCenterEntry> newEntrys = [];
        List<InfoCenterEntry> obsoleteEntrys = [];

        foreach (var savedParameter in savedParameters)
        {
            obsoleteEntrys.AddRange(infoCenterEntrys.Where(x => x.UniqueName == savedParameter.Item1).ToList());
            newEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
            {
                UniqueName = savedParameter.Item1,
                ParameterName = savedParameter.Item2,
                NewValue = savedParameter.Item3
            });
        }

        infoCenterEntrys.RemoveRange(obsoleteEntrys);
        infoCenterEntrys.AddRange(newEntrys);
        await Task.CompletedTask;
    }
}
