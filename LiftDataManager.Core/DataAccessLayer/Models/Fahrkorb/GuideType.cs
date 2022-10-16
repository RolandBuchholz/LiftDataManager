﻿namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<GuideModelType>? GuideModelTypes { get; set; }
}