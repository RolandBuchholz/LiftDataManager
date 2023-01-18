namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearType : BaseEntity
{
    public IEnumerable<SafetyGearModelType>? SafetyGearModelTypes { get; set; }
}
