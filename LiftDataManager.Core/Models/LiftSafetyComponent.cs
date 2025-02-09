namespace LiftDataManager.Core.Models;

public class LiftSafetyComponent(string safetyType, string manufacturer, string model, string certificateNumber)
{
    public string SafetyType { get; set; } = safetyType;
    public string Manufacturer { get; set; } = manufacturer;
    public string Model { get; set; } = model;
    public string CertificateNumber { get; set; } = certificateNumber;
}
