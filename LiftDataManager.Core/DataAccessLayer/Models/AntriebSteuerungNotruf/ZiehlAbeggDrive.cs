namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class ZiehlAbeggDrive : SelectionEntity
{
    public DriveSafetyBrake? DriveSafetyBrake { get; set; }
    public int DriveSafetyBrakeId { get; set; }
}
