namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class LiftType : BaseEntity
{
    public string? Name { get; set; }
    public string? DriveType { get; set; }
    public string? CargoType { get; set; }
}
