namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
public class ZipCode : BaseEntity
{
    public IEnumerable<LiftPlanner>? LiftPlanners { get; set; }
    public required int ZipCodeNumber { get; set; }
    public int CountryId { get; set; }
    public required Country Country { get; set; }
}
