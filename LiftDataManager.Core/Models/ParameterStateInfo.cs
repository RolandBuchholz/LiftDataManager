namespace LiftDataManager.Core.Models;

public class ParameterStateInfo
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public ErrorLevel Severity { get; set; }
    public bool HasDependentParameters => DependentParameter.Any();
    public string[] DependentParameter { get; set; } = Array.Empty<string>();

    public ParameterStateInfo(string name, string displayname, bool isvalid)
    {
        Name = name;
        DisplayName = displayname;
        IsValid = isvalid;
    }

    public ParameterStateInfo(string name, string displayname, string message, ErrorLevel level, bool isvalid = false)
    {
        Name = name;
        DisplayName = displayname;
        ErrorMessage = message;
        Severity = level;
        IsValid = isvalid;
    }
}
