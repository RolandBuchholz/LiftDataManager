﻿
namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class DriveSystemType : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<DriveSystem>? DriveSystems { get; set; }
}