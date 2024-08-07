﻿namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class CarFlooring : SelectionEntity
{
    public double? Thickness { get; set; }
    public double? WeightPerSquareMeter { get; set; }
    public bool SpecialSheet { get; set; }
    public IEnumerable<CarFloorColorTyp>? CarFloorColorTyps { get; set; }
}
