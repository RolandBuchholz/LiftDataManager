﻿namespace LiftDataManager.Contracts.Services;

public interface ISettingService
{
    public bool Adminmode { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? PathCFP { get; set; }
    public string? PathZALift { get; set; }
    public string? PathLilo { get; set; }
    public string? PathExcel { get; set; }
    public string? PathDataBase { get; set; }
    public string? LogLevel { get; set; }

    Task InitializeAsync();
    Task SetSettingsAsync(string key, object setting);
}
