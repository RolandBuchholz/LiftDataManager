namespace LiftDataManager.Core.DataAccessLayer.Models;

public class SafetyComponentEntity : SelectionEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public string? SAISDescription { get; set; }
    public string? SAISIdentificationNumber { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
