using System.ComponentModel;
using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class ListenansichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public CollectionViewSource GroupedItems
    {
        get; set;
    }

    public ListenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };

        SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
    }

    public IAsyncRelayCommand SaveParameter
    {
        get;
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
            CanSaveParameter = true;
        }
        else
        {
            CanSaveParameter = false;
        }

        SaveParameter.NotifyCanExecuteChanged();
    }

    protected async override Task SetModelStateAsync()
    {
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

    private bool _CanShowUnsavedParameters;
    public bool CanShowUnsavedParameters
    {
        get => _CanShowUnsavedParameters;
        set => SetProperty(ref _CanShowUnsavedParameters, value);
    }

    private bool _IsItemSelected;
    public bool IsItemSelected
    {
        get => _IsItemSelected;
        set => SetProperty(ref _IsItemSelected, value);
    }

    private bool _CanSaveParameter;
    public bool CanSaveParameter
    {
        get => _CanSaveParameter;
        set
        {
            SetProperty(ref _CanSaveParameter, value);
            SaveParameter.NotifyCanExecuteChanged();
        }
    }

    private string? _SearchInput;
    public string? SearchInput
    {
        get => _SearchInput;

        set
        {
            SetProperty(ref _SearchInput, value);
            if (CurrentSpeziProperties is not null)
            {
                CurrentSpeziProperties.SearchInput = SearchInput;
                Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
            }
        }
    }

    private async Task SaveParameterAsync()
    {
        var infotext = await _parameterDataService!.SaveParameterAsync(Selected, FullPathXml);
        InfoSidebarPanelText += infotext;
        CanSaveParameter = false;
        if (Selected is not null)
        {
            Selected.IsDirty = false;
        }
        await SetModelStateAsync();
        var allUnsavedParameters = (ObservableGroupedCollection<string, Parameter>)GroupedItems.Source;
        if (Selected is not null)
        {
        allUnsavedParameters.RemoveItem(allUnsavedParameters.First(g => g.Any(p => p.Name == Selected.Name)).Key, Selected, true);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null)
        {
            SearchInput = CurrentSpeziProperties.SearchInput;
        }
        if (CurrentSpeziProperties is not null && CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }

    public void EnsureItemSelected()
    {
        if (Selected == null && GroupedItems.View != null && GroupedItems.View.Count > 0)
        {
            Selected = (Parameter?)GroupedItems.View.FirstOrDefault();
        }
    }
}
