
namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSafetyBrake : SafetyComponentEntity
{
    public IEnumerable<ZiehlAbeggDrive>? ZiehlAbeggDrives { get; set; }
}