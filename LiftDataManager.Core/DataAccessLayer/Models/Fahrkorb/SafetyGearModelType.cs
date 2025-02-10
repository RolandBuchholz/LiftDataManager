namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearModelType : SafetyComponentEntity
{
    public int SafetyGearTypeId { get; set; }
    public SafetyGearType? SafetyGearType { get; set; }
    public int MinLoadOiledColddrawn { get; set; }
    public int MaxLoadOiledColddrawn { get; set; }
    public int MinLoadDryColddrawn { get; set; }
    public int MaxLoadDryColddrawn { get; set; }
    public int MinLoadOiledMachined { get; set; }
    public int MaxLoadOiledMachined { get; set; }
    public int MinLoadDryMachined { get; set; }
    public int MaxLoadDryMachined { get; set; }
    public string? AllowableWidth { get; set; }
}
