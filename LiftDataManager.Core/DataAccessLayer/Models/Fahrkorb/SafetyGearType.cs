namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearType : SelectionEntity
{
    public IEnumerable<SafetyGearModelType>? SafetyGearModelTypes { get; set; }
}
