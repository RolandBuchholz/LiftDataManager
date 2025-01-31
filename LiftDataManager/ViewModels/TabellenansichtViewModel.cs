using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class TabellenansichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public CollectionViewSource GroupedItems { get; set; }

    public TabellenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                                    ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
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
    public partial bool CanShowUnsavedParameters { get; set; }

    [ObservableProperty]
    public partial bool HasHighlightedParameters { get; set; }

    [ObservableProperty]
    public partial SelectorBarItem? SelectedFilter { get; set; }

    [ObservableProperty]
    public partial string? SearchInput { get; set; }
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
}