namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class LiftType : SelectionEntity
{
    public int DriveTypeId { get; set; }
    public DriveType? DriveType { get; set; }
    public int CargoTypeId { get; set; }
    public CargoType? CargoType { get; set; }
}
