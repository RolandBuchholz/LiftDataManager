namespace LiftDataManager.Core.DataAccessLayer.Models;
public class SafetyComponentTyp : BaseEntity
{
    public IEnumerable<TypeExaminationCertificate>? TypeExaminationCertificates { get; set; }
}
