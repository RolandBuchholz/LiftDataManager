using LiftDataManager.Core.Contracts.Services;
using System.Collections.ObjectModel;

namespace LiftDataManager.Core.Services;

public class InfoCenterService : IInfoCenterService
{
    /// <summary>
    /// add a message to the infoCenter.
    /// </summary>
    /// <param name="infoCenterEntrys">ObservableCollection of infoCenterEntrys.</param>
    /// <param name="message">The message.</param>
    /// <returns>Task.</returns>
    public async Task AddInfoCenterMessageAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string message)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        { 
            State =InfoCenterEntryState.InfoCenterMessage, 
            Message = await Task.FromResult(message)
        });
    }

    public async Task AddInfoCenterWarningAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string warning)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        { 
            State=InfoCenterEntryState.InfoCenterWarning,
            Message = await Task.FromResult(warning) 
        });
    }
    public async Task AddInfoCenterErrorAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string error)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        { 
            State = InfoCenterEntryState.InfoCenterError,
            Message = await Task.FromResult(error) 
        });
    }
    public async Task AddInfoCenterParameterChangedAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string newValue, string oldValue)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        { 
            State = InfoCenterEntryState.InfoCenterParameterChanged,
            Message = await Task.FromResult(newValue) 
        });
    }

    public async Task AddInfoCenterSaveInfoAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, KeyValuePair<string, string?> savedParameter)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        { 
            State = InfoCenterEntryState.InfoCenterSaveParameter,
            Message = await Task.FromResult(savedParameter.Key) 
        });
    }
    public async Task AddInfoCenterSaveAllInfoAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<KeyValuePair<string, string?>> savedParameters)
    {
        infoCenterEntrys.Add(new InfoCenterEntry 
        {
            State = InfoCenterEntryState.InfoCenterSaveParameter,
            Message = await Task.FromResult("Hallo") 
        });
    }
}
