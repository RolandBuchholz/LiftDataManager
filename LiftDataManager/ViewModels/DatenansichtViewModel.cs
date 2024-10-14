using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Animations;

namespace LiftDataManager.ViewModels;

public partial class DatenansichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly IJsonNavigationViewService _jsonNavigationViewService;
    public CollectionViewSource GroupedItems { get; set; }

    public DatenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, IJsonNavigationViewService jsonNavigationViewService) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        _jsonNavigationViewService = jsonNavigationViewService;
        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

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

    protected async override Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            HasErrors = ParameterDictionary!.Values.Any(p => p.HasErrors);
            if (HasErrors)
                SetErrorDictionary();
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
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
                    LiftParameterNavigationHelper.NavigateToPage(typeof(HomePage));
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
        if (ParameterDictionary is null || ParameterDictionary.Values is null)
            return false;
        return ParameterDictionary.Values.Any(x => x.IsKey);
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
            SearchInput = CurrentSpeziProperties.SearchInput;
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }

    [RelayCommand]
    private void ItemClick(ItemClickEventArgs e)
    {
        if (e.ClickedItem is Parameter parameter)
        {
            _jsonNavigationViewService.Frame?.SetListDataItemForNextConnectedAnimation(parameter);
            _jsonNavigationViewService.NavigateTo(typeof(DatenansichtDetailPage), parameter.Name);
        }
    }
}
