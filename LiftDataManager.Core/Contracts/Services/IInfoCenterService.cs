using System.Collections.ObjectModel;

namespace LiftDataManager.Core.Contracts.Services;

public interface IInfoCenterService
{
    Task AddInfoCenterMessageAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string message);
    Task AddInfoCenterWarningAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string message);
    Task AddInfoCenterErrorAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string error);
    Task AddInfoCenterParameterChangedAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, string newValue, string oldValue);
    Task AddInfoCenterSaveInfoAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, KeyValuePair<string, string?> savedParameter);
    Task AddInfoCenterSaveAllInfoAsync(ObservableCollection<InfoCenterEntry> infoCenterEntrys, IEnumerable<KeyValuePair<string, string?>> savedParameters);
}
