namespace LiftDataManager.Core.Models;

public class LiftSafetyComponent
{
    public LiftSafetyComponent(string safetyType, string manufacturer, string model, string certificateNumber)
    {
        SafetyType = safetyType;
        Manufacturer = manufacturer;
        Model = model;
        CertificateNumber = certificateNumber;
    }

    public string SafetyType { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string CertificateNumber { get; set; }
}
