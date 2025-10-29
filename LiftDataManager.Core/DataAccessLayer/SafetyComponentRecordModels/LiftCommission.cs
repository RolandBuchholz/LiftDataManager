namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class LiftCommission : BaseEntity
{
    public int LiftInstallerID { get; set; } = 1491;
    public string? SAISEquipment { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public int ZIPCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "DE";
    public IEnumerable<SafetyComponentRecord>? SafetyComponentRecords { get; set; }
}
