namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class SafetyGearModelType : BaseEntity
{
    public int SafetyGearTypeId { get; set; }
    public SafetyGearType? SafetyGearType { get; set; }
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
