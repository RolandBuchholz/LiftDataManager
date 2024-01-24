namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class ZiehlAbeggDrive : BaseEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}
