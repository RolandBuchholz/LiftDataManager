namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class LiftType : BaseEntity
{
    public string? Name { get; set; }
    public int DriveTypeId { get; set; }
    public DriveType? DriveType { get; set; }
    public int CargoTypeId { get; set; }
    public CargoType? CargoType { get; set; }
}
