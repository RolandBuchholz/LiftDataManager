namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideRails : BaseEntity
{
    public bool UsageAsCarRail { get; set; }
    public bool UsageAsCwtRail { get; set; }
    public bool Machined { get; set; }
    public double RailHead { get; set; }
}
