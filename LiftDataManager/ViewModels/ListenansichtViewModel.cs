using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public partial class ListenansichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public CollectionViewSource GroupedItems { get; set; }
    private ObservableDictionary<string, List<LiftHistoryEntry>> HistoryEntrysDictionary { get; set; } = new();
    public ObservableCollection<LiftHistoryEntry> ParameterHistoryEntrys { get; set; } = new();

    public ListenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

    public override void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

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
    private string selectedFilter = "All";

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
        var infotext = await _parameterDataService!.SaveParameterAsync(Selected, FullPathXml);
        InfoSidebarPanelText += infotext;
        CanSaveParameter = false;
        Selected.IsDirty = false;
        await SetModelStateAsync();

        if (Selected is null)
            return;
        if (!HistoryEntrysDictionary.ContainsKey(Selected?.Name!))
        {
            HistoryEntrysDictionary.Add(Selected?.Name!, new List<LiftHistoryEntry>());
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
            ParameterErrorDictionary ??= new();
            ParameterErrorDictionary.Clear();
            if (HasErrors)
            {
                var errors = ParameterDictionary.Values.Where(e => e.HasErrors);
                foreach (var error in errors)
                {
                    if (!ParameterErrorDictionary.ContainsKey(error.Name!))
                    {
                        var errorList = new List<ParameterStateInfo>();
                        errorList.AddRange(error.parameterErrors["Value"].ToList());
                        ParameterErrorDictionary.Add(error.Name!, errorList);
                    }
                    else
                    {
                        ParameterErrorDictionary[error.Name!].AddRange(error.parameterErrors["Value"].ToList());
                    }
                }
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            CanShowUnsavedParameters = false;
            var dirty = ParameterDictionary!.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanShowUnsavedParameters = dirty;
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei aus zu checken?",
                                    "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                if ((bool)dialogResult)
                {
                    CheckoutDialogIsOpen = false;
                    _navigationService!.NavigateTo("LiftDataManager.ViewModels.HomeViewModel");
                }
                else
                {
                    CheckoutDialogIsOpen = false;
                    LikeEditParameter = false;
                }
            }

        }
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
                if (!HistoryEntrysDictionary.ContainsKey(entry.Name))
                {
                    HistoryEntrysDictionary.Add(entry.Name, new List<LiftHistoryEntry>());
                }
                HistoryEntrysDictionary[entry.Name].Add(entry);
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
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null)
            SearchInput = CurrentSpeziProperties.SearchInput;
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
            _ = SetModelStateAsync();

        _ = GetHistoryEntrysAsync(FullPathXml);
        HasHighlightedParameters = CheckhasHighlightedParameters();

        if (parameter is null and not string)
            return;
        if (!Equals(parameter, "ShowHighlightParameter"))
            return;
        if (!HasHighlightedParameters)
            return;
        SelectedFilter = "Highlighted";
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }

    public void EnsureItemSelected()
    {
        if (Selected == null && GroupedItems.View != null && GroupedItems.View.Count > 0)
            Selected = (Parameter?)GroupedItems.View.FirstOrDefault();
    }
}