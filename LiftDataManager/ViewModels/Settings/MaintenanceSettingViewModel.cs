using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LiftDataManager.ViewModels;

public partial class MaintenanceSettingViewModel : ObservableRecipient, INavigationAwareEx
{
    private readonly ISettingService _settingService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<MaintenanceSettingViewModel> _logger;
    public MaintenanceSettingViewModel(ISettingService settingService, IDialogService dialogService, ILogger<MaintenanceSettingViewModel> logger)
    {
        _settingService = settingService;
        _dialogService = dialogService;
        _logger = logger;
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

    public List<string> LogLevel = ["Verbose", "Debug", "Information", "Warning", "Error", "Fatal"];

    [ObservableProperty]
    public partial string? SelectedLogLevel { get; set; }
    partial void OnSelectedLogLevelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.LogLevel))
            _settingService.SetSettingsAsync(nameof(LogLevel), value);
    }
    [ObservableProperty]
    public partial string? PathCFP { get; set; }
    partial void OnPathCFPChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathCFP))
            _settingService.SetSettingsAsync(nameof(PathCFP), value);
    }

    [ObservableProperty]
    public partial string? PathZALift { get; set; }
    partial void OnPathZALiftChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathZALift))
            _settingService.SetSettingsAsync(nameof(PathZALift), value);
    }

    [ObservableProperty]
    public partial string? PathLilo { get; set; }
    partial void OnPathLiloChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathLilo))
            _settingService.SetSettingsAsync(nameof(PathLilo), value);
    }
    [ObservableProperty]
    public partial string? PathExcel { get; set; }
    partial void OnPathExcelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathExcel))
            _settingService.SetSettingsAsync(nameof(PathExcel), value);
    }
    [ObservableProperty]
    public partial string? PathDataBase { get; set; }
    partial void OnPathDataBaseChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathDataBase))
            _settingService.SetSettingsAsync(nameof(PathDataBase), value);
    }
    [ObservableProperty]
    public partial string? InfoBarVersionsUpdateText { get; set; }
    [ObservableProperty]
    public partial bool InfoBarVersionsUpdateIsOpen { get; set; }
    [ObservableProperty]
    public partial InfoBarSeverity InfoBarVersionsUpdateSeverity { get; set; }
    [RelayCommand]
    private async Task UpdateCheckAsync()
    {
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
                        _logger.LogInformation(60181, "LiftDataManagerupdate found and installing...");
                        await pm.AddPackageByAppInstallerFileAsync(new Uri(@"\\Bauer\AUFTRÄGE NEU\Vorlagen\LiftDataManager\LiftDataManager_x64.appinstaller"),
                            AddPackageByAppInstallerOptions.None, pm.GetDefaultPackageVolume());
                    }
                    break;
                case PackageUpdateAvailability.NoUpdates:
                    InfoBarVersionsUpdateIsOpen = true;
                    InfoBarVersionsUpdateText = $"Es ist die aktuellste LiftDataManagerVersion installiert.";
                    InfoBarVersionsUpdateSeverity = InfoBarSeverity.Success;
                    _logger.LogInformation(60181, "LiftDataManagerVersion is up to date");
                    break;
                case PackageUpdateAvailability.Unknown:
                default:
                    InfoBarVersionsUpdateIsOpen = true;
                    InfoBarVersionsUpdateText = $"Keine Updateinformationen für {package.DisplayName} gefunden!";
                    InfoBarVersionsUpdateSeverity = InfoBarSeverity.Warning;
                    _logger.LogWarning(60181, "No update information associated with app {DisplayName}", package.DisplayName);
                    break;
            }
        }
        else
        {
            InfoBarVersionsUpdateIsOpen = true;
            InfoBarVersionsUpdateText = $"Kein installierte App gefunden!";
            InfoBarVersionsUpdateSeverity = InfoBarSeverity.Error;
            _logger.LogError(61082, "No found a installed app");
        }
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
    [RelayCommand]
    private static async Task OpenLogFolder()
    {
        string path = Path.Combine(Path.GetTempPath(), "LiftDataManager");
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
        await Task.CompletedTask;
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

        var zaLiftRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\VB and VBA Program Settings\ZETALIFT\All");
        if (zaLiftRegistryKey is not null)
        {
            var zaLiftPath = Registry.GetValue(zaLiftRegistryKey.Name, "InstallPath", "")?.ToString();
            if (!string.IsNullOrWhiteSpace(zaLiftPath))
            {
                PathZALift = Path.Combine(zaLiftPath, "ZAlift.exe");
            }
            else
            {
                PathZALift = string.Empty;
            }
        }
        else
        {
            PathZALift = string.Empty;
        }

        var cfpLiftRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\BERCHTENBREITER_GMBH\CARFRAMEPROGRAM");
        if (cfpLiftRegistryKey is not null)
        {
            var cfpLiftPath = Registry.GetValue(cfpLiftRegistryKey.Name, "Path: ", "")?.ToString();
            if (!string.IsNullOrWhiteSpace(cfpLiftPath))
            {
                PathCFP = Path.Combine(cfpLiftPath, "UpdateCFP.exe");
            }
            else
            {
                PathCFP = string.Empty;
            }
        }
        else
        {
            PathCFP = string.Empty;
        }

        PathExcel = _settingService.PathExcel;
    }

    public void OnNavigatedTo(object parameter)
    {
        GetProgrammsPath();
        PathDataBase = _settingService.PathDataBase;
        SelectedLogLevel = _settingService.LogLevel;
    }
    public void OnNavigatedFrom()
    {

    }
}
