using System.Collections.ObjectModel;
namespace LiftDataManager.ViewModels;

public partial class HelpViewModel : ObservableRecipient, INavigationAwareEx
{
    public ObservableCollection<HelpContent>? HelpTreeDataSource;
    public HelpViewModel()
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
            var name = Path.GetFileName(file)[3..];
            var newHelpContentEntry = new HelpContent(name) 
            { 
                Level = HelpContentLevel.Main,
            };

            foreach (var subfile in Directory.GetDirectories(file))
            {
                var subname = Path.GetFileName(subfile)[3..];
                var newChildren = new HelpContent(subname) 
                {
                    Level = HelpContentLevel.Sub,
                };

                foreach (var sub2file in Directory.GetDirectories(subfile))
                {
                    var sub2name = Path.GetFileName(sub2file)[3..];
                    var newChildren2 = new HelpContent(sub2name) 
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

    public void OnNavigatedTo(object parameter) 
    {
        HelpTreeDataSource = GetData();
    }
    public void OnNavigatedFrom()
    {
    }
}
