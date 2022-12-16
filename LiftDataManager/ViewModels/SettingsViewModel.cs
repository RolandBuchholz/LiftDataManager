using LiftDataManager.Core.DataAccessLayer.Models;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private const string adminpasswort = "2342";
    private readonly IThemeSelectorService _themeSelectorService;
    private CurrentSpeziProperties? CurrentSpeziProperties = new();
    private readonly ISettingService _settingService;
    private readonly IDialogService _dialogService;
    private readonly ParameterContext _parametercontext;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService, IDialogService dialogService, ParameterContext parametercontext)
    {
        _themeSelectorService = themeSelectorService;
        _settingService = settingsSelectorService;
        _dialogService = dialogService;
        _parametercontext = parametercontext;
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

#pragma warning disable CA1822 // Member als statisch markieren
    public string UserName => string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) ? "no user detected" : System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    public OperatingSystem OSVersion => Environment.OSVersion;
    public string RuntimeInformation => string.IsNullOrWhiteSpace(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription) ? "no runtime detected" : System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    public string Vault2023Installed => File.Exists(@"C:\\Programme\\Autodesk\\Vault Client 2023\Explorer\\Connectivity.VaultPro.exe") ? "installiert" : "nicht installiert";
    public string VDSInstalled => Directory.Exists(@"C:\\ProgramData\\Autodesk\\Vault 2023\\Extensions\\DataStandard") ? "installiert" : "nicht installiert";
    public string PowerVaultInstalled => File.Exists(@"C:\\Program Files\\coolOrange\\Modules\\powerVault\\powerVault.Cmdlets.dll") ? "installiert" : "nicht installiert";
    public string AdskLicensingSDKInstalled => File.Exists(@"C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\AdskLicensingSDK_6.dll") ? "installiert" : "nicht installiert";
#pragma warning restore CA1822 // Member als statisch markieren

    public List<string> LogLevel = new() { "Verbose", "Debug", "Information", "Warning", "Error" , "Fatal" };

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
        await _dialogService.MessageDialogAsync("Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    [RelayCommand]
    private async Task UpdateAuswahlParameterAsync()
    {
        InfoText += "Datentransfer zwischen Excel-Spezifikation und \n";
        InfoText += "LiftDataManager wurde durch SQLite ersetzt \n";
        InfoText += "Setzen Sie den Pfad zur Datenbank! \n";
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
        PathDataBase = _settingService.PathDataBase;
        selectedLogLevel = _settingService.LogLevel;
    }
    public void OnNavigatedFrom()
    {
    }
}