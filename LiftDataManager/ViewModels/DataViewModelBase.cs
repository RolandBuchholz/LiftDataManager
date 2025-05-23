﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class DataViewModelBase : ObservableRecipient
{
    protected readonly IParameterDataService _parameterDataService;
    protected readonly IDialogService _dialogService;
    protected readonly IInfoCenterService _infoCenterService;
    protected readonly ISettingService _settingService;
    protected readonly ILogger<DataViewModelBase> _baseLogger;

    public bool Adminmode { get; set; }
    public bool VaultDisabled { get; set; }
    public bool CheckoutDialogIsOpen { get; set; }
    public string SpezifikationsNumber => !string.IsNullOrWhiteSpace(FullPathXml) ? Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "") : string.Empty;
    public CurrentSpeziProperties? CurrentSpeziProperties { get; set; }
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public ObservableDictionary<string, List<ParameterStateInfo>> ParameterErrorDictionary { get; set; }
    public ObservableRangeCollection<InfoCenterEntry> InfoCenterEntrys { get; set; }

    public DataViewModelBase(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingsSelectorService, ILogger<DataViewModelBase> baseLogger)
    {
        _parameterDataService = parameterDataService;
        _dialogService = dialogService;
        _infoCenterService = infoCenterService;
        _settingService = settingsSelectorService;
        ParameterDictionary = _parameterDataService.GetParameterDictionary();
        InfoCenterEntrys = _infoCenterService.GetInfoCenterEntrys();
        ParameterErrorDictionary ??= [];
        _baseLogger = baseLogger;
    }
    public virtual void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
           !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }
    public virtual void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        SetInfoSidebarPanelHighlightText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    public virtual void Receive(RefreshModelStateMessage message)
    {
        if (message is null)
        {
            return;
        }
        CheckOut = message.Value.IsCheckOut;
        LikeEditParameter = message.Value.LikeEditParameterEnabled;
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    [ObservableProperty]
    public partial bool HasErrors { get; set; }

    [ObservableProperty]
    public partial bool InfoCenterIsOpen { get; set; }

    [ObservableProperty]
    public partial bool HideInfoErrors { get; set; }
    partial void OnHideInfoErrorsChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.HideInfoErrors, value))
        {
            CurrentSpeziProperties.HideInfoErrors = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    public partial string? FullPathXml { get; set; }
    partial void OnFullPathXmlChanged(string? value)
    {
        SetFullpathAutodeskTransfer(value);
    }

    [ObservableProperty]
    public partial bool AuftragsbezogeneXml { get; set; }
    partial void OnAuftragsbezogeneXmlChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.AuftragsbezogeneXml, value))
        {
            CurrentSpeziProperties.AuftragsbezogeneXml = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    public partial bool CheckOut { get; set; }
    partial void OnCheckOutChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.CheckOut, value))
        {
            CurrentSpeziProperties.CheckOut = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
        if (_settingService.AutoSave)
        {
            if (value)
            {
                _parameterDataService.StartAutoSaveTimerAsync(GetSaveTimerPeriod(), FullPathXml, Adminmode).SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
            }
            else
            {
                _parameterDataService.StopAutoSaveTimerAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
            }
        }
    }

    [ObservableProperty]
    public partial bool LikeEditParameter { get; set; }
    partial void OnLikeEditParameterChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.LikeEditParameter, value))
        {
            CurrentSpeziProperties.LikeEditParameter = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAllParameterCommand))]
    public partial bool CanSaveAllSpeziParameters { get; set; }

    [RelayCommand(CanExecute = nameof(CanSaveAllSpeziParameters))]
    public async Task SaveAllParameterAsync()
    {
        if (ParameterDictionary is null)
        {
            return;
        }
        if (FullPathXml is null)
        {
            return;
        }
        var saveResult = await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
        if (saveResult.Count != 0)
        {
            await _infoCenterService.AddInfoCenterSaveAllInfoAsync(saveResult);
        }
        await SetModelStateAsync();
    }
    protected virtual void SetFullpathAutodeskTransfer(string? value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<string>.Default.Equals(CurrentSpeziProperties.FullPathXml, value))
        {
            CurrentSpeziProperties.FullPathXml = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }
    protected virtual void SynchronizeViewModelParameter()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties is null)
        {
            return;
        }
        if (CurrentSpeziProperties.FullPathXml is not null)
        {
            FullPathXml = CurrentSpeziProperties.FullPathXml;
        }
        Adminmode = CurrentSpeziProperties.Adminmode;
        VaultDisabled = CurrentSpeziProperties.VaultDisabled;
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        HideInfoErrors = CurrentSpeziProperties.HideInfoErrors;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
    }
    protected async virtual Task SetModelStateAsync()
    {
        if (AuftragsbezogeneXml)
        {
            HasErrors = false;
            HasErrors = ParameterDictionary.Values.Any(p => p.HasErrors);
            if (HasErrors)
            {
                SetErrorDictionary();
            }
        }

        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParameterDictionary.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty;
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
    public void SetErrorDictionary()
    {
        ParameterErrorDictionary ??= [];
        ParameterErrorDictionary.Clear();
        var errors = ParameterDictionary?.Values.Where(e => e.HasErrors);
        if (errors is null)
            return;
        foreach (var error in errors)
        {
            if (ParameterErrorDictionary.TryGetValue(error.Name!, out List<ParameterStateInfo>? value))
            {
                value.AddRange([.. error.parameterErrors["Value"]]);
            }
            else
            {
                var errorList = new List<ParameterStateInfo>();
                errorList.AddRange([.. error.parameterErrors["Value"]]);
                ParameterErrorDictionary.Add(error.Name!, errorList);
            }
        }
    }
    protected void SetInfoSidebarPanelText(PropertyChangedMessage<string> message)
    {
        _infoCenterService.AddInfoCenterParameterChangedAsync(
            ((Parameter)message.Sender).Name,
            ((Parameter)message.Sender).DisplayName,
            message.OldValue,
            message.NewValue,
            ((Parameter)message.Sender).IsAutoUpdated);
    }
    protected void SetInfoSidebarPanelHighlightText(PropertyChangedMessage<bool> message)
    {
        var sender = (Parameter)message.Sender;
        _infoCenterService.AddInfoCenterMessageAsync($"|{sender.DisplayName}| Markierung " + (message.NewValue ? "hinzugefügt" : "entfernt"));
    }
    public CurrentSpeziProperties GetCurrentSpeziProperties()
    {
        return Messenger.Send<SpeziPropertiesRequestMessage>();
    }
    protected void SetModifyInfos()
    {
        ParameterDictionary["var_GeaendertVon"].AutoUpdateParameterValue(string.IsNullOrWhiteSpace(Environment.UserName) ? "Keine Angaben" : Environment.UserName);
        ParameterDictionary["var_GeaendertAm"].AutoUpdateParameterValue(DateTime.Now.ToShortDateString());
    }

    protected void IncreaseRevision()
    {
        if (ParameterDictionary is not null)
        {
            var newRevision = RevisionHelper.GetNextRevision(ParameterDictionary["var_Index"].Value);
            ParameterDictionary["var_Index"].AutoUpdateParameterValue(newRevision);
            ParameterDictionary["var_StandVom"].AutoUpdateParameterValue(DateTime.Today.ToShortDateString());
        }
    }

    protected int GetSaveTimerPeriod()
    {
        int defaultSaveTime = 5;
        var autoSavePeriod = _settingService.AutoSavePeriod;
        if (!string.IsNullOrWhiteSpace(autoSavePeriod))
        {
            return Convert.ToInt32(autoSavePeriod.Replace(" min", ""));
        }
        return defaultSaveTime;
    }
    private void ParameterDataService_ParameterDataAutoSaveStarted(object? sender, EventArgs e) 
    {
        SaveAllParameterCommand.Execute(null);
        _infoCenterService.AddInfoCenterMessageAsync("Parameter wurden automatisch gespeichert.");
    }
    protected void NavigatedToBaseActions()
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null)
        {
            SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
        }
        _parameterDataService.ParameterDataAutoSaveStarted += ParameterDataService_ParameterDataAutoSaveStarted;
    }
    protected void NavigatedFromBaseActions()
    {
        IsActive = false;
        _parameterDataService.ParameterDataAutoSaveStarted -= ParameterDataService_ParameterDataAutoSaveStarted;
    }

    [LoggerMessage(03001, LogLevel.Error,
    "TaskException: {taskEx}")]
    protected partial void LogTaskException(string taskEx);
}