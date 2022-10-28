using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public partial class ListenansichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public CollectionViewSource GroupedItems { get; set; }

    public ListenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

    [ObservableProperty]
    private bool isItemSelected;

    [ObservableProperty]
    private bool canShowUnsavedParameters;

    [ObservableProperty]
    private string? searchInput;
    partial void OnSearchInputChanged(string? value)
    {
        if (CurrentSpeziProperties != null)
        {
            CurrentSpeziProperties.SearchInput = SearchInput;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
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

    [RelayCommand(CanExecute = nameof(CanSaveParameter))]
    private async Task SaveParameterAsync()
    {
        if(Selected is null) return;
        if (FullPathXml is null) return;
        var infotext = await _parameterDataService!.SaveParameterAsync(Selected, FullPathXml);
        InfoSidebarPanelText += infotext;
        CanSaveParameter = false;
        Selected.IsDirty = false;
        await SetModelStateAsync();
        var allUnsavedParameters = (ObservableGroupedCollection<string, Parameter>)GroupedItems.Source;
        allUnsavedParameters.RemoveItem(allUnsavedParameters.First(g => g.Any(p => p.Name == Selected.Name)).Key, Selected, true);
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
            HasErrors = ParamterDictionary!.Values.Any(p => p.HasErrors);
            ParamterErrorDictionary ??= new();
            ParamterErrorDictionary.Clear();
            if (HasErrors)
            {
                var errors = ParamterDictionary.Values.Where(e => e.HasErrors);
                foreach (var error in errors)
                {
                    ParamterErrorDictionary.Add(error.Name!, error.parameterErrors[error.Name!]);
                }
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParamterDictionary!.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanShowUnsavedParameters = dirty;
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(App.MainRoot!,
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei aus zu checken?",
                                    "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                if (dialogResult)
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

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null)
            SearchInput = CurrentSpeziProperties.SearchInput;
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();
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
