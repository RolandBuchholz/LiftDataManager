namespace LiftDataManager.Core.Models;
public class SelectionValue
{
    public SelectionValue()
    {
        Id = 0;
        Name = "(keine Auswahl)";
        DisplayName = "(keine Auswahl)";
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
}
