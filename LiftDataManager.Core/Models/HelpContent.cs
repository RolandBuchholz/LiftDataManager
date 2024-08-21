using System.Collections.ObjectModel;
namespace LiftDataManager.Core.Models;
public class HelpContent(string name, string folderPath)
{
    public string Name { get; set; } = name;
    public string FolderPath { get; set; } = folderPath;

    public HelpContentLevel Level { get; set; }

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
