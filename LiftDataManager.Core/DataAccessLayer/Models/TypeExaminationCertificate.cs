namespace LiftDataManager.Core.DataAccessLayer.Models;

public class TypeExaminationCertificate : BaseEntity
{
    public string? CertificateNumber { get; set; }
    public string? ManufacturerName { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}
