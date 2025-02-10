namespace LiftDataManager.Core.DataAccessLayer.Models;

public class SafetyComponentEntity : SelectionEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
