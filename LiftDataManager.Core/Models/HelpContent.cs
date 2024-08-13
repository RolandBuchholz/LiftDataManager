using System.Collections.ObjectModel;
namespace LiftDataManager.Core.Models;
public class HelpContent
{
    public string Name { get; set; }

    public HelpContentLevel Level { get; set; }
    public HelpContent(string name)
    {
        Name = name;
    }
    private ObservableCollection<HelpContent>? children;
    public ObservableCollection<HelpContent> Children
    {
        get
        {
            children ??= [];
            return children;
        }
        set
        {
            children = value;
        }
    }
}
