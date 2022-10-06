namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class CargoType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<LiftType>? LiftTypes { get; set; }
}
