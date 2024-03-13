
namespace LiftDataManager.Core.Models;

public struct InfoCenterEntry
{
    public InfoCenterEntry(InfoCenterEntryState state)
    {
        State = state;
        TimeStamp = DateTime.Now;
    }

    public InfoCenterEntryState State { get; set; }
    public DateTime TimeStamp { get; set; }
    public  string? ParameterName { get; set; }
    public string? NewValue { get; set; }
    public string? OldValue { get; set; }
    public string? Message { get; set; }
}
