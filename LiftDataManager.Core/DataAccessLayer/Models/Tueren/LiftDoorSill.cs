﻿namespace LiftDataManager.Core.DataAccessLayer.Models.Tueren;

public class LiftDoorSill : SelectionEntity
{
    public string? Manufacturer { get; set; }
    public int SillMountTyp { get; set; }
    public string? SillFilterTyp { get; set; }
    public bool IsVandalResistant { get; set; }
}
