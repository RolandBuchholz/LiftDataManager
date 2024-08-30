using System.Collections.ObjectModel;
namespace LiftDataManager.Core.Models;
public class HelpContent(string name, string fullPath, string relativePath)
{
    public string Name { get; set; } = name;
    public string FullPath { get; set; } = fullPath;
    public string RelativePath { get; set; } = relativePath;
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
