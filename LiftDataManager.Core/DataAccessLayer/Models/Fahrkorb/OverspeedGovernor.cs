namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class OverspeedGovernor : BaseEntity
{
    public string? Name { get; set; }
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
