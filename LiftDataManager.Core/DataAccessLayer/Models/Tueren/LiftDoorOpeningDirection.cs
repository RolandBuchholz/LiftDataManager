namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorOpeningDirection : SelectionEntity
{
    public IEnumerable<ShaftDoor>? ShaftDoors { get; set; }
    public IEnumerable<CarDoor>? CarDoors { get; set; }
}
