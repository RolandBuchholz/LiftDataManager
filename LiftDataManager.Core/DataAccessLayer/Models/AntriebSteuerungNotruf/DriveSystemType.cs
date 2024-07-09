
namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystemType : SelectionEntity
{
    public IEnumerable<DriveSystem>? DriveSystems { get; set; }
}