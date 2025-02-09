
namespace LiftDataManager.Core.Models;

public struct InfoCenterEntry(InfoCenterEntryState state)
{
    public InfoCenterEntryState State { get; set; } = state;
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    public string? ParameterName { get; set; }
    public string? UniqueName { get; set; }
    public string? NewValue { get; set; }
    public string? OldValue { get; set; }
    public string? Message { get; set; }
}
