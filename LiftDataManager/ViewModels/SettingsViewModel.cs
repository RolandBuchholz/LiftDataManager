using LiftDataManager.Core.DataAccessLayer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUICommunity;

namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private const string adminpasswort = "2342";
    private readonly IThemeSelectorService _themeSelectorService;
    private CurrentSpeziProperties? CurrentSpeziProperties = new();
    private readonly ISettingService _settingService;
    private readonly IDialogService _dialogService;
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<SettingsViewModel> _logger;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService, IDialogService dialogService,
                             ParameterContext parametercontext, ILogger<SettingsViewModel> logger)
    {
        _themeSelectorService = themeSelectorService;
        _settingService = settingsSelectorService;
        _dialogService = dialogService;
        _parametercontext = parametercontext;
        _logger = logger;
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
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
        }
    }

    [ObservableProperty]
    private bool autoSave;
    partial void OnAutoSaveChanged(bool value)
    {
        if (!Equals(value, _settingService.AutoSave))
            _settingService.SetSettingsAsync(nameof(AutoSave), value);
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

    public string[] SavePeriods = { "2 min", "5 min", "10 min", "15 min", "20 min", "30 min", "45 min" };

#pragma warning disable CA1822 // Member als statisch markieren
    public string UserName => string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) ? "no user detected" : System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    public OperatingSystem OSVersion => Environment.OSVersion;
    public string RuntimeInformation => string.IsNullOrWhiteSpace(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription) ? "no runtime detected" : System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    public string Vault2023Installed => File.Exists(@"C:\\Programme\\Autodesk\\Vault Client 2023\Explorer\\Connectivity.VaultPro.exe") ? "installiert" : "nicht installiert";
    public string VDSInstalled => Directory.Exists(@"C:\\ProgramData\\Autodesk\\Vault 2023\\Extensions\\DataStandard") ? "installiert" : "nicht installiert";
    public string PowerVaultInstalled => File.Exists(@"C:\\Program Files\\coolOrange\\Modules\\powerVault\\powerVault.Cmdlets.dll") ? "installiert" : "nicht installiert";
    public string AdskLicensingSDKInstalled => File.Exists(@"C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\AdskLicensingSDK_6.dll") ? "installiert" : "nicht installiert";
#pragma warning restore CA1822 // Member als statisch markieren

    public List<string> LogLevel = new() { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" };

    [ObservableProperty]
    private string? selectedLogLevel;
    partial void OnSelectedLogLevelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.LogLevel))
            _settingService.SetSettingsAsync(nameof(LogLevel), value);
    }

    [ObservableProperty]
    private string? versionDescription;

    public List<LiftDataManagerVersion> VersionsHistory => _parametercontext.Set<LiftDataManagerVersion>().OrderByDescending(x => x.VersionsNumber).ToList();

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
            _settingService.SetSettingsAsync(nameof(Adminmode), value);
            CurrentSpeziProperties.Adminmode = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(CurrentSpeziProperties));
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
        if (!string.Equals(value, _settingService.PathCFP))
            _settingService.SetSettingsAsync(nameof(PathCFP), value);
    }

    [ObservableProperty]
    private string? pathZALift;
    partial void OnPathZALiftChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathZALift))
            _settingService.SetSettingsAsync(nameof(PathZALift), value);
    }

    [ObservableProperty]
    private string? pathLilo;
    partial void OnPathLiloChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathLilo))
            _settingService.SetSettingsAsync(nameof(PathLilo), value);
    }

    [ObservableProperty]
    private string? pathExcel;
    partial void OnPathExcelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathExcel))
            _settingService.SetSettingsAsync(nameof(PathExcel), value);
    }

    [ObservableProperty]
    private string? pathDataBase;
    partial void OnPathDataBaseChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathDataBase))
            _settingService.SetSettingsAsync(nameof(PathDataBase), value);
    }

    [ObservableProperty]
    private string? infoBarVersionsUpdateText;

    [ObservableProperty]
    private bool infoBarVersionsUpdateIsOpen;

    [ObservableProperty]
    private InfoBarSeverity infoBarVersionsUpdateSeverity;

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
    private async Task PinDialogAsync(ContentDialog pwdDialog)
    {
        if (!Adminmode)
        {
            var result = await pwdDialog.ShowAsyncQueueDraggable();

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
        await _dialogService.MessageDialogAsync("Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    [RelayCommand]
    private async Task UpdateCheckAsync()
    {
        InfoText = string.Empty;
        var pm = new PackageManager();
        Package? package = pm.FindPackagesForUser(string.Empty, "BD956274-5B08-468A-AE87-B6190C6A8210", "CN=Buchholz").FirstOrDefault();
        if (package is not null)
        {
            PackageUpdateAvailabilityResult result = await package.CheckUpdateAvailabilityAsync();
            switch (result.Availability)
            {
                case PackageUpdateAvailability.Available:
                case PackageUpdateAvailability.Required:
                    InfoBarVersionsUpdateIsOpen = true;
                    InfoBarVersionsUpdateText = $"Es wurde eine neuere LiftDataManagerVersion gefunden und wird installiert...";
                    InfoBarVersionsUpdateSeverity = InfoBarSeverity.Informational;
                    var dialogResult = await _dialogService.ConfirmationDialogAsync("LiftDataManagerVersionsUpdateCheck", "Liftdatamanager muss nach dem Update neugestartet werden.\n" +
                        "Nicht gespeicherte Daten gehen verloren!", "Update starten", "Später updaten", "Abbrechen");
                    if (dialogResult is not null && (bool)dialogResult)
                    {
                        InfoText += $"Es wurde eine neuere LiftDataManagerVersion gefunden und wird installiert...";
                        _logger.LogInformation(60181, "LiftDataManagerupdate found and installing...");
                        await pm.AddPackageByAppInstallerFileAsync(new Uri(@"\\Bauer\AUFTRÄGE NEU\Vorlagen\LiftDataManager\LiftDataManager_x64.appinstaller"),
                            AddPackageByAppInstallerOptions.None, pm.GetDefaultPackageVolume());
                    }
                    break;
                case PackageUpdateAvailability.NoUpdates:
                    InfoBarVersionsUpdateIsOpen = true;
                    InfoBarVersionsUpdateText = $"Es ist die aktuellste LiftDataManagerVersion installiert.";
                    InfoBarVersionsUpdateSeverity = InfoBarSeverity.Success;
                    InfoText += $"Es ist die aktuellste LiftDataManagerVersion installiert.";
                    _logger.LogInformation(60181, "LiftDataManagerVersion is up to date");
                    break;
                case PackageUpdateAvailability.Unknown:
                default:
                    InfoBarVersionsUpdateIsOpen = true;
                    InfoBarVersionsUpdateText = $"Keine Updateinformationen für {package.DisplayName} gefunden!";
                    InfoBarVersionsUpdateSeverity = InfoBarSeverity.Warning;
                    InfoText += $"Keine Updateinformationen für {package.DisplayName} gefunden\n";
                    _logger.LogWarning(60181, "No update information associated with app {DisplayName}", package.DisplayName);
                    break;
            }
        }
        else
        {
            InfoBarVersionsUpdateIsOpen = true;
            InfoBarVersionsUpdateText = $"Kein installierte App gefunden!";
            InfoBarVersionsUpdateSeverity = InfoBarSeverity.Error;
            InfoText += $"Kein installierte App gefunden\n";
            _logger.LogError(61082, "No found a installed app");
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task UpdateFilePathAsync(string program)
    {
        var filePicker = App.MainWindow.CreateOpenFilePicker();
        filePicker.ViewMode = PickerViewMode.Thumbnail;
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.FileTypeFilter.Add("*");
        StorageFile file = await filePicker.PickSingleFileAsync();

        var newFilePath = (file is not null) ? file.Path : string.Empty;

        switch (program)
        {
            case nameof(PathCFP):
                PathCFP = newFilePath;
                break;
            case nameof(PathZALift):
                PathZALift = newFilePath;
                break;
            case nameof(PathLilo):
                PathLilo = newFilePath;
                break;
            case nameof(PathExcel):
                PathExcel = newFilePath;
                break;
            case nameof(PathDataBase):
                PathDataBase = newFilePath;
                break;
            default:
                break;
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

    [RelayCommand]
    private static async Task OpenLogFolder()
    {
        string path = Path.Combine(Path.GetTempPath(), "LiftDataManager");
        if (Directory.Exists(path))
            Process.Start("explorer.exe", path);
        await Task.CompletedTask;
    }

    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();
        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private void GetProgrammsPath()
    {
        var liloRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\BUCHER\LILO");
        if (liloRegistryKey is not null)
        {
            var liloPath = Registry.GetValue(liloRegistryKey.Name, "basepath", "")?.ToString();

            if (!string.IsNullOrWhiteSpace(liloPath))
            {
                PathLilo = Path.Combine(liloPath, "PRG", "LILO.EXE");
            }
            else
            {
                PathLilo = string.Empty;
            }
        }
        else
        {
            PathLilo = string.Empty;
        }
        PathCFP = _settingService.PathCFP;
        PathZALift = _settingService.PathZALift;
        PathExcel = _settingService.PathExcel;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (CurrentSpeziProperties is not null)
            CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        GetProgrammsPath();
        Adminmode = _settingService.Adminmode;
        CustomAccentColor = _settingService.CustomAccentColor;
        PathDataBase = _settingService.PathDataBase;
        selectedLogLevel = _settingService.LogLevel;
        AutoSave = _settingService.AutoSave;
        AutoSavePeriod = _settingService.AutoSavePeriod;
        TonerSaveMode = _settingService.TonerSaveMode;
        LowHighlightMode = _settingService.LowHighlightMode;
    }
    public void OnNavigatedFrom()
    {
    }
}