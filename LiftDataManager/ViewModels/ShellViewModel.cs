using DevWinUI;
using Microsoft.UI.Xaml.Navigation;
using System.Text.Json;
using Windows.Storage;

namespace LiftDataManager.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IRecipient<SpeziPropertiesChangedMessage>, IRecipient<SpeziPropertiesRequestMessage>
{
    private CurrentSpeziProperties CurrentSpeziProperties = new();
    private readonly Dictionary<Type, DataItem> NavigationDataItemsDictionary = [];
    public IJsonNavigationService JsonNavigationService { get; }
    public ShellViewModel(IJsonNavigationService jsonNavigationViewService)
    {
        JsonNavigationService = jsonNavigationViewService;
        JsonNavigationService.FrameNavigated += OnNavigated;
        IsActive = true;
    }



    [ObservableProperty]
    public partial bool ShowGlobalSearch { get; set; } = true;

    [ObservableProperty]
    public partial string? GlobalSearchInput { get; set; }

    [ObservableProperty]
    public partial string? HeaderText { get; set; }

    [ObservableProperty]
    public partial bool ShowHeaderNavigationViewControl { get; set; }

    [ObservableProperty]
    public partial Visibility HeaderTextVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial Visibility BreadcrumbnavigatorVisibility { get; set; } = Visibility.Collapsed;


    public void Receive(SpeziPropertiesRequestMessage message)
    {
        message.Reply(CurrentSpeziProperties);
    }

    public void Receive(SpeziPropertiesChangedMessage message)
    {
        CurrentSpeziProperties = message.Value;
    }

    [RelayCommand]
    private void StartGlobalSearch()
    {
        var searchInput = GlobalSearchInput;
        GlobalSearchInput = string.Empty;
        JsonNavigationService.NavigateTo(typeof(ListenansichtPage), searchInput);
    }

    [RelayCommand]
    private void GoToHelpViewModel() => JsonNavigationService.NavigateTo(typeof(HelpPage));

    private async Task GenreateNavigationDataItemsDictionary()
    {
        const string jsonFilePath = "Assets/NavViewMenu/NavigationViewControlData.json";
        var basePath = Path.GetDirectoryName(ProcessInfoHelper.GetFileVersionInfo().FileName);
        if (basePath is null)
        {
            return;
        }
        var sourcePath = Path.GetFullPath(Path.Combine(basePath, jsonFilePath));
        var file = await StorageFile.GetFileFromPathAsync(sourcePath);
        if (file is null)
        {
            return;
        }
        var jsonText = await FileIO.ReadTextAsync(file);
        if (jsonText is null)
        {
            return;
        }
        var controlInfoDataGroups = JsonSerializer.Deserialize<Root>(jsonText);
        if (controlInfoDataGroups is null)
        {
            return;
        }
        if (JsonNavigationService is not JsonNavigationService jsonNavigationService)
        {
            return;
        }
        for (int i = 0; i < controlInfoDataGroups.Groups.Count; i++)
        {
            foreach (var dataItem in controlInfoDataGroups.Groups[i].Items)
            {
                NavigationDataItemsDictionary.AddIfNotExists(jsonNavigationService.GetPageType(dataItem.UniqueId), dataItem);
            }
        }
        await Task.CompletedTask;
    }

    private async void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (NavigationDataItemsDictionary.Count == 0)
        {
           await GenreateNavigationDataItemsDictionary();
        }
        if (BreadcrumbPageMappings.PageDictionary.ContainsKey(e.SourcePageType))
        {
            ShowHeaderNavigationViewControl = true;
            ShowGlobalSearch = false;
            BreadcrumbnavigatorVisibility = Visibility.Visible;
            HeaderTextVisibility = Visibility.Collapsed;
        }
        else
        {
            breadcrumbnavigator.Visibility = Visibility.Collapsed;
            headerTextBlock.Visibility = Visibility.Visible;
            DataItem? dataItem = null;
            if (e.Parameter is DataItem)
            {
                dataItem = e.Parameter as DataItem;
            }
            HeaderText = dataItem?.Description;
            ShowGlobalSearch = dataItem is null || !dataItem.HideItem;
            view.AlwaysShowHeader = !string.IsNullOrWhiteSpace(dataItem?.Description);

            BreadcrumbnavigatorVisibility = Visibility.Collapsed;
            HeaderTextVisibility = Visibility.Visible;
            var dataItem = NavigationDataItemsDictionary.GetValueOrDefault(e.SourcePageType);
            HeaderText = dataItem?.Description;
            ShowGlobalSearch = dataItem is null || !dataItem.HideItem;
            ShowHeaderNavigationViewControl = !string.IsNullOrWhiteSpace(dataItem?.Description);
        }
        await Task.CompletedTask;
    }
}