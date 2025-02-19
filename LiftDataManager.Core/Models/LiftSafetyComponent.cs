namespace LiftDataManager.Core.Models;

public class LiftSafetyComponent(string safetyType, string manufacturer, string model, string certificateNumber, string safetyComponentTyp)
{
    public string SafetyType { get; set; } = safetyType;
    public string Manufacturer { get; set; } = manufacturer;
    public string Model { get; set; } = model;
    public string CertificateNumber { get; set; } = certificateNumber;
    public string SafetyComponentTyp { get; set; } = safetyComponentTyp;
    public string? SpecialOption { get; set; }
}
