namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class CarFrameBaseType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<CarFrameType>? CarFrameTypes { get; set; }
}
