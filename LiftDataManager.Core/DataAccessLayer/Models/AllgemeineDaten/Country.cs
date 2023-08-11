namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
public class Country : BaseEntity
{
    public IEnumerable<ZipCode>? ZipCodes { get; set; }
    public string? ShortMark { get; set; }
}
