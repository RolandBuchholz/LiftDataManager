namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class CarDoor : BaseEntity
{
    public string? Manufacturer { get; set; }
    public double SillWidth { get; set; }
    public double MinimalMountingSpace { get; set; }
    public double DoorPanelWidth { get; set; }
    public double DoorPanelSpace { get; set; }
    public int DoorPanelCount { get; set; }
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
    public IEnumerable<LiftDoorGroup>? LiftDoorGroups { get; set; }
    public int LiftDoorOpeningDirectionId { get; set; }
    public LiftDoorOpeningDirection? LiftDoorOpeningDirection { get; set; }
}
