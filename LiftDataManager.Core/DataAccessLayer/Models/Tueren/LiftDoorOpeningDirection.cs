namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorOpeningDirection : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<LiftDoorGroup>? LiftDoorGroups { get; set; }
}
