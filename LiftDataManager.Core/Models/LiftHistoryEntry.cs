using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace LiftDataManager.Core.Models;

public partial class LiftHistoryEntry(string name, string displayName, string newValue, string author, string comment, ParameterCategoryValue? category = default) : ObservableObject
{
    public string Name { get; set; } = name;
    public string DisplayName { get; set; } = displayName;
    public string? NewValue { get; set; } = newValue;
    public string Author { get; set; } = author;
    public DateTime TimeStamp { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial string? Comment { get; set; } = comment;
    [JsonIgnore]
    public ParameterCategoryValue? Category { get; set; } = category;
    [JsonIgnore]
    public bool CarDesignRelated { get; set; }
    [JsonIgnore]
    public bool DispoPlanRelated { get; set; }
    [JsonIgnore]
    public bool LiftPanelRelated { get; set; }
}
