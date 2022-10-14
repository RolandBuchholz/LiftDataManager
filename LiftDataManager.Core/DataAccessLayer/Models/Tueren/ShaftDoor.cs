﻿namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class ShaftDoor : BaseEntity
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public double SillWidth { get; set; }
    public int DoorPanelCount { get; set; }
    public int TypeExaminationCertificateId { get; set; }
    public TypeExaminationCertificate? TypeExaminationCertificate { get; set; }
}