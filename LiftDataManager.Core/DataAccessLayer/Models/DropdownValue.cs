namespace LiftDataManager.Core.DataAccessLayer.Models;

public class DropdownValue : ViewEntity
{
    public required string Base { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public bool IsFavorite { get; set; }
    public bool SchindlerCertified { get; set; }
    public int OrderSelection { get; set; }
}
