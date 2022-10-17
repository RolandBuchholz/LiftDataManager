namespace LiftDataManager.Services;

public class SettingsService : ISettingService
{
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private const string SettingsKeyCustomAccentColor = "AppCustomAccentColorRequested";
    private const string SettingsKeyPathCFP = "AppPathCFPRequested";
    private const string SettingsKeyPathZALift = "AppPathZALiftRequested";
    private const string SettingsKeyPathLilo = "AppPathLiloRequested";
    private const string SettingsKeyPathExcel = "AppPathExcelRequested";
    private const string SettingsKeyPathDataBase = "AppPathDataBaseRequested";

    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public bool Adminmode { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? PathCFP { get; set; }
    public string? PathZALift { get; set; }
    public string? PathLilo { get; set; }
    public string? PathExcel { get; set; }
    public string? PathDataBase { get; set; }

    public async Task InitializeAsync()
    {
        await LoadSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetSettingsAsync(string key, object value)
    {
        switch (key)
        {
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
            default:
                return;
        }
    }

    private async Task LoadSettingsAsync()
    {
        var storedAdminmode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAdminmode);
        Adminmode = !string.IsNullOrWhiteSpace(storedAdminmode) && Convert.ToBoolean(storedAdminmode);

        var storedCustomAccentColor = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyCustomAccentColor);
        CustomAccentColor = !string.IsNullOrWhiteSpace(storedCustomAccentColor) && Convert.ToBoolean(storedCustomAccentColor);

        var storedPathCFP = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathCFP);
        if (!string.IsNullOrWhiteSpace(storedPathCFP))
        {
            PathCFP = storedPathCFP;
        }
        else
        {
            var user = Environment.GetEnvironmentVariable("userprofile");
            PathCFP = user + @"\AppData\Local\Bausatzauslegung\CFP\UpdateCFP.exe";
        }

        var storedPathZALift = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathZALift);
        if (!string.IsNullOrWhiteSpace(storedPathZALift))
        {
            PathZALift = storedPathZALift;
        }
        else
        {
            PathZALift = @"C:\Program Files (x86)\zetalift\Lift.exe";
        }

        var storedPathLilo = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathLilo);
        if (!string.IsNullOrWhiteSpace(storedPathLilo))
        {
            PathLilo = storedPathLilo;
        }
        else
        {
            PathLilo = @"C:\Program Files (x86)\BucherHydraulics\LILO\PRG\LILO.EXE";
        }

        var storedPathExcel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathExcel);
        if (!string.IsNullOrWhiteSpace(storedPathExcel))
        {
            PathExcel = storedPathExcel;
        }
        else
        {
            PathExcel = @"C:\Program Files (x86)\Microsoft Office\Office16\EXCEL.EXE";
        }

        var storedPathDataBase = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathDataBase);
        if (!string.IsNullOrWhiteSpace(storedPathDataBase))
        {
            PathDataBase = storedPathDataBase;
        }
        else
        {
            PathDataBase = @"\\Bauer\AUFTRÄGE NEU\Vorlagen\DataBase\LiftDataParameter.db";
        }
    }

    private async Task SaveSettingsAsync(string key, object value)
    {
        switch (key)
        {
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
            default:
                return;
        }
    }
}





