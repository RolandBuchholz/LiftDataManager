namespace LiftDataManager.Services;

public class SettingsService : ISettingService
{
    private const string SettingsKeyFirstSetup = "AppAdminmodeFirstSetup";
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private const string SettingsKeyCustomAccentColor = "AppCustomAccentColorRequested";
    private const string SettingsKeyPathCFP = "AppPathCFPRequested";
    private const string SettingsKeyPathZALift = "AppPathZALiftRequested";
    private const string SettingsKeyPathLilo = "AppPathLiloRequested";
    private const string SettingsKeyPathExcel = "AppPathExcelRequested";
    private const string SettingsKeyPathDataBase = "AppPathDataBaseRequested";
    private const string SettingsKeyLogLevel = "AppLogLevelRequested";
    private const string SettingsKeyAutoSave = "AppAutoSaveRequested";
    private const string SettingsKeyAutoSavePeriod = "AppAutoSavePeriodRequested";

    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    private bool FirstSetup { get; set; }
    public bool Adminmode { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? PathCFP { get; set; }
    public string? PathZALift { get; set; }
    public string? PathLilo { get; set; }
    public string? PathExcel { get; set; }
    public string? PathDataBase { get; set; }
    public string? LogLevel { get; set; }
    public bool AutoSave { get; set; }
    public string? AutoSavePeriod { get; set; }

    public async Task InitializeAsync()
    {
        await LoadSettingsAsync();
        await Task.CompletedTask;
    }

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
            case nameof(PathDataBase):
                PathDataBase = (string)value;
                await SaveSettingsAsync(key, PathDataBase);
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
        var storedCustomAccentColor = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyCustomAccentColor);
        CustomAccentColor = !string.IsNullOrWhiteSpace(storedCustomAccentColor) && Convert.ToBoolean(storedCustomAccentColor);
        PathCFP = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathCFP);
        PathZALift = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathZALift);
        PathLilo = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathLilo);
        PathExcel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathExcel);
        PathDataBase = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathDataBase);
        LogLevel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyLogLevel);
        var storedAutoSave = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAutoSave);
        AutoSave = !string.IsNullOrWhiteSpace(storedAutoSave) && Convert.ToBoolean(storedAutoSave);
        AutoSavePeriod = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAutoSavePeriod);
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
            case nameof(PathDataBase):
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathDataBase, value);
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
            default:
                return;
        }
    }

    private async Task SetDefaultPaths()
    {
        await SetSettingsAsync(nameof(PathCFP), Environment.GetEnvironmentVariable("userprofile") + @"\AppData\Local\Bausatzauslegung\CFP\UpdateCFP.exe");
        await SetSettingsAsync(nameof(PathZALift), @"C:\Program Files (x86)\ZETALIFT\ZAlift.exe");
        await SetSettingsAsync(nameof(PathLilo), @"C:\Program Files (x86)\BucherHydraulics\LILO\PRG\LILO.EXE");
        await SetSettingsAsync(nameof(PathExcel), @"C:\Program Files (x86)\Microsoft Office\Office16\EXCEL.EXE");
        await SetSettingsAsync(nameof(PathDataBase), @"\\Bauer\AUFTRÄGE NEU\Vorlagen\DataBase\LiftDataParameter.db");
        await SetSettingsAsync(nameof(LogLevel), @"Information");
        await SetSettingsAsync(nameof(FirstSetup), true);
    }
}





