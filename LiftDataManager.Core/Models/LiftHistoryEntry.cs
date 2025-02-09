using System.Text.Json.Serialization;

namespace LiftDataManager.Core.Models;

public class LiftHistoryEntry(string name, string displayName, string newValue, string author, string comment, ParameterCategoryValue? category = default)
{
    public string Name { get; set; } = name;
    public string DisplayName { get; set; } = displayName;
    public string? NewValue { get; set; } = newValue;
    public string Author { get; set; } = author;
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    public string? Comment { get; set; } = comment;
    [JsonIgnore]
    public ParameterCategoryValue? Category { get; set; } = category;
}
