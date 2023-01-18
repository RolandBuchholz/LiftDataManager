namespace LiftDataManager.Core.DataAccessLayer.Models;

public class ParameterDto : BaseEntity
{
    public int ParameterCategoryId { get; set; }
    public ParameterCategory? ParameterCategory { get; set; }
    public int ParameterTypId { get; set; }
    public ParameterTyp? ParameterTyp { get; set; }
    public int ParameterTypeCodeId { get; set; }
    public ParameterTypeCode? ParameterTypeCode { get; set; }
    public string? DisplayName { get; set; }
    public string? Value { get; set; }
    public string? Comment;
    public bool IsKey;
    public bool DefaultUserEditable { get; set; }
    public string? DropdownList { get; set; }
}
