namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class LiftPositionSystem : SelectionEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
