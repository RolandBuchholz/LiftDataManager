using Windows.ApplicationModel;

namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private const string adminpasswort = "2342";
    private readonly IThemeSelectorService _themeSelectorService;
    private CurrentSpeziProperties? CurrentSpeziProperties = new();
    private readonly ISettingService _settingService;
    private readonly IAuswahlParameterDataService _auswahlParameterDataService;
    private readonly IDialogService _dialogService;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService, IAuswahlParameterDataService auswahlParameterDataService, IDialogService dialogService)
    {
        _themeSelectorService = themeSelectorService;
        _settingService = settingsSelectorService;
        _auswahlParameterDataService = auswahlParameterDataService;
        _dialogService = dialogService;
        ElementTheme = _themeSelectorService.Theme;
        VersionDescription = GetVersionDescription();
    }

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private bool customAccentColor;
    partial void OnCustomAccentColorChanged(bool value)
    {
        _settingService.SetSettingsAsync("CustomAccentColor", value);
        if (CurrentSpeziProperties is not null)
        {
            CurrentSpeziProperties.CustomAccentColor = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private string? versionDescription;

    [ObservableProperty]
    private string? infoText;

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
        if (CurrentSpeziProperties is not null && value != CurrentSpeziProperties.Adminmode)
        {
            _settingService.SetSettingsAsync("Adminmode", value);
            CurrentSpeziProperties.Adminmode = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool adminmodeWarningAccepted;
    partial void OnAdminmodeWarningAcceptedChanged(bool value)
    {
        CanSwitchToAdminmode = (AdminmodeWarningAccepted == true
                        && PasswortInfoText == "Adminmode Pin korrekt Zugriff gewährt");
    }

    [ObservableProperty]
    private string? pathCFP;
    partial void OnPathCFPChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value,_settingService.PathCFP)) _settingService.SetSettingsAsync("PathCFP", value);
    }

    [ObservableProperty]
    private string? pathZALift;
    partial void OnPathZALiftChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathZALift)) _settingService.SetSettingsAsync("PathZALift", value);
    }

    [ObservableProperty]
    private string? pathLilo;
    partial void OnPathLiloChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathLilo)) _settingService.SetSettingsAsync("PathLilo", value);
    }

    [ObservableProperty]
    private string? pathExcel;
    partial void OnPathExcelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathExcel)) _settingService.SetSettingsAsync("PathExcel", value);
    }

    [RelayCommand]
    private async Task SwitchThemeAsync(ElementTheme param)
    {
        if (ElementTheme != param)
        {
            ElementTheme = param;
            await _themeSelectorService.SetThemeAsync(param);
        }
    }

    [RelayCommand]
    private async Task PinDialogAsync(ContentDialog? pwdDialog)
    {
        if (!Adminmode)
        {
            var result = await pwdDialog?.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Adminmode = true;
                InfoText += "Adminmode aktiviert\n";
            }
            else
            {
                Adminmode = false;
                InfoText += "Adminmode deaktiviert\n";
            }
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SwitchAccentColorAsync()
    {
        await _dialogService.MessageDialogAsync(App.MainRoot!, "Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    [RelayCommand]
    private async Task UpdateAuswahlParameterAsync()
    {
        var watch = Stopwatch.StartNew();
        InfoText += "Daten aus Spezifikation werden geladen\n";
        InfoText += "Loading ...\n";
        var result = await Task.Run(() => _auswahlParameterDataService.UpdateAuswahlparameterAsync());
        var updateDataInfo = new StringBuilder();
        updateDataInfo.AppendLine("Aktualisierte Daten: ");
        updateDataInfo.AppendLine("--------------------");
        updateDataInfo.AppendLine(result);
        InfoText += updateDataInfo;
        var stopTimeMs = watch.ElapsedMilliseconds;
        InfoText += $"Downloadtime:  {stopTimeMs} ms\n";
        InfoText += "Daten erfolgreich geladen\n";
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

    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();
        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public void OnNavigatedTo(object parameter)
    {
        if (CurrentSpeziProperties is not null)
            CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        CustomAccentColor = _settingService.CustomAccentColor;
        PathCFP = _settingService.PathCFP;
        PathZALift = _settingService.PathZALift;
        PathLilo = _settingService.PathLilo;
        PathExcel = _settingService.PathExcel;
    }
    public void OnNavigatedFrom()
    {
    }
}