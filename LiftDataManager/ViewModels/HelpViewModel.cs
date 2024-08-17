using System.Collections.ObjectModel;
namespace LiftDataManager.ViewModels;

public partial class HelpViewModel : ObservableRecipient, INavigationAwareEx
{
    public ObservableCollection<HelpContent>? HelpTreeDataSource;
    public HelpViewModel()
    {
    }
    [ObservableProperty]
    private HelpContent? selectedHelpContent;
    partial void OnSelectedHelpContentChanged(HelpContent? value) 
    {
    
    }

    private ObservableCollection<HelpContent> GetData()
    {
        var helpfilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Docs","de");
        var helpTreeDatalist = new ObservableCollection<HelpContent>();
        if (string.IsNullOrWhiteSpace(helpfilesPath))
        {
            return helpTreeDatalist;
        }
        if (!Directory.Exists(helpfilesPath))
        {
            return helpTreeDatalist;
        }
        foreach (var file in Directory.GetDirectories(helpfilesPath))
        {
            var newHelpContentEntry = new HelpContent(TreeViewEntryName(file)) 
            { 
                Level = HelpContentLevel.Main,
            };
            foreach (var subfile in Directory.GetDirectories(file))
            {
                var newChildren = new HelpContent(TreeViewEntryName(subfile)) 
                {
                    Level = HelpContentLevel.Sub,
                };

                foreach (var sub2file in Directory.GetDirectories(subfile))
                {
                    var newChildren2 = new HelpContent(TreeViewEntryName(sub2file)) 
                    {
                        Level = HelpContentLevel.Sub2,
                    };
                    newChildren.Children.Add(newChildren2);
                }
                newHelpContentEntry.Children.Add(newChildren);
            }
            helpTreeDatalist.Add(newHelpContentEntry);
        }
        return helpTreeDatalist;
    }

    private static string TreeViewEntryName(string? folderNamePath)
    {
        if (string.IsNullOrWhiteSpace(folderNamePath))
        {
            return string.Empty;
        }
        return Path.GetFileName(folderNamePath)[3..];
    }

    public void OnNavigatedTo(object parameter) 
    {
        HelpTreeDataSource = GetData();
        if (parameter is DataItem)
        {
            SelectedHelpContent = HelpTreeDataSource[0];
        }
    }
    public void OnNavigatedFrom()
    {
    }
}
