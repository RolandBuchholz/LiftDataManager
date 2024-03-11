
namespace LiftDataManager.Core.Models;

public struct InfoCenterEntry
{
    public InfoCenterEntryState State { get; set; }
    public string ParameterName { get; set; }
    public string Message { get; set; }
}
