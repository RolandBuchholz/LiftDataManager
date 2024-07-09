namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorGroup : SelectionEntity
{
    public int CarDoorId { get; set; }
    public CarDoor? CarDoor { get; set; }
    public int ShaftDoorId { get; set; }
    public ShaftDoor? ShaftDoor { get; set; }
    public string? DoorManufacturer { get; set; }
}
