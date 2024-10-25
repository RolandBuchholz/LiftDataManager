using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public partial class ListenansichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public CollectionViewSource GroupedItems { get; set; }
    private ObservableDictionary<string, List<LiftHistoryEntry>> HistoryEntrysDictionary { get; set; } = new();
    public ObservableCollection<LiftHistoryEntry> ParameterHistoryEntrys { get; set; } = new();

    public ListenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService) :
         base(parameterDataService, dialogService, infoCenterService, settingService)
    {
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

    public override void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        SetInfoSidebarPanelHighlightText(message);
        _ = SetModelStateAsync();
        HasHighlightedParameters = false;
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    [ObservableProperty]
    private bool isItemSelected;

    [ObservableProperty]
    private bool canShowUnsavedParameters;

    [ObservableProperty]
    private bool hasHighlightedParameters;

    [ObservableProperty]
    private SelectorBarItem? selectedFilter;

    [ObservableProperty]
    private string? searchInput;
    partial void OnSearchInputChanged(string? value)
    {
        if (CurrentSpeziProperties != null)
        {
            CurrentSpeziProperties.SearchInput = SearchInput;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveParameterCommand))]
    private bool canSaveParameter;

    private Parameter? _selected;
    public Parameter? Selected
    {
        get
        {
            CheckIsDirty(_selected);
            if (_selected is not null)
            {
                IsItemSelected = true;
                SetParameterHistoryEntrys();
            }
            else
            {
                IsItemSelected = false;
            }
            return _selected;
        }
        set
        {
            if (Selected is not null)
            {
                Selected.PropertyChanged -= CheckIsDirty;
            }

            SetProperty(ref _selected, value);
            if (_selected is not null)
            {
                _selected.PropertyChanged += CheckIsDirty;
            }
        }
    }

    private void SetParameterHistoryEntrys()
    {
        if (_selected is null)
            return;
        ParameterHistoryEntrys.Clear();
        if (HistoryEntrysDictionary.TryGetValue(_selected.Name!, out var historyEntry))
        {
            foreach (var item in historyEntry.OrderByDescending(x => x.TimeStamp))
            {
                if (item is not null)
                    ParameterHistoryEntrys.Add(item);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveParameter))]
    private async Task SaveParameterAsync()
    {
        if (Selected is null)
            return;
        if (FullPathXml is null)
            return;
        var saveResult = await _parameterDataService!.SaveParameterAsync(Selected, FullPathXml);
        if (saveResult.Item1 != "Error")
        {
            await _infoCenterService.AddInfoCenterSaveInfoAsync(saveResult);
        }
        else
        {
            await _infoCenterService.AddInfoCenterErrorAsync(saveResult.Item3!);
            return;
        }
        CanSaveParameter = false;
        Selected.IsDirty = false;
        await SetModelStateAsync();

        if (Selected is null)
            return;
        if (!HistoryEntrysDictionary.ContainsKey(Selected?.Name!))
        {
            HistoryEntrysDictionary.Add(Selected?.Name!, []);
        }
        HistoryEntrysDictionary[Selected?.Name!].Add(_parameterDataService.GenerateLiftHistoryEntry(Selected!));

        SetParameterHistoryEntrys();
    }

    private void CheckIsDirty(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not null)
        {
            CheckIsDirty((Parameter)sender);
        }
    }

    private void CheckIsDirty(Parameter? Item)
    {
        if (Item is not null && Item.IsDirty)
        {
            CanSaveParameter = Item.DefaultUserEditable ? CheckOut : Adminmode && CheckOut;
        }
        else
        {
            CanSaveParameter = false;
        }

        SaveParameterCommand.NotifyCanExecuteChanged();
    }

    protected async override Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            HasErrors = ParameterDictionary!.Values.Any(p => p.HasErrors);
            if (HasErrors)
            {
                SetErrorDictionary();
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            CanShowUnsavedParameters = false;
            var dirty = ParameterDictionary!.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty;
                CanShowUnsavedParameters = dirty;
            }
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService.CheckOutDialogAsync(SpezifikationsNumber);
                switch (dialogResult)
                {
                    case CheckOutDialogResult.SuccessfulIncreaseRevision:
                        IncreaseRevision();
                        LikeEditParameter = true;
                        CheckOut = true;
                        break;
                    case CheckOutDialogResult.SuccessfulNoRevisionChange:
                        LikeEditParameter = true;
                        CheckOut = true;
                        break;
                    case CheckOutDialogResult.CheckOutFailed:
                        goto default;
                    case CheckOutDialogResult.ReadOnly:
                        LikeEditParameter = false;
                        CheckOut = false;
                        break;
                    default:
                        break;
                }
                CheckoutDialogIsOpen = false;
                SetModifyInfos();
            }
        }
        await Task.CompletedTask;
    }

    private async Task GetHistoryEntrysAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;
        var result = await _parameterDataService!.LoadLiftHistoryEntryAsync(path);
        if (result != null)
        {
            foreach (var entry in result)
            {
                if (HistoryEntrysDictionary.TryGetValue(entry.Name, out List<LiftHistoryEntry>? value))
                {
                    value.Add(entry);
                }
                else
                {
                    HistoryEntrysDictionary.Add(entry.Name, new List<LiftHistoryEntry>());
                    HistoryEntrysDictionary[entry.Name].Add(entry);
                }
            }
        }
    }

    private bool CheckhasHighlightedParameters()
    {
        if (ParameterDictionary is null || ParameterDictionary.Values is null)
            return false;
        return ParameterDictionary.Values.Any(x => x.IsKey);
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
            SearchInput = CurrentSpeziProperties.SearchInput;

        _ = GetHistoryEntrysAsync(FullPathXml);
        HasHighlightedParameters = CheckhasHighlightedParameters();

        if (parameter is null and not string)
        {
            return;
        }
        if (!Equals(parameter, "ShowHighlightParameter"))
        {
            SearchInput = parameter as string;
            return;
        }

        if (!HasHighlightedParameters)
        {
            return;
        }
        SelectedFilter = new SelectorBarItem() { Text = "RequestHighlighted" };
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }

    public void EnsureItemSelected()
    {
        if (Selected == null && GroupedItems.View != null && GroupedItems.View.Count > 0)
            Selected = (Parameter?)GroupedItems.View.FirstOrDefault();
    }
}