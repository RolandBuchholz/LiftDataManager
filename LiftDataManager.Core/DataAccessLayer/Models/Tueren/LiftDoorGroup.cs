namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorGroup : BaseEntity
{
    public int DoorPanelCount { get; set; }
    public int CarDoorId { get; set; }
    public CarDoor? CarDoor { get; set; }
    public int ShaftDoorId { get; set; }
    public ShaftDoor? ShaftDoor { get; set; }
    public int LiftDoorOpeningDirectionId { get; set; }
    public LiftDoorOpeningDirection? LiftDoorOpeningDirection { get; set; }
}
