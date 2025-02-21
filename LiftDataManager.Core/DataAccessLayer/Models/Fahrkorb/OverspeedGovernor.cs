namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class OverspeedGovernor : SafetyComponentEntity
{
    public bool HasUCMPCertification { get; set; }
    public string? ShortName { get; set; }
}
