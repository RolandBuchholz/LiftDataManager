﻿using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class DataViewModelBase : ObservableRecipient
{
    public readonly IParameterDataService? _parameterDataService;
    public readonly IDialogService? _dialogService;
    public readonly INavigationService? _navigationService;

    public bool Adminmode { get; set; }
    public bool CheckoutDialogIsOpen { get; set; }
    public string SpezifikationsNumber => !string.IsNullOrWhiteSpace(FullPathXml) ? Path.GetFileNameWithoutExtension(FullPathXml!).Replace("-AutoDeskTransfer", "") : string.Empty;
    public DispatcherTimer? AutoSaveTimer { get; set; }
    public CurrentSpeziProperties? CurrentSpeziProperties;
    public ObservableDictionary<string, Parameter>? ParameterDictionary { get; set; }
    public ObservableDictionary<string, List<ParameterStateInfo>>? ParameterErrorDictionary { get; set; } = new();

    public DataViewModelBase()
    {
    }

    public DataViewModelBase(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService)
    {
        _parameterDataService = parameterDataService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    public virtual void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    public virtual void Receive(PropertyChangedMessage<bool> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        SetInfoSidebarPanelHighlightText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private bool hideInfoErrors;
    partial void OnHideInfoErrorsChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.HideInfoErrors, value))
        {
            CurrentSpeziProperties.HideInfoErrors = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private string? fullPathXml;
    partial void OnFullPathXmlChanged(string? value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<string>.Default.Equals(CurrentSpeziProperties.FullPathXml, value))
        {
            CurrentSpeziProperties.FullPathXml = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool auftragsbezogeneXml;
    partial void OnAuftragsbezogeneXmlChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.AuftragsbezogeneXml, value))
        {
            CurrentSpeziProperties.AuftragsbezogeneXml = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool checkOut;
    partial void OnCheckOutChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.CheckOut, value))
        {
            CurrentSpeziProperties.CheckOut = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool likeEditParameter;
    partial void OnLikeEditParameterChanged(bool value)
    {
        if (CurrentSpeziProperties is not null && !EqualityComparer<bool>.Default.Equals(CurrentSpeziProperties.LikeEditParameter, value))
        {
            CurrentSpeziProperties.LikeEditParameter = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private string? infoSidebarPanelText;
    partial void OnInfoSidebarPanelTextChanged(string? value)
    {
        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.InfoSidebarPanelText = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAllParameterCommand))]
    private bool canSaveAllSpeziParameters;

    [RelayCommand(CanExecute = nameof(CanSaveAllSpeziParameters))]
    public async Task SaveAllParameterAsync()
    {
        if (ParameterDictionary is null)
            return;
        if (FullPathXml is null)
            return;
        var infotext = await _parameterDataService!.SaveAllParameterAsync(ParameterDictionary, FullPathXml, Adminmode);
        InfoSidebarPanelText += infotext;
        await SetModelStateAsync();
        if (AutoSaveTimer is not null)
        {
            var saveTimeIntervall = AutoSaveTimer.Interval;
            AutoSaveTimer.Stop();
            AutoSaveTimer.Interval = saveTimeIntervall;
            AutoSaveTimer.Start();
        }
    }

    protected virtual void SynchronizeViewModelParameter()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.FullPathXml is not null)
            FullPathXml = CurrentSpeziProperties.FullPathXml;
        if (CurrentSpeziProperties.ParameterDictionary is not null)
            ParameterDictionary = CurrentSpeziProperties.ParameterDictionary;
        Adminmode = CurrentSpeziProperties.Adminmode;
        AuftragsbezogeneXml = CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = CurrentSpeziProperties.CheckOut;
        HideInfoErrors = CurrentSpeziProperties.HideInfoErrors;
        LikeEditParameter = CurrentSpeziProperties.LikeEditParameter;
        InfoSidebarPanelText = CurrentSpeziProperties.InfoSidebarPanelText;
    }

    protected async virtual Task SetModelStateAsync()
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
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService!.WarningDialogAsync(
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei auszuchecken?",
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
                    if (CurrentSpeziProperties is not null)
                    {
                        CurrentSpeziProperties.LikeEditParameter = LikeEditParameter;
                        _ = Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
                    }
                }
            }
        }
        await Task.CompletedTask;
    }

    public void SetErrorDictionary()
    {
        ParameterErrorDictionary ??= new();
        ParameterErrorDictionary.Clear();
        var errors = ParameterDictionary?.Values.Where(e => e.HasErrors);
        if (errors is null)
            return;
        foreach (var error in errors)
        {
            if (ParameterErrorDictionary.TryGetValue(error.Name!, out List<ParameterStateInfo>? value))
            {
                value.AddRange(error.parameterErrors["Value"].ToList());
            }
            else
            {
                var errorList = new List<ParameterStateInfo>();
                errorList.AddRange(error.parameterErrors["Value"].ToList());
                ParameterErrorDictionary.Add(error.Name!, errorList);
            }
        }
    }

    protected void SetInfoSidebarPanelText(PropertyChangedMessage<string> message)
    {
        InfoSidebarPanelText += $"{message.PropertyName} : {message.OldValue} => {message.NewValue} geändert \n";
    }

    protected void SetInfoSidebarPanelHighlightText(PropertyChangedMessage<bool> message)
    {
        var sender = (Parameter)message.Sender;

        if (message.NewValue)
        {
            InfoSidebarPanelText += $"|{sender.DisplayName}| Markierung hinzugefügt\n";
        }
        else
        {
            InfoSidebarPanelText += $"|{sender.DisplayName}| Markierung entfernt\n";
        }
    }

    public CurrentSpeziProperties GetCurrentSpeziProperties()
    {
        return Messenger.Send<SpeziPropertiesRequestMessage>();
    }
}