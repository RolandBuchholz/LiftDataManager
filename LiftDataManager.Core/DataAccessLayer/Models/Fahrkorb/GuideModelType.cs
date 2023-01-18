namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideModelType : BaseEntity
{
    public int GuideTypeId { get; set; }
    public GuideType? GuideType { get; set; }
}
