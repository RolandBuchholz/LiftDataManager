﻿namespace LiftDataManager.Services;

public class SettingsService : ISettingService
{
    private const string SettingsKeyAdminmode = "AppAdminmodeRequested";
    private const string SettingsKeyDefaultAccentColor = "AppDefaultAccentColorRequested";
    private const string SettingsKeyPathCFP = "AppPathCFPRequested";
    private const string SettingsKeyPathZALift = "AppPathZALiftRequested";
    private const string SettingsKeyPathLilo = "AppPathLiloRequested";
    private const string SettingsKeyPathExcel = "AppPathExcelRequested";

    private readonly ILocalSettingsService _localSettingsService;

    public SettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public bool Adminmode{get; set;}
    public bool CustomAccentColor{get; set;}
    public string? PathCFP { get; set; }
    public string? PathZALift { get; set; }
    public string? PathLilo { get; set; }
    public string? PathExcel { get; set; }

    public async Task InitializeAsync()
    {
        await LoadSettingsAsync("Adminmode");
        await LoadSettingsAsync("AccentColor");
        await LoadSettingsAsync("PathCFP");
        await LoadSettingsAsync("PathZALift");
        await LoadSettingsAsync("PathLilo");
        await LoadSettingsAsync("PathExcel");
        await Task.CompletedTask;
    }

    public async Task SetSettingsAsync(string key, object value)
    {
        switch (key)
        {
            case "Adminmode":
                Adminmode = (bool)value;
                await SaveSettingsAsync(key, Adminmode);
                return;
            case "AccentColor":
                CustomAccentColor = (bool)value;
                await SaveSettingsAsync(key, CustomAccentColor);
                return;
            case "PathCFP":
                PathCFP = (string)value;
                await SaveSettingsAsync(key, PathCFP);
                return;
            case "PathZALift":
                PathZALift = (string)value;
                await SaveSettingsAsync(key, PathZALift);
                return;
            case "PathLilo":
                PathLilo = (string)value;
                await SaveSettingsAsync(key, PathLilo);
                return;
            case "PathExcel":
                PathLilo = (string)value;
                await SaveSettingsAsync(key, PathExcel);
                return;
            default:
                return;
        }
    }

    private async Task LoadSettingsAsync(string key)
    {
        switch (key)
        {
            case "Adminmode":
                var storedAdminmode = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyAdminmode);
                Adminmode = !string.IsNullOrWhiteSpace(storedAdminmode) && Convert.ToBoolean(storedAdminmode);
                return;
            case "AccentColor":
                var storedDefaultAccentColor = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyDefaultAccentColor);
                CustomAccentColor = !string.IsNullOrWhiteSpace(storedDefaultAccentColor) && Convert.ToBoolean(storedDefaultAccentColor);
                return;
            case "PathCFP":
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
                return;
            case "PathZALift":
                var storedPathZALift = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathZALift);
                if (!string.IsNullOrWhiteSpace(storedPathZALift))
                {
                    PathZALift = storedPathZALift;
                }
                else
                {
                    PathZALift = @"C:\Program Files (x86)\zetalift\Lift.exe";
                }
                return;
            case "PathLilo":
                var storedPathLilo = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathLilo);
                if (!string.IsNullOrWhiteSpace(storedPathLilo))
                {
                    PathLilo = storedPathLilo;
                }
                else
                {
                    PathLilo = @"C:\Program Files (x86)\BucherHydraulics\LILO\PRG\LILO.EXE";
                }
                return;
            case "PathExcel":
                var storedPathExcel = await _localSettingsService.ReadSettingAsync<string>(SettingsKeyPathExcel);
                if (!string.IsNullOrWhiteSpace(storedPathExcel))
                {
                    PathExcel = storedPathExcel;
                }
                else
                {
                    PathExcel = @"C:\Program Files (x86)\Microsoft Office\Office16\EXCEL.EXE";
                }
                return;
            default:
                return;
        }
    }

    private async Task SaveSettingsAsync(string key, object value)
    {
        switch (key)
        {
            case "Adminmode":
                await _localSettingsService.SaveSettingAsync(SettingsKeyAdminmode, ((bool)value).ToString());
                return;
            case "AccentColor":
                await _localSettingsService.SaveSettingAsync(SettingsKeyDefaultAccentColor, ((bool)value).ToString());
                return;
            case "PathCFP":
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathCFP, value);
                return;
            case "PathZALift":
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathZALift, value);
                return;
            case "PathLilo":
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathLilo, value);
                return;
            case "PathExcel":
                await _localSettingsService.SaveSettingAsync(SettingsKeyPathExcel, value);
                return;
            default:
                return;
        }
    }
}





