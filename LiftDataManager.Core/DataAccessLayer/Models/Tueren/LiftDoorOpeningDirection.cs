namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorOpeningDirection : BaseEntity
{
    public IEnumerable<LiftDoorGroup>? LiftDoorGroups { get; set; }
}
