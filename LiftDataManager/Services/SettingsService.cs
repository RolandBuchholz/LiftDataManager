namespace LiftDataManager.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="ISettingService"/> <see langword="interface"/> using settingService
/// </summary>
public class SettingsService : ISettingService
{
    private const string SettingsKeyFirstSetup = "AppFirstSetupRequested";
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private const string SettingsKeySafetycomponentEditormode = "AppSafetycomponentEditormodeRequested";
    private const string SettingsKeyCustomAccentColor = "AppCustomAccentColorRequested";
    private const string SettingsKeyPathCFP = "AppPathCFPRequested";
    private const string SettingsKeyPathZALift = "AppPathZALiftRequested";
    private const string SettingsKeyPathLilo = "AppPathLiloRequested";
    private const string SettingsKeyPathExcel = "AppPathExcelRequested";
    private const string SettingsKeyPathDataStorage = "AppPathDataStorageRequested";
    private const string SettingsKeyPathDataBase = "AppPathDataBaseRequested";
    private const string SettingsKeyPathDataBaseSafetyComponents = "AppPathDataBaseSafetyComponentsRequested";
    private const string SettingsKeyLogLevel = "AppLogLevelRequested";
    private const string SettingsKeyAutoSave = "AppAutoSaveRequested";
    private const string SettingsKeyAutoSavePeriod = "AppAutoSavePeriodRequested";
    private const string SettingsKeyTonerSaveMode = "AppTonerSaveModeRequested";
    private const string SettingsKeyLowHighlightMode = "AppLowHighlightModeRequested";
    private const string SettingsKeyAutoOpenInfoCenter = "AppAutoOpenInfoCenterRequested";
    private const string SettingsKeyVaultDisabled = "AppVaultDisabledRequested";

    private const string LiftDataParameterDBPath = @"\\Bauer\AUFTRÄGE NEU\Vorlagen\DataBase\LiftDataParameter.db";
    private const string SafetyComponentRecordsDBPath = @"\\Bauer\AUFTRÄGE NEU\Vorlagen\DataBase\SafetyComponentRecords.db";

    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    private bool FirstSetup { get; set; }
    public bool Adminmode { get; set; }
    public bool SafetycomponentEditormode { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? PathCFP { get; set; }
    public string? PathZALift { get; set; }
    public string? PathLilo { get; set; }
    public string? PathExcel { get; set; }
    public string? PathDataStorage { get; set; }
    public string? PathDataBase { get; set; }
    public string? PathDataBaseSafetyComponents { get; set; }
    public string? LogLevel { get; set; }
    public bool AutoSave { get; set; }
    public string? AutoSavePeriod { get; set; }
    public bool TonerSaveMode { get; set; }
    public bool LowHighlightMode { get; set; }
    public bool AutoOpenInfoCenter { get; set; }
    public bool VaultDisabled { get; set; }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await LoadSettingsAsync();
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SetSettingsAsync(string key, object value)
    {
        switch (key)
        {
            case nameof(FirstSetup):
                FirstSetup = (bool)value;
                await SaveSettingsAsync(key, FirstSetup);
                return;
            case nameof(Adminmode):
                Adminmode = (bool)value;
                await SaveSettingsAsync(key, Adminmode);
                return;
            case nameof(SafetycomponentEditormode):
                SafetycomponentEditormode = (bool)value;
                await SaveSettingsAsync(key, SafetycomponentEditormode);
                return;
            case nameof(CustomAccentColor):
                CustomAccentColor = (bool)value;
                await SaveSettingsAsync(key, CustomAccentColor);
                return;
            case nameof(PathCFP):
                PathCFP = (string)value;
                await SaveSettingsAsync(key, PathCFP);
                return;
            case nameof(PathZALift):
                PathZALift = (string)value;
                await SaveSettingsAsync(key, PathZALift);
                return;
            case nameof(PathLilo):
                PathLilo = (string)value;
                await SaveSettingsAsync(key, PathLilo);
                return;
            case nameof(PathExcel):
                PathExcel = (string)value;
                await SaveSettingsAsync(key, PathExcel);
                return;
            case nameof(PathDataStorage):
                PathDataStorage = (string)value;
                await SaveSettingsAsync(key, PathDataStorage);
                return;
            case nameof(PathDataBase):
                PathDataBase = (string)value;
                await SaveSettingsAsync(key, PathDataBase);
                return;
            case nameof(PathDataBaseSafetyComponents):
                PathDataBaseSafetyComponents = (string)value;
                await SaveSettingsAsync(key, PathDataBaseSafetyComponents);
                return;
            case nameof(LogLevel):
                LogLevel = (string)value;
                await SaveSettingsAsync(key, LogLevel);
                return;
            case nameof(AutoSave):
                AutoSave = (bool)value;
                await SaveSettingsAsync(key, AutoSave);
                return;
            case nameof(AutoSavePeriod):
                AutoSavePeriod = (string)value;
                await SaveSettingsAsync(key, AutoSavePeriod);
                return;
            case nameof(TonerSaveMode):
                TonerSaveMode = (bool)value;
                await SaveSettingsAsync(key, TonerSaveMode);
                return;
            case nameof(LowHighlightMode):
                LowHighlightMode = (bool)value;
                await SaveSettingsAsync(key, LowHighlightMode);
                return;
            case nameof(AutoOpenInfoCenter):
                AutoOpenInfoCenter = (bool)value;
                await SaveSettingsAsync(key, AutoOpenInfoCenter);
                return;
            case nameof(VaultDisabled):
                VaultDisabled = (bool)value;
                await SaveSettingsAsync(key, VaultDisabled);
                return;
            default:
                return;
        }
    }

    private async Task LoadSettingsAsync()
    {
        var storedFirstSetup = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyFirstSetup);
        FirstSetup = !string.IsNullOrWhiteSpace(storedFirstSetup) && Convert.ToBoolean(storedFirstSetup);

        if (!FirstSetup)
        {
            await SetDefaultPaths();
        }

        var storedAdminmode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAdminmode);
        Adminmode = !string.IsNullOrWhiteSpace(storedAdminmode) && Convert.ToBoolean(storedAdminmode);
        var storedSafetycomponentEditormode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeySafetycomponentEditormode);
        SafetycomponentEditormode = !string.IsNullOrWhiteSpace(storedSafetycomponentEditormode) && Convert.ToBoolean(storedSafetycomponentEditormode);
        var storedCustomAccentColor = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyCustomAccentColor);
        CustomAccentColor = !string.IsNullOrWhiteSpace(storedCustomAccentColor) && Convert.ToBoolean(storedCustomAccentColor);
        PathCFP = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathCFP);
        PathZALift = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathZALift);
        PathLilo = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathLilo);
        PathExcel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathExcel);
        PathDataStorage = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathDataStorage);
        var storedPathDataBase = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathDataBase);
        PathDataBase = !string.IsNullOrWhiteSpace(storedPathDataBase) ? storedPathDataBase : LiftDataParameterDBPath;
        var storedPathDataBaseSafetyComponent = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathDataBaseSafetyComponents);
        PathDataBaseSafetyComponents = !string.IsNullOrWhiteSpace(storedPathDataBaseSafetyComponent) ? storedPathDataBaseSafetyComponent : SafetyComponentRecordsDBPath;
        LogLevel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyLogLevel);
        var storedAutoSave = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAutoSave);
        AutoSave = !string.IsNullOrWhiteSpace(storedAutoSave) && Convert.ToBoolean(storedAutoSave);
        AutoSavePeriod = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAutoSavePeriod);
        var storedTonerSaveMode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyTonerSaveMode);
        TonerSaveMode = !string.IsNullOrWhiteSpace(storedTonerSaveMode) && Convert.ToBoolean(storedTonerSaveMode);
        var storedLowHighlightMode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyLowHighlightMode);
        LowHighlightMode = !string.IsNullOrWhiteSpace(storedLowHighlightMode) && Convert.ToBoolean(storedLowHighlightMode);
        var storedAutoOpenInfoCenter = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAutoOpenInfoCenter);
        AutoOpenInfoCenter = string.IsNullOrWhiteSpace(storedAutoOpenInfoCenter) ? true : Convert.ToBoolean(storedAutoOpenInfoCenter);
        var storedVaultDisabled = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyVaultDisabled);
        VaultDisabled = !string.IsNullOrWhiteSpace(storedVaultDisabled) && Convert.ToBoolean(storedVaultDisabled);
    }

    private async Task SaveSettingsAsync(string key, object value)
    {
        switch (key)
        {
            case nameof(FirstSetup):
                await _localSettingsService.SaveSettingAsync(SettingsKeyFirstSetup, ((bool)value).ToString());
                return;
            case nameof(Adminmode):
                await _localSettingsService.SaveSettingAsync(SettingsKeyAdminmode, ((bool)value).ToString());
                return;
            case nameof(SafetycomponentEditormode):
                await _localSettingsService.SaveSettingAsync(SettingsKeySafetycomponentEditormode, ((bool)value).ToString());
                return;
            case nameof(CustomAccentColor):
                await _localSettingsService.SaveSettingAsync(SettingsKeyCustomAccentColor, ((bool)value).ToString());
                return;
            case nameof(PathCFP):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathCFP, value);
                return;
            case nameof(PathZALift):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathZALift, value);
                return;
            case nameof(PathLilo):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathLilo, value);
                return;
            case nameof(PathExcel):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathExcel, value);
                return;
            case nameof(PathDataStorage):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathDataStorage, value);
                return;
            case nameof(PathDataBase):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathDataBase, value);
                return;
            case nameof(PathDataBaseSafetyComponents):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathDataBaseSafetyComponents, value);
                return;
            case nameof(LogLevel):
                await _localSettingsService.SaveSettingAsync(SettingsKeyLogLevel, value);
                return;
            case nameof(AutoSave):
                await _localSettingsService.SaveSettingAsync(SettingsKeyAutoSave, ((bool)value).ToString());
                return;
            case nameof(AutoSavePeriod):
                await _localSettingsService.SaveSettingAsync(SettingsKeyAutoSavePeriod, value);
                return;
            case nameof(TonerSaveMode):
                await _localSettingsService.SaveSettingAsync(SettingsKeyTonerSaveMode, ((bool)value).ToString());
                return;
            case nameof(LowHighlightMode):
                await _localSettingsService.SaveSettingAsync(SettingsKeyLowHighlightMode, ((bool)value).ToString());
                return;
            case nameof(AutoOpenInfoCenter):
                await _localSettingsService.SaveSettingAsync(SettingsKeyAutoOpenInfoCenter, ((bool)value).ToString());
                return;
            case nameof(VaultDisabled):
                await _localSettingsService.SaveSettingAsync(SettingsKeyVaultDisabled, ((bool)value).ToString());
                return;
            default:
                return;
        }
    }

    private async Task SetDefaultPaths()
    {
        await SetSettingsAsync(nameof(PathCFP), Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"\Bausatzauslegung\CFP\UpdateCFP.exe"));
        await SetSettingsAsync(nameof(PathZALift), @"C:\Program Files (x86)\ZETALIFT\ZAlift.exe");
        await SetSettingsAsync(nameof(PathLilo), @"C:\Program Files (x86)\BucherHydraulics\LILO\PRG\LILO.EXE");
        await SetSettingsAsync(nameof(PathExcel), @"C:\Program Files (x86)\Microsoft Office\Office16\EXCEL.EXE");
        await SetSettingsAsync(nameof(PathDataStorage), Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"\Bausatzauslegung\Auftrag\"));
        await SetSettingsAsync(nameof(PathDataBase), LiftDataParameterDBPath);
        await SetSettingsAsync(nameof(PathDataBaseSafetyComponents), SafetyComponentRecordsDBPath);
        await SetSettingsAsync(nameof(LogLevel), "Information");
        await SetSettingsAsync(nameof(TonerSaveMode), true);
        await SetSettingsAsync(nameof(AutoOpenInfoCenter), true);
        await SetSettingsAsync(nameof(FirstSetup), true);
    }
}