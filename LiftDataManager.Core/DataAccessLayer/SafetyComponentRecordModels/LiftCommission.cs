namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class LiftCommission : BaseEntity
{
    public string? SAISEquipment { get; set; }
    public IEnumerable<SafetyComponentRecord>? SafetyComponentRecords { get; set; }
}
