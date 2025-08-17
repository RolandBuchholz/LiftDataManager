using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Helpers;
using WinUI.TableView;

namespace LiftDataManager.ViewModels;

public partial class LiftHistoryViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<RefreshModelStateMessage>
{
    const string defaultRevisionName = "All Revisions";
    const string defaultPriorityName = "All Parameter";
    public List<LiftHistoryEntry> HistoryEntrys { get; set; }
    public TableView? HistoryTableView { get; set; }
    public ObservableDictionary<string, DateTime> RevisionsDictionary { get; set; }
    public List<string> Prioritys { get; set; }
    
    private static DateRange _dateRange = new(DateTime.MinValue, DateTime.MaxValue);

    public LiftHistoryViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
                                base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        HistoryEntrys ??= [];
        RevisionsDictionary ??= [];
        Prioritys ??=
        [
            defaultPriorityName,
            "Dispoplan relevant",
            "Kabinendesign relevant",
            "Tableaudesign relevant"
        ];
    }

    [RelayCommand]
    public async Task HistoryTableViewLoadedAsync(TableView sender)
    {
        HistoryTableView = sender as TableView;
        HistoryTableView.FilterDescriptions.Add(new FilterDescription(string.Empty, Filter));
        FilteredHistoryEntrysCount = HistoryTableView.Items.Count;
        await Task.CompletedTask;
    }
    private bool Filter(object? item)
    {
        if (item is null)
        {
            return false;
        }
        LiftHistoryEntry entry = (LiftHistoryEntry)item;

        var matchTextSearch = string.IsNullOrWhiteSpace(SearchInput) ||
                              entry.DisplayName.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.Name.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.NewValue?.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.Comment?.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true;

        var inDateRange = _dateRange.WithInRange(entry.TimeStamp);

        var matchPriority = Priority switch
        {
            "Dispoplan relevant"=> entry.DispoPlanRelated,
            "Kabinendesign relevant"=> entry.CarDesignRelated,
            "Tableaudesign relevant"=> entry.LiftPanelRelated,
            _ => true
        };
        return matchTextSearch && matchPriority && inDateRange;
    }

    private static void SetDateRange(DateTimeOffset? startDate, DateTimeOffset? enddate)
    {
        var start = startDate is null ? DateTime.MinValue : startDate.GetValueOrDefault().DateTime;
        var end = enddate is null ? DateTime.MaxValue : enddate.GetValueOrDefault().DateTime;
        _dateRange = new DateRange(start, end);
    }

    [ObservableProperty]
    public partial bool CanShowHistoryEntrys { get; set; }

    [ObservableProperty]
    public partial int FilteredHistoryEntrysCount { get; set; }

    [ObservableProperty]
    public partial string Revision { get; set; } = defaultRevisionName;
    partial void OnRevisionChanged(string value)
    {
        if (!string.Equals(value, defaultRevisionName))
        {
            if (string.Equals(Revision, "Erster Stand"))
            {
                if (RevisionsDictionary.TryGetValue($"Stand : A", out DateTime endValue))
                {
                    EndDate = endValue;
                }
            }
            else
            {
                if (RevisionsDictionary.TryGetValue(Revision, out DateTime startValue))
                {
                    StartDate = startValue;
                }
                var nextRevision = RevisionHelper.GetNextRevision(Revision.Replace("Stand :", ""));
                if (RevisionsDictionary.TryGetValue($"Stand : {nextRevision}", out DateTime endValue))
                {
                    EndDate = endValue;
                }
            }
        }
        else
        {
            StartDate = null;
            EndDate = null;
        }
    }

    [ObservableProperty]
    public partial string Priority { get; set; } = defaultPriorityName;
    partial void OnPriorityChanged(string value)
    {
        RefreshFilterCommand.Execute(this);
    }

    [ObservableProperty]
    public partial DateTimeOffset? StartDate { get; set; }
    partial void OnStartDateChanged(DateTimeOffset? value)
    {
        if (EndDate != DateTimeOffset.MinValue && value > EndDate)
        {
            EndDate = value.Value;
        }
        SetDateRange(value, EndDate);
        RefreshFilterCommand.Execute(this);
    }

    [ObservableProperty]
    public partial DateTimeOffset? EndDate { get; set; }
    partial void OnEndDateChanged(DateTimeOffset? value)
    {
        if (value < StartDate)
        {
            StartDate = value.Value;
        }
        SetDateRange(StartDate, value);
        RefreshFilterCommand.Execute(this);
    }

    [ObservableProperty]
    public partial string? SearchInput { get; set; }
    partial void OnSearchInputChanged(string? value)
    {
        RefreshFilterCommand.Execute(this);
    }

    [RelayCommand]
    public async Task RefreshFilterAsync() 
    {
        HistoryTableView?.RefreshFilter();
        FilteredHistoryEntrysCount = HistoryTableView is null ? 0 : HistoryTableView.Items.Count;
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ResetFilterAsync()
    {
        StartDate = null;
        EndDate = null;
        SetDateRange(null, null);
        Priority = defaultPriorityName;
        Revision = defaultRevisionName;
        HistoryTableView?.ClearAllFilters();
        HistoryTableView?.ClearAllSorting();
        HistoryTableView?.FilterDescriptions.Add(new FilterDescription(string.Empty, Filter));
        SearchInput = string.Empty;
        await Task.CompletedTask;
    }

    private async Task GetLiftHistoryEntrysAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
        var result = await _parameterDataService.LoadLiftHistoryEntryAsync(path, true);
        if (result is null)
        {
            return;
        }
        HistoryEntrys.AddRange(result.OrderByDescending(x => x.TimeStamp));
    }

    [RelayCommand]
    public async Task EditCommentAsync(object sender)
    {
        if (!CheckOut)
        {
            await _dialogService.MessageDialogAsync("Edit Lift History Entry", "Auftrag ist schreibgeschützt(Checked In)!");
            return;
        }

        if (sender is LiftHistoryEntry liftHistoryEntry)
        {
            var timestamp = liftHistoryEntry.TimeStamp;
            var name = liftHistoryEntry.Name;
            SearchInput = name;
            var result = await _dialogService.InputDialogAsync("Edit Lift History Entry", $"Kommentar ({name}) ändern:", "Kommentar");

            if (!string.IsNullOrWhiteSpace(result))
            {
                var entry = HistoryEntrys.SingleOrDefault(s => s.TimeStamp == timestamp && s.Name == name);
                if (entry is not null)
                {
                    var user = _parameterDataService?.GetCurrentUser();
                    var editTime = DateTime.Now;
                    var newComment = $"""
                        Edit: {user} ({editTime})
                        {result}
                        """;
                    entry.Comment = newComment;
                }
                if (FullPathXml is not null && _parameterDataService is not null)
                {
                    await _parameterDataService.AddParameterListToHistoryAsync(HistoryEntrys, FullPathXml, true);
                }
                SearchInput = string.Empty;
            }
        }
    }

    private void SetRevisions()
    {
        RevisionsDictionary.Clear();
        RevisionsDictionary.TryAdd(defaultRevisionName, DateTime.MinValue);
        var revisions = HistoryEntrys.Where(x => x.Name == "var_Index");

        foreach (var revision in revisions)
        {
            if (!string.IsNullOrWhiteSpace(revision.NewValue))
            {
                RevisionsDictionary.TryAdd($"Stand : {revision.NewValue}", revision.TimeStamp);
            }
            else
            {
                RevisionsDictionary.TryAdd("Erster Stand", DateTime.MinValue);
            }
        }
    }
    public async void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        await GetLiftHistoryEntrysAsync(FullPathXml);
        CanShowHistoryEntrys = HistoryEntrys.Count != 0;
        if (CanShowHistoryEntrys)
        {
            SetRevisions();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}