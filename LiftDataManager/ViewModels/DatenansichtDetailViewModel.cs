using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public partial class DatenansichtDetailViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public ObservableCollection<ParameterStateInfo> ErrorsList { get; set; }
    public ObservableCollection<LiftHistoryEntry> ParameterHistoryEntrys { get; set; } = new();
    private Parameter? _item;

    public Parameter? Item
    {
        get
        {
            CheckIsDirty(_item);
            return _item;
        }

        set
        {
            SetProperty(ref _item, value);
            if (_item != null)
                _item.PropertyChanged += OnPropertyChanged;
        }
    }

    public DatenansichtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        ErrorsList ??= new();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        CheckIsDirty((Parameter?)sender);
        SetParameterState((Parameter?)sender);
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveParameterCommand))]
    private bool canSaveParameter;

    [RelayCommand(CanExecute = nameof(CanSaveParameter))]
    private async Task SaveParameterAsync()
    {
        if (Item is null)
            return;
        if (FullPathXml == null)
            return;
        var saveResult = await _parameterDataService!.SaveParameterAsync(Item, FullPathXml);
        if (saveResult.Key != "Error")
        {
            await _infoCenterService.AddInfoCenterSaveInfoAsync(InfoCenterEntrys, saveResult);
        }
        else
        {
            await _infoCenterService.AddInfoCenterErrorAsync(InfoCenterEntrys, saveResult.Value!);
        }
        CanSaveParameter = false;
        if (Item is not null)
        {
            Item.IsDirty = false;
            ParameterHistoryEntrys.Add(_parameterDataService.GenerateLiftHistoryEntry(Item));
            var sortedHistoryEntrys = ParameterHistoryEntrys.OrderByDescending(o => o.TimeStamp).ToList();
            ParameterHistoryEntrys.Clear();
            foreach (var entry in sortedHistoryEntrys)
            {
                ParameterHistoryEntrys.Add(entry);
            }
        }
    }

    private void SetParameterState(Parameter? liftParameter)
    {
        ErrorsList.Clear();

        if (liftParameter is null)
            return;

        if (!liftParameter.HasErrors)
        {
            ErrorsList.Add(new ParameterStateInfo(liftParameter.Name!, liftParameter.DisplayName!, true)
            {
                Severity = ErrorLevel.Valid,
                ErrorMessage = "Keine Information, Warnungen oder Fehler vorhanden"
            });
        }

        if (liftParameter.parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? errorList))
        {
            if (errorList is null)
                return;
            if (!errorList.Any())
                return;

            var sortedErrorList = errorList.OrderByDescending(p => p.Severity);
            foreach (var item in sortedErrorList)
            {
                ErrorsList.Add(item);
            }
        }
    }

    private async Task GetHistoryEntrysAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;
        var result = await _parameterDataService!.LoadLiftHistoryEntryAsync(path);
        if (result is not null && Item is not null)
        {
            foreach (var item in result.Where(x => x.Name == Item.Name).OrderByDescending(x => x.TimeStamp))
            {
                if (item is not null)
                    ParameterHistoryEntrys.Add(item);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (parameter is not null && ParameterDictionary is not null)
        {
            var data = ParameterDictionary.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name));
            Item = data.First(i => i.Name == (string)parameter);
            SetParameterState(Item);
        }
        _ = GetHistoryEntrysAsync(FullPathXml);
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
        if (Item != null)
            Item.PropertyChanged -= OnPropertyChanged;
    }
}
