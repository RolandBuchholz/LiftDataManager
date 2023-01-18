
namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystemType : BaseEntity
{
    public IEnumerable<DriveSystem>? DriveSystems { get; set; }
}