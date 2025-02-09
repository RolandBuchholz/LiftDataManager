
namespace LiftDataManager.Core.Models;
public class SelectionValue
{
    public SelectionValue()
    {
        Id = 0;
        Name = "(keine Auswahl)";
        DisplayName = "(keine Auswahl)";
        SchindlerCertified = false;
        IsFavorite = false;
        OrderSelection = 0;
    }
    public SelectionValue(int id, string name, string displayName)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool IsFavorite { get; set; }
    public bool SchindlerCertified { get; set; }
    public int OrderSelection { get; set; }

    public override bool Equals(object? obj) => obj is SelectionValue value && Id == value.Id && Name == value.Name && DisplayName == value.DisplayName;
    public override int GetHashCode() => HashCode.Combine(Id, Name, DisplayName);
}
