namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystem : BaseEntity
{
    public string? Name { get; set; }
    public bool IsGearbox { get; set; }
}
