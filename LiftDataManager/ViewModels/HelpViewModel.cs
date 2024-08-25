﻿using LiftDataManager.Controls;
using LiftDataManager.Core.Models;
using Microsoft.UI.Dispatching;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace LiftDataManager.ViewModels;

public partial class HelpViewModel : ObservableRecipient, INavigationAwareEx
{
    public TreeView? HelpTreeView { get; set; }
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

    [RelayCommand]
    public void TreeViewLoaded(TreeView sender)
    {
        HelpTreeView = sender;
        if (SelectedHelpContent == null)
        {
            HelpTreeView.SelectedNode = HelpTreeView.RootNodes.First();
        }
    }
    [RelayCommand]
    public async Task SwitchHelpContent(string? contentName)
    {
        if (string.IsNullOrWhiteSpace(contentName))
        {
            return;
        }
        var helpContent = GetHelpContentByName(contentName);
        if (helpContent is not null && HelpTreeView is not null)
        {
            var pathSplit = helpContent.FolderPath.Split('\\');
            switch (helpContent.Level)
            {
                case HelpContentLevel.Main:
                    break;
                case HelpContentLevel.Sub:
                    ExpandNode(pathSplit.Last());
                    break;
                case HelpContentLevel.Sub2:

                    break;
                default:
                    return;
            }
            var itemContainer = HelpTreeView.ContainerFromItem(helpContent);
            HelpTreeView.SelectedNode = HelpTreeView.NodeFromContainer(itemContainer);
        }
        await Task.CompletedTask;
    }

    [ObservableProperty]
    private HelpContent? selectedHelpContent;
    partial void OnSelectedHelpContentChanged(HelpContent? value)
    {
        if (value == null)
        {
            return;
        }
        Task.Run(async () => await LoadMarkDownTextAsync(value.FolderPath));
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
            foreach (var item in MarkdownTextBlockList.ToImmutableArray())
            {
                MarkdownTextBlockList.Remove(item);
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
                    MarkdownText = text,
                };
                Binding switchHelpContentCommandBinding = new()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(SwitchHelpContentCommand)),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                helpTextBlock.SetBinding(MarkdownTextBlockControl.SwitchContentCommandProperty, switchHelpContentCommandBinding);
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

    private HelpContent? GetHelpContentByName(string name)
    {
        if (HelpTreeDataSource is null)
        {
            return null;
        }
        List<HelpContent> tempDataSource = [];
        foreach (var item in HelpTreeDataSource)
        {
            tempDataSource.Add(item);
            tempDataSource.AddRange(ReturnChildNodes(item));
        }

        return tempDataSource.FirstOrDefault(x => x.Name == name);
    }

    private static List<HelpContent> ReturnChildNodes(HelpContent node)
    {
        List<HelpContent> childs = [];
        if (node.Children.Any())
        {
            foreach (var child in node.Children)
            {
                childs.Add(child);
                if (child.Children.Any())
                {
                    childs.AddRange(ReturnChildNodes(child));
                }
            }
        }
        return childs;
    }
    private void ExpandNode(string node)
    {
        if (string.IsNullOrWhiteSpace(node))
        {
            return;
        }
        var content = GetHelpContentByName(node[3..]);
        if (content is not null && HelpTreeView is not null)
        {
            var itemContainer = HelpTreeView.ContainerFromItem(content);
            HelpTreeView.NodeFromContainer(itemContainer).IsExpanded = true;
        }
    }
    public void OnNavigatedTo(object parameter)
    {
        Task.Run(async () => await GetTreeViewEntrysAsync(parameter)).Wait();
    }

    public void OnNavigatedFrom()
    {
    }
}
