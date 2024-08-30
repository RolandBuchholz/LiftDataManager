using MvvmHelpers;

namespace LiftDataManager.Core.Contracts.Services;

public interface IInfoCenterService
{
    Task AddInfoCenterMessageAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message);

    Task AddInfoCenterWarningAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string message);

    Task AddInfoCenterErrorAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string error);

    Task AddInfoCenterParameterChangedAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, string uniqueName, string parameterName, string oldValue, string newValue, bool autoUpdated);

    Task AddInfoCenterSaveInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, Tuple<string, string, string?> savedParameter);

    Task AddInfoCenterSaveAllInfoAsync(ObservableRangeCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<Tuple<string, string, string?>> savedParameters);
}
