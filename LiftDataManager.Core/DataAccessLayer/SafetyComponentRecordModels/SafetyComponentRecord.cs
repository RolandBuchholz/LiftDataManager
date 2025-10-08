namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class SafetyComponentRecord : BaseEntity
{
    public int LiftCommissionId { get; set; }
    public LiftCommission? LiftCommission { get; set; }
    public bool IncompleteRecord { get; set; }
    public bool SchindlerCertified { get; set; }
    public int Release { get; set; }
    public int Revision { get; set; }
    public int IdentificationNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? BatchNumber { get; set; }
    public int SafetyComponentManfacturerId { get; set; }
    public SafetyComponentManfacturer? SafetyComponentManfacturer { get; set; }
}
