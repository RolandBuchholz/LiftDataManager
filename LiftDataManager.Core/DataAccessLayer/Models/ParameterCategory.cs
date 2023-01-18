namespace LiftDataManager.Core.DataAccessLayer.Models;
public class ParameterCategory : BaseEntity
{
    public IEnumerable<ParameterDto>? ParameterDtos { get; set; }
}
