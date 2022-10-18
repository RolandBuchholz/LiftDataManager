namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorGroup : BaseEntity
{
    public string? Name { get; set; }
    public string? DoorOpeningType { get; set; }
    public int DoorPanelCount { get; set; }
    public int CarDoorId { get; set; }
    public CarDoor? CarDoor { get; set; }
    public int ShaftDoorId { get; set; }
    public ShaftDoor? ShaftDoor { get; set; }
}
