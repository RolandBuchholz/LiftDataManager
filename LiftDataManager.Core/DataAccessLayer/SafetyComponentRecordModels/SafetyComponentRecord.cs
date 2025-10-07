using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class SafetyComponentRecord : BaseEntity
{
    public bool IncompleteRecord { get; set; }
    public bool SchindlerCertified { get; set; }
    public int LiftCommissionId { get; set; }
    public LiftCommission? LiftCommission { get; set; }
}
