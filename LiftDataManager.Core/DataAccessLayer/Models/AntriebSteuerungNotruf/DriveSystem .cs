namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystem : BaseEntity
{
    public DriveSystemType? DriveSystemType { get; set; }
    public int DriveSystemTypeId { get; set; }
}
