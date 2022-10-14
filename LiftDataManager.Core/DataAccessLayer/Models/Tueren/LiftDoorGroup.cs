namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorGroup : BaseEntity
{
    public string? Name { get; set; }
    public string? DoorOpeningType { get; set; }
    public int DoorPanelCount { get; set; }
}
