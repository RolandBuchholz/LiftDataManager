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
    /// <param name="parameterName">ParameterName.</param>
    /// <param name="oldValue">Old Value</param>
    /// <param name="newValue">New Value</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterParameterChangedAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string parameterName, string oldValue, string newValue)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterParameterChanged)
        { 
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
    /// <param name="savedParameter">KeyValuePair*ParameterName-New Value*</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterSaveInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, KeyValuePair<string, string?> savedParameter)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
        {
            ParameterName = savedParameter.Key,
            NewValue = savedParameter.Value
        });
        await Task.CompletedTask;
    }

    /// <summary>
    /// add a parameter changed info to the infoCenter
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys</param>
    /// <param name="savedParameters">IEnumerable*KeyValuePair*ParameterName-New Value*</param>
    /// <returns>Task</returns>
    public async Task AddInfoCenterSaveAllInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<KeyValuePair<string, string?>> savedParameters)
    {
        List<InfoCenterEntry> newEntrys = new();
        foreach (var savedParameter in savedParameters)
        {
            newEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
            {
                ParameterName = savedParameter.Key,
                NewValue = savedParameter.Value
            });
        }
        infoCenterEntrys.AddRange(newEntrys);
        await Task.CompletedTask;
    }
}
