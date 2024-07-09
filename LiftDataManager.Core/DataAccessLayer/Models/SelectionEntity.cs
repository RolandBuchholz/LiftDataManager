namespace LiftDataManager.Core.DataAccessLayer.Models;

public class SelectionEntity : BaseEntity
{
    public required string DisplayName { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsObsolete { get; set; }
    public bool SchindlerCertified { get; set; }
    public int OrderSelection { get; set; }
}
