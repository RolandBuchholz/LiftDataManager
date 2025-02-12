namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class ShaftDoor : SafetyComponentEntity
{
    public string? Manufacturer { get; set; }
    public int DoorPanelCount { get; set; }
    public double SillWidth { get; set; }
    public double DoorPanelWidth { get; set; }
    public double DoorPanelSpace { get; set; }
    public IEnumerable<LiftDoorGroup>? LiftDoorGroups { get; set; }
    public int LiftDoorOpeningDirectionId { get; set; }
    public LiftDoorOpeningDirection? LiftDoorOpeningDirection { get; set; }
    public double DefaultFrameWidth { get; set; }
    public double DefaultFrameDepth { get; set; }
}