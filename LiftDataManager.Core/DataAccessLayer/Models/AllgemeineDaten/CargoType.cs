﻿namespace LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;

public class CargoType : SelectionEntity
{
    public IEnumerable<LiftType>? LiftTypes { get; set; }
}
