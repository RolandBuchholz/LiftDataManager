using Microsoft.Win32;

namespace LiftDataManager.ViewModels;

public partial class GeneralSettingViewModel : ObservableRecipient, INavigationAwareEx
{
    private static readonly string _defaultDataStoragePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"\Bausatzauslegung\Auftrag\");
    private readonly ISettingService _settingService;
    public readonly IDialogService _dialogService;
    private CurrentSpeziProperties _currentSpeziProperties;
    public GeneralSettingViewModel(ISettingService settingService, IDialogService dialogService)
    {
        _currentSpeziProperties ??= new();
        _settingService = settingService;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    public partial string? PathDataStorage { get; set; }
    partial void OnPathDataStorageChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathDataStorage))
            _settingService.SetSettingsAsync(nameof(PathDataStorage), value);
    }

    [ObservableProperty]
    public partial bool AutoSave { get; set; }
    partial void OnAutoSaveChanged(bool value)
    {
        if (!Equals(value, _settingService.AutoSave))
            _settingService.SetSettingsAsync(nameof(AutoSave), value);
    }

    [ObservableProperty]
    public partial bool VaultDisabled { get; set; }
    partial void OnVaultDisabledChanged(bool value)
    {
        if (!Equals(value, _settingService.VaultDisabled))
            _settingService.SetSettingsAsync(nameof(VaultDisabled), value);
    }

    [ObservableProperty]
    public partial bool AutoOpenInfoCenter { get; set; }
    partial void OnAutoOpenInfoCenterChanged(bool value)
    {
        if (!Equals(value, _settingService.AutoOpenInfoCenter))
            _settingService.SetSettingsAsync(nameof(AutoOpenInfoCenter), value);
    }

    [ObservableProperty]
    public partial bool TonerSaveMode { get; set; }
    partial void OnTonerSaveModeChanged(bool value)
    {
        if (!Equals(value, _settingService.TonerSaveMode))
            _settingService.SetSettingsAsync(nameof(TonerSaveMode), value);
    }

    [ObservableProperty]
    public partial bool LowHighlightMode { get; set; }
    partial void OnLowHighlightModeChanged(bool value)
    {
        if (!Equals(value, _settingService.LowHighlightMode))
            _settingService.SetSettingsAsync(nameof(LowHighlightMode), value);
    }

    [ObservableProperty]
    public partial string? AutoSavePeriod { get; set; }
    partial void OnAutoSavePeriodChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.LogLevel))
            _settingService.SetSettingsAsync(nameof(AutoSavePeriod), value);
    }

    public string[] SavePeriods = ["2 min", "5 min", "10 min", "15 min", "20 min", "30 min", "45 min"];

    [ObservableProperty]
    public partial bool Adminmode { get; set; }
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
    public partial bool SafetycomponentEditormode  { get; set; }
    partial void OnSafetycomponentEditormodeChanged(bool value)
    {
        if (!Equals(value, _settingService.SafetycomponentEditormode))
            _settingService.SetSettingsAsync(nameof(SafetycomponentEditormode), value);
    }

    [RelayCommand]
    private async Task PinDialogAsync()
    {
        if (!Adminmode)
        {
            var title = "Admin Mode";
            var condition = "Ich verfüge über die Erfahrung Pamameter ohne Plausibilitätsprüfung zu ändern.";
            var description = """
                              Achtung im Adminmode können nicht validierte Parameter gespeichet werden.
                              Die Parameter werden nicht auf Plausibilität geprüft.
                              """;
            Adminmode = await _dialogService.PasswordDialogAsync(title, condition, description, "2342");
        }
    }

    [RelayCommand]
    private async Task PinDialogSafetycomponentEditormodeAsync()
    {
        if (!SafetycomponentEditormode)
        {
            var title = "Safetycomponent Editormode";
            var condition = "Ich verfüge über die Berechtigung die Sicherheitsbauteil-Datenbank zu bearbeiten.";
            var description = """
                              Achtung im Safetycomponent Editormode können Sicherheitsbauteile angelegt,
                              geändert und gelöscht werden.
                              """;
            SafetycomponentEditormode = await _dialogService.PasswordDialogAsync(title, condition, description, "2014");
        }
    }

    private static string GetDataStoragePath()
    {
        var cfpLiftRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\BERCHTENBREITER_GMBH\CARFRAMEPROGRAM");
        if (cfpLiftRegistryKey is null)
        {
            return _defaultDataStoragePath;
        }
        var cfpLiftPath = Registry.GetValue(cfpLiftRegistryKey.Name, "CFP-DataSavePath", "")?.ToString();
        if (string.IsNullOrWhiteSpace(cfpLiftPath))
        {
            return _defaultDataStoragePath;
        }
        return cfpLiftPath;
    }
    public void OnNavigatedTo(object parameter)
    {
        _currentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        SafetycomponentEditormode = _settingService.SafetycomponentEditormode;
        AutoSave = _settingService.AutoSave;
        AutoSavePeriod = _settingService.AutoSavePeriod;
        TonerSaveMode = _settingService.TonerSaveMode;
        LowHighlightMode = _settingService.LowHighlightMode;
        AutoOpenInfoCenter = _settingService.AutoOpenInfoCenter;
        VaultDisabled = _settingService.VaultDisabled;
        PathDataStorage = GetDataStoragePath();
    }
    public void OnNavigatedFrom()
    {

    }
}
