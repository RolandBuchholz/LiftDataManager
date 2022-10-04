namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class CarFrameType : BaseEntity
{
    public string? Name { get; set; }
    public int CarFrameWeight { get; set; }
    public bool IsCFPControlled { get; set; }
    public bool HasMachineRoom  { get; set; }
    public string? DriveType { get; set; }
    public string? CarFrameBaseType { get; set; }
}
