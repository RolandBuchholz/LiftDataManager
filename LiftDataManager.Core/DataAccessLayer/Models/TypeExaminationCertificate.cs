using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Models;

public class TypeExaminationCertificate : BaseEntity
{
    public string? CertificateNumber { get; set; }
    public string? ManufacturerName { get; set; }
    public IEnumerable<SafetyGearModelType>? SafetyGearModelTypes { get; set; }
    public IEnumerable<OverspeedGovernor>? OverspeedGovernors { get; set; }
    public IEnumerable<LiftPositionSystem>? LiftPositionSystems { get; set; }
    public IEnumerable<CarDoor>? CarDoors { get; set; }
    public IEnumerable<ShaftDoor>? ShaftDoors { get; set; }
}
