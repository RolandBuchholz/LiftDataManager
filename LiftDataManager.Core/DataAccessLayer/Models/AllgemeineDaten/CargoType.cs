namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class CargoType : BaseEntity
{
    public IEnumerable<LiftType>? LiftTypes { get; set; }
}
