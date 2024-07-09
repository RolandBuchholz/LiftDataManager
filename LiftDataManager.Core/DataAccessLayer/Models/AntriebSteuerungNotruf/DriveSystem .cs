namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystem : SelectionEntity
{
    public DriveSystemType? DriveSystemType { get; set; }
    public int DriveSystemTypeId { get; set; }
    public string? DriveControlTyp { get; set; }
}
