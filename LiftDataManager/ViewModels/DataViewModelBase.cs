using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MvvmHelpers;
using System.Collections.Specialized;

namespace LiftDataManager.ViewModels;

public partial class DataViewModelBase : ObservableRecipient
{
    public readonly IParameterDataService _parameterDataService;
    public readonly IDialogService _dialogService;
    public readonly IInfoCenterService _infoCenterService;

    public bool Adminmode { get; set; }
    public bool CheckoutDialogIsOpen { get; set; }
    public string SpezifikationsNumber => !string.IsNullOrWhiteSpace(FullPathXml) ? Path.GetFileNameWithoutExtension(FullPathXml!).Replace("-AutoDeskTransfer", "") : string.Empty;
    public DispatcherTimer? AutoSaveTimer { get; set; }
    public CurrentSpeziProperties? CurrentSpeziProperties { get; set; }
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public ObservableDictionary<string, List<ParameterStateInfo>> ParameterErrorDictionary { get; set; } = [];
    public ObservableRangeCollection<InfoCenterEntry> InfoCenterEntrys { get; set; }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    public DataViewModelBase()
    {
    }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    public DataViewModelBase(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService)
    {
        _parameterDataService = parameterDataService;
        _dialogService = dialogService;
        _infoCenterService = infoCenterService;
        ParameterDictionary ??= [];
        ParameterErrorDictionary ??= [];
        InfoCenterEntrys ??= [];
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
    private bool infoCenterIsOpen;

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
    [NotifyCanExecuteChangedFor(nameof(SaveAllParameterCommand))]
    private bool canSaveAllSpeziParameters;

    [RelayCommand(CanExecute = nameof(CanSaveAllSpeziParameters))]
    public async Task SaveAllParameterAsync()
    {
        if (ParameterDictionary is null)
            return;
        if (FullPathXml is null)
            return;
        var saveResult = await _parameterDataService!.SaveAllParameterAsync(ParameterDictionary, FullPathXml, Adminmode);
        if (saveResult.Any())
        {
            await _infoCenterService.AddInfoCenterSaveAllInfoAsync(InfoCenterEntrys, saveResult);
        }
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
        if (CurrentSpeziProperties is null)
            return;
        if (CurrentSpeziProperties.FullPathXml is not null)
            FullPathXml = CurrentSpeziProperties.FullPathXml;
        if (CurrentSpeziProperties.ParameterDictionary is not null)
            ParameterDictionary = CurrentSpeziProperties.ParameterDictionary;
        if (CurrentSpeziProperties.InfoCenterEntrys is not null)
            InfoCenterEntrys = CurrentSpeziProperties.InfoCenterEntrys;
        Adminmode = CurrentSpeziProperties.Adminmode;
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
                    LiftParameterNavigationHelper.NavigateToPage(typeof(HomePage));
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
        _infoCenterService.AddInfoCenterParameterChangedAsync(InfoCenterEntrys, ((Parameter)message.Sender).DisplayName, message.OldValue, message.NewValue, ((Parameter)message.Sender).IsAutoUpdated);
    }

    protected void SetInfoSidebarPanelHighlightText(PropertyChangedMessage<bool> message)
    {
        var sender = (Parameter)message.Sender;
        _infoCenterService.AddInfoCenterMessageAsync(InfoCenterEntrys, $"|{sender.DisplayName}| Markierung " + (message.NewValue ? "hinzugefügt" : "entfernt"));
    }

    public CurrentSpeziProperties GetCurrentSpeziProperties()
    {
        return Messenger.Send<SpeziPropertiesRequestMessage>();
    }

    private void OnInfoCenterEntrys_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.InfoCenterEntrys = InfoCenterEntrys;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    protected void NavigatedToBaseActions()
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties?.ParameterDictionary is not null &&
            CurrentSpeziProperties?.ParameterDictionary?.Values is not null)
            _ = SetModelStateAsync();
        InfoCenterEntrys.CollectionChanged += OnInfoCenterEntrys_CollectionChanged;
    }

    protected void NavigatedFromBaseActions()
    {
        IsActive = false;
        InfoCenterEntrys.CollectionChanged -= OnInfoCenterEntrys_CollectionChanged;
    }
}