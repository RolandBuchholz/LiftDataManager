namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class OverspeedGovernor : SelectionEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
