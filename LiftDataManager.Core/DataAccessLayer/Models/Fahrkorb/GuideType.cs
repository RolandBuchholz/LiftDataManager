namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideType : BaseEntity
{
    public IEnumerable<GuideModelType>? GuideModelTypes { get; set; }
}
