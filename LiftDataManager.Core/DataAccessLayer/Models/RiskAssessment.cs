namespace LiftDataManager.Core.DataAccessLayer.Models;

public class RiskAssessment : BaseEntity
{
    public required string VaultDocument { get; set; }
    public required string Description { get; set; }
}

