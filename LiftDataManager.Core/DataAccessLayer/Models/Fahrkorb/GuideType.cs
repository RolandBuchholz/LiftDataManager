namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideType : SelectionEntity
{
    public IEnumerable<GuideModelType>? GuideModelTypes { get; set; }
}
