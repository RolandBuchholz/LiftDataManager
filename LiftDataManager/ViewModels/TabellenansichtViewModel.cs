using CommunityToolkit.Mvvm.Messaging.Messages;
using WinUI.TableView;

namespace LiftDataManager.ViewModels;

public partial class TabellenansichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public List<Parameter>? ParameterList { get; set; }
    public TableView? ParameterTableView { get; set; }
    public TabellenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                    ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    public override void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        SetInfoSidebarPanelHighlightText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    [RelayCommand]
    public async Task ParameterViewLoadedAsync(TableView sender)
    {
        ParameterTableView = sender;
        ParameterTableView.FilterDescriptions.Add(new FilterDescription(string.Empty, Filter));
        await Task.CompletedTask;
    }

    private bool Filter(object? item)
    {
        if (item is null)
        {
            return false;
        }
        Parameter entry = (Parameter)item;

        var matchTextSearch = string.IsNullOrWhiteSpace(SearchInput) ||
                              entry.Name.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.DisplayName.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.Value?.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true ||
                              entry.Comment?.Contains(SearchInput, StringComparison.OrdinalIgnoreCase) is true;

        var matchFilter = SelectedFilter?.Text switch
        {
            "All" => true,
            "Highlighted" => entry.IsKey,
            "Validation Errors" => entry.HasErrors,
            "Unsaved" => entry.IsDirty,
            _ => true
        };
        return matchFilter && matchTextSearch;
    }

    [ObservableProperty]
    public partial bool CanShowUnsavedParameters { get; set; }

    [ObservableProperty]
    public partial bool HasHighlightedParameters { get; set; }

    [ObservableProperty]
    public partial SelectorBarItem? SelectedFilter { get; set; }
    partial void OnSelectedFilterChanged(SelectorBarItem? value) 
    {
        RefreshFilterCommand.Execute(this);
    }

    [ObservableProperty]
    public partial string? SearchInput { get; set; }
    partial void OnSearchInputChanged(string? value)
    {
        RefreshFilterCommand.Execute(this);
        if (CurrentSpeziProperties != null)
        {
            CurrentSpeziProperties.SearchInput = SearchInput;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [RelayCommand]
    public async Task RefreshFilterAsync()
    {
        ParameterTableView?.RefreshFilter();
        await Task.CompletedTask;
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

    private bool CheckhasHighlightedParameters()
    {
        if (ParameterDictionary is null || ParameterDictionary.Values is null)
        {
            return false;
        }
        return ParameterDictionary.Values.Any(x => x.IsKey);
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            SearchInput = CurrentSpeziProperties.SearchInput;
            ParameterList = [.. ParameterDictionary.Values];
        }
        HasHighlightedParameters = CheckhasHighlightedParameters();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}