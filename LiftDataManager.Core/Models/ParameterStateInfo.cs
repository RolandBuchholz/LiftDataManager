namespace LiftDataManager.Core.Models;

public class ParameterStateInfo
{
    public enum ErrorLevel
    {
        Informational,
        Warning,
        Error
    }

    public string Name { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public ErrorLevel Severity {get; set;}
    public bool HasDependentParameters => DependentParameter.Any();
    public string[] DependentParameter { get; set; } = Array.Empty<string>();

    public ParameterStateInfo(string name, bool isvalid)
    {
        Name = name;
        IsValid = isvalid;
    }

    public ParameterStateInfo(string name, string message, ErrorLevel level, bool isvalid = false)
    {
        Name = name;
        ErrorMessage = message;
        Severity = level;
        IsValid = isvalid;
    }
}
