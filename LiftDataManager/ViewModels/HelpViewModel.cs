using LiftDataManager.Controls;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace LiftDataManager.ViewModels;

public partial class HelpViewModel : ObservableRecipient, INavigationAwareEx
{
    public ObservableCollection<HelpContent>? HelpTreeDataSource { get; set; }
    public ObservableCollection<MarkdownTextBlockControl> MarkdownTextBlockList { get; set; }

    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IStorageService _storageService;
    public HelpViewModel(IStorageService storageService)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _storageService = storageService;
        MarkdownTextBlockList ??= [];
    }

    [ObservableProperty]
    private HelpContent? selectedHelpContent;
    partial void OnSelectedHelpContentChanged(HelpContent? value)
    {
        Task.Run(async () => await LoadMarkDownTextAsync(value?.FolderPath));
    }
    private async Task LoadMarkDownTextAsync(string? folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            return;
        }
        var helpFilesFolder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        var helpFiles = await helpFilesFolder.GetFilesAsync();
        _dispatcherQueue.TryEnqueue(() =>
        {
            for (global::System.Int32 i = 0; i < MarkdownTextBlockList.Count; i++)
            {
                MarkdownTextBlockList.RemoveAt(i);
            }
        });
        foreach (var helpFile in helpFiles)
        {
            if (helpFile is null)
            {
                continue;
            }
            var text = await _storageService.ReadStorageFileAsync(helpFile);

            _dispatcherQueue.TryEnqueue(() =>
            {
                var helpTextBlock = new MarkdownTextBlockControl
                {
                    MarkdownText = text
                };
                MarkdownTextBlockList.Add(helpTextBlock);
            });
        }
        await Task.CompletedTask;
    }

    private async Task GetTreeViewEntrysAsync(object parameter)
    {
        var helpfilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Docs", "de");
        if (string.IsNullOrWhiteSpace(helpfilesPath))
        {
            return;
        }
        if (!Directory.Exists(helpfilesPath))
        {
            return;
        }
        var helpTreeDatalist = new ObservableCollection<HelpContent>();
        var mainHelpFolder = await _storageService.GetFoldersAsync(helpfilesPath);
        foreach (var mainItem in mainHelpFolder)
        {
            var newHelpContentEntry = new HelpContent(TreeViewEntryName(mainItem.Name), mainItem.Path)
            {
                Level = HelpContentLevel.Main,
            };

            var subHelpFolder = await mainItem.GetFoldersAsync();
            foreach (var subItem in subHelpFolder)
            {
                var newChildren = new HelpContent(TreeViewEntryName(subItem.Name), subItem.Path)
                {
                    Level = HelpContentLevel.Sub,
                };
                var sub2HelpFolder = await subItem.GetFoldersAsync();
                foreach (var sub2Item in sub2HelpFolder)
                {
                    var newChildren2 = new HelpContent(TreeViewEntryName(sub2Item.Name), sub2Item.Path)
                    {
                        Level = HelpContentLevel.Sub2,
                    };
                    newChildren.Children.Add(newChildren2);
                }
                newHelpContentEntry.Children.Add(newChildren);
            }
            helpTreeDatalist.Add(newHelpContentEntry);
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            HelpTreeDataSource = helpTreeDatalist;
            if (parameter is DataItem)
            {
                if (HelpTreeDataSource.Count > 0)
                {
                    SelectedHelpContent = HelpTreeDataSource[0];
                }
            }
        });
        await Task.CompletedTask;
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
        Task.Run(async () => await GetTreeViewEntrysAsync(parameter)).Wait();
    }

    public void OnNavigatedFrom()
    {
    }
}
