namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideRails : BaseEntity
{
    public string? Name { get; set; }
    public bool UsageAsCarRail { get; set; }
    public bool UsageAsCwtRail { get; set; }
}
