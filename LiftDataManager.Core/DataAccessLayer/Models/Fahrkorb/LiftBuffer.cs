namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class LiftBuffer : BaseEntity
{
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
    public required string Manufacturer { get; set; }
    public int Diameter { get; set; }
    public int Height { get; set; }
    public int MinLoad063 { get; set; }
    public int MaxLoad063 { get; set; }
    public int MinLoad100 { get; set; }
    public int MaxLoad100 { get; set; }
    public int MinLoad130 { get; set; }
    public int MaxLoad130 { get; set; }
    public int MinLoad160 { get; set; }
    public int MaxLoad160 { get; set; }
    public int MinLoad200 { get; set; }
    public int MaxLoad200 { get; set; }
    public int MinLoad250 { get; set; }
    public int MaxLoad250 { get; set; }
}
