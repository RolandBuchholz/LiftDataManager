using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class TabellenansichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public CollectionViewSource GroupedItems
    {
        get; set;
    }

    public TabellenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

    private bool _CanShowUnsavedParameters;
    public bool CanShowUnsavedParameters
    {
        get => _CanShowUnsavedParameters;
        set => SetProperty(ref _CanShowUnsavedParameters, value);
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
            }
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
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
}
