using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class LiftHistoryViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public List<LiftHistoryEntry> HistoryEntrys { get; set; }
    public CollectionViewSource FilteredItems { get; set; }

    public LiftHistoryViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
                                base(parameterDataService, dialogService, navigationService)
    {
        FilteredItems = new CollectionViewSource
        {
            IsSourceGrouped = false
        };
        HistoryEntrys ??= new();
    }

    [ObservableProperty]
    private bool canShowHistoryEntrys;

    [ObservableProperty]
    private string? searchInput;

    private async Task GetLiftHistoryEntrysAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;
        var result = await _parameterDataService!.LoadLiftHistoryEntryAsync(path, true);
        if (result is null)
            return;
        HistoryEntrys.AddRange(result);
    }

    [RelayCommand]
    public async Task EditCommentAsync(object sender)
    {
        if (!CheckOut)
        {
            await _dialogService!.MessageDialogAsync("Edit Lift History Entry", "Auftrag ist schreibgeschützt(Checked In)!");
            return;
        }

        if (sender is LiftHistoryEntry liftHistoryEntry)
        {
            var timestamp = liftHistoryEntry.TimeStamp;
            var name = liftHistoryEntry.Name;
            SearchInput = name;
            var result = await _dialogService!.InputDialogAsync("Edit Lift History Entry", $"Kommentar ({name}) ändern:", "Kommentar");

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

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
            _ = SetModelStateAsync();
        _ = GetLiftHistoryEntrysAsync(FullPathXml);
        CanShowHistoryEntrys = HistoryEntrys.Any();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}