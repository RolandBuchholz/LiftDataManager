namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class CarFrameBaseType : SelectionEntity
{
    public IEnumerable<CarFrameType>? CarFrameTypes { get; set; }
}
