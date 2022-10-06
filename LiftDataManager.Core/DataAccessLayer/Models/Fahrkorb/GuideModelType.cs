namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideModelType : BaseEntity
{
    public string? Name { get; set; }
    public int GuideTypeId { get; set; }
    public GuideType? GuideType { get; set; }
}
