namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<SafetyGearModelType>? SafetyGearModelTypes { get; set; }
}
