namespace LiftDataManager.Core.DataAccessLayer.Models;

public class ParameterDto : BaseEntity
{
    public int ParameterCategoryId { get; set; }
    public required ParameterCategory ParameterCategory { get; set; }
    public int ParameterTypId { get; set; }
    public required ParameterTyp ParameterTyp { get; set; }
    public int ParameterTypeCodeId { get; set; }
    public required ParameterTypeCode ParameterTypeCode { get; set; }
    public required string DisplayName { get; set; }
    public string? Abbreviation { get; set; }
    public string? Value { get; set; }
    public string? Comment;
    public bool IsKey;
    public bool DefaultUserEditable { get; set; }
    public bool CarDesignRelated { get; set; }
    public bool DispoPlanRelated { get; set; }
    public bool LiftPanelRelated { get; set; }
    public string? DropdownList { get; set; }
}
