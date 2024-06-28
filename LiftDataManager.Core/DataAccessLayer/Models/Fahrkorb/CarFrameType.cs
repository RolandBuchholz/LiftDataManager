namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class CarFrameType : BaseEntity
{
    public int CarFrameWeight { get; set; }
    public bool IsCFPControlled { get; set; }
    public bool HasMachineRoom { get; set; }
    public int DriveTypeId { get; set; }
    public DriveType? DriveType { get; set; }
    public int CarFrameBaseTypeId { get; set; }
    public CarFrameBaseType? CarFrameBaseType { get; set; }
    public int CFPStartIndex { get; set; }
    public int CarFrameDGB { get; set; }
    public int CounterweightDGB { get; set; }
    public int CarFrameDGBOffset { get; set; }
    public int CarFrametoCWTDGBOffset { get; set; }
    public int CounterweightDGBOffset { get; set; }
}
