namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class CarFrameBaseType : BaseEntity
{
    public IEnumerable<CarFrameType>? CarFrameTypes { get; set; }
}
