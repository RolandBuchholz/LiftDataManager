namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorOpeningDirection : BaseEntity
{
    public IEnumerable<ShaftDoor>? ShaftDoors { get; set; }
}
