namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class LiftPositionSystem : BaseEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
