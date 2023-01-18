namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class OverspeedGovernor : BaseEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
