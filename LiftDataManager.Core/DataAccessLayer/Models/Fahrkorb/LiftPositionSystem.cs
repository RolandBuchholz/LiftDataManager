namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class LiftPositionSystem : BaseEntity
{
    public string? Name { get; set; }
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
