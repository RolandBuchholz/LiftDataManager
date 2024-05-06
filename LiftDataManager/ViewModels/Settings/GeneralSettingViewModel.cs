namespace LiftDataManager.ViewModels;

public partial class GeneralSettingViewModel : ObservableRecipient ,INavigationAwareEx
{
    private const string adminpasswort = "2342";
    private readonly ISettingService _settingService;
    private CurrentSpeziProperties _currentSpeziProperties;
    public GeneralSettingViewModel(ISettingService settingService)
    {
        _currentSpeziProperties ??= new();
        _settingService = settingService;
    }

    [ObservableProperty]
    private bool autoSave;
    partial void OnAutoSaveChanged(bool value)
    {
        if (!Equals(value, _settingService.AutoSave))
            _settingService.SetSettingsAsync(nameof(AutoSave), value);
    }

    [ObservableProperty]
    private bool autoOpenInfoCenter;
    partial void OnAutoOpenInfoCenterChanged(bool value)
    {
        if (!Equals(value, _settingService.AutoOpenInfoCenter))
            _settingService.SetSettingsAsync(nameof(AutoOpenInfoCenter), value);
    }

    [ObservableProperty]
    private bool tonerSaveMode;
    partial void OnTonerSaveModeChanged(bool value)
    {
        if (!Equals(value, _settingService.TonerSaveMode))
            _settingService.SetSettingsAsync(nameof(TonerSaveMode), value);
    }

    [ObservableProperty]
    private bool lowHighlightMode;
    partial void OnLowHighlightModeChanged(bool value)
    {
        if (!Equals(value, _settingService.LowHighlightMode))
            _settingService.SetSettingsAsync(nameof(LowHighlightMode), value);
    }

    [ObservableProperty]
    private string? autoSavePeriod;
    partial void OnAutoSavePeriodChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.LogLevel))
            _settingService.SetSettingsAsync(nameof(AutoSavePeriod), value);
    }

    public string[] SavePeriods = ["2 min", "5 min", "10 min", "15 min", "20 min", "30 min", "45 min"];

    [ObservableProperty]
    private bool canSwitchToAdminmode;

    [ObservableProperty]
    private string? passwortInfoText = "Kein PIN eingegeben";

    [ObservableProperty]
    private string? passwortInput;
    partial void OnPasswortInputChanged(string? value)
    {
        CheckpasswortInput();
    }

    [ObservableProperty]
    private bool adminmode;
    partial void OnAdminmodeChanged(bool value)
    {
        if (_currentSpeziProperties is not null && value != _currentSpeziProperties.Adminmode)
        {
            _settingService.SetSettingsAsync(nameof(Adminmode), value);
            _currentSpeziProperties.Adminmode = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(_currentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool adminmodeWarningAccepted;
    partial void OnAdminmodeWarningAcceptedChanged(bool value)
    {
        CanSwitchToAdminmode = (AdminmodeWarningAccepted == true
                        && PasswortInfoText == "Adminmode Pin korrekt Zugriff gewährt");
    }

    [RelayCommand]
    private async Task PinDialogAsync(ContentDialog pwdDialog)
    {
        if (!Adminmode)
        {
            var result = await pwdDialog.ShowAsyncQueueDraggable();
            Adminmode = result == ContentDialogResult.Primary;
        }
    }

    private void CheckpasswortInput()
    {
        switch (PasswortInput)
        {
            case "":
                PasswortInfoText = "Kein PIN eingegeben";
                CanSwitchToAdminmode = false;
                break;
            case adminpasswort:
                PasswortInfoText = "Adminmode Pin korrekt Zugriff gewährt";
                CanSwitchToAdminmode = AdminmodeWarningAccepted;
                break;
            default:
                PasswortInfoText = "Incorrecter Adminmode Pin";
                CanSwitchToAdminmode = false;
                break;
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        _currentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        AutoSave = _settingService.AutoSave;
        AutoSavePeriod = _settingService.AutoSavePeriod;
        TonerSaveMode = _settingService.TonerSaveMode;
        LowHighlightMode = _settingService.LowHighlightMode;
        AutoOpenInfoCenter = _settingService.AutoOpenInfoCenter;
    }
    public void OnNavigatedFrom()
    {

    }
}
