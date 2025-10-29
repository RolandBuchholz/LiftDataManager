namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class SafetyComponentRecord : BaseEntity
{
    public int LiftCommissionId { get; set; }
    public LiftCommission? LiftCommission { get; set; }
    public bool CompleteRecord { get; set; }
    public bool SchindlerCertified { get; set; }
    public int Release { get; set; }
    public int Revision { get; set; }
    public string? IdentificationNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? BatchNumber { get; set; }
    public string? Imported { get; set; }
    public DateTime CreationDate { get; set; }
    public bool Active { get; set; }
    public int SafetyComponentManfacturerId { get; set; }
    public SafetyComponentManfacturer? SafetyComponentManfacturer { get; set; }
}
