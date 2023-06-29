using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class TabellenansichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public CollectionViewSource GroupedItems { get; set; }

    public TabellenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    [ObservableProperty]
    private bool canShowUnsavedParameters;

    [ObservableProperty]
    private bool hasHighlightedParameters;

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
                    if (!ParamterErrorDictionary.ContainsKey(error.Name!))
                    {
                        var errorList = new List<ParameterStateInfo>();
                        errorList.AddRange(error.parameterErrors["Value"].ToList());
                        ParamterErrorDictionary.Add(error.Name!, errorList);
                    }
                    else
                    {
                        ParamterErrorDictionary[error.Name!].AddRange(error.parameterErrors["Value"].ToList());
                    }
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

    private bool CheckhasHighlightedParameters()
    {
        if (ParamterDictionary is null || ParamterDictionary.Values is null)
            return false;
        return ParamterDictionary.Values.Any(x => x.IsKey);
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
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}