namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class SafetyComponentManufacturer : BaseEntity
{
    public int ZIPCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public IEnumerable<SafetyComponentRecord>? SafetyComponentRecords { get; set; }
}
