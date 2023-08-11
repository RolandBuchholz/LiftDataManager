namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
public class LiftPlanner : BaseEntity
{
    public string? FirstName { get; set; }
    public required string Company { get; set; }
    public required string Street { get; set; }
    public string? StreetNumber { get; set; }
    public int ZipCodeId { get; set; }
    public required ZipCode ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public required string EmailAddress { get; set; }
}
