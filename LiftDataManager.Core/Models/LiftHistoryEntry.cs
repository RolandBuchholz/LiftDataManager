namespace LiftDataManager.Core.Models;

internal class LiftHistoryEntry
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string? NewValue { get; set; }
    public string Author { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? Comment { get; set; }

    public LiftHistoryEntry(string name, string displayName , string newValue, string author, string comment)
    {
        TimeStamp = DateTime.Now;
        Name = name;
        DisplayName = displayName;
        NewValue = newValue;
        Author = author;
        Comment = comment;
    }
}
