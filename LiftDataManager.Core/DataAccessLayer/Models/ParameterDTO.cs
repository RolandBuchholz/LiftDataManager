using static LiftDataManager.Core.Models.ParameterBase;

namespace LiftDataManager.Core.DataAccessLayer.Models;

public class ParameterDTO : BaseEntity
{
    public ParameterTypValue ParameterTyp { get; set; }
    public ParameterCategoryValue ParameterCategory { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Value { get; set; }
    public TypeCodeValue TypeCode { get; set; }
    public string? Comment;
    public bool IsKey;
    public bool DefaultUserEditable { get; set; }
    public List<string> DropDownList { get; } = new();
}
