namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystem : BaseEntity
{
    public string? Name { get; set; }
    public DriveSystemType? DriveSystemType { get; set; }
    public int DriveSystemTypeId { get; set; }
}
