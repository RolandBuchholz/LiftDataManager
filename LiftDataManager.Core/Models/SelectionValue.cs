namespace LiftDataManager.Core.Models;
public class SelectionValue
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public bool IsFavorite { get; set; }
    public bool SchindlerCertified { get; set; }
    public int OrderSelection { get; set; }
}
