namespace LiftDataManager.Core.DataAccessLayer.Models;

public class ParameterTyp : BaseEntity
{
    public string? Name { get; set; }
    public IEnumerable<ParameterDto>? ParameterDtos { get; set; }
}
