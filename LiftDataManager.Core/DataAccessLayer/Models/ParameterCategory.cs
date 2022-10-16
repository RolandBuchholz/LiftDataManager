﻿namespace LiftDataManager.Core.DataAccessLayer.Models;
public class ParameterCategory : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<ParameterDto>? ParameterDtos { get; set; }
}