namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearModelType : BaseEntity
{
    public string? Name { get; set; }
    public int SafetyGearTypeId { get; set; }
    public SafetyGearType? SafetyGearType { get; set; }
}
