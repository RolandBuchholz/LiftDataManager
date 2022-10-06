namespace LiftDataManager.Core.DataAccessLayer.Models;

public class ParameterTypeCode : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<ParameterDto>? ParameterDtos { get; set; }
}
