using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Models;

public class DriveType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<LiftType>? LiftTypes { get; set; }
    public IEnumerable<CarFrameType>? CarFrameTypes { get; set; }
}
