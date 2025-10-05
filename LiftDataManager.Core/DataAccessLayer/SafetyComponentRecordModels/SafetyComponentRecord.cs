namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

public class SafetyComponentRecord : BaseEntity
{
    public bool IncompleteRecord { get; set; }
    public bool SchindlerCertified { get; set; }
}
