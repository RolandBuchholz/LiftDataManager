using LiftDataManager.Core.Contracts.Services;
using MvvmHelpers;

namespace LiftDataManager.Core.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="IInfoCenterService"/> <see langword="interface"/> add infos to the infoCenter.
/// </summary>
public class InfoCenterService : IInfoCenterService
{
    /// <inheritdoc/>
    public async Task AddInfoCenterMessageAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterMessage)
        {
            Message = await Task.FromResult(message)
        });
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterWarningAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string warning)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterWarning)
        {
            Message = await Task.FromResult(warning)
        });
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterErrorAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string error)
    {
        infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterError)
        {
            Message = await Task.FromResult(error)
        });
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task AddListofInfoCenterEntrysAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<InfoCenterEntry> newInfoCenterEntrys)
    {
        foreach (var newInfoCenterEntry in newInfoCenterEntrys)
        {
            infoCenterEntrys.Add(newInfoCenterEntry);
        }
        await Task.CompletedTask;
    }
}
