using LiftDataManager.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace LiftDataManager.ViewModels;

public partial class ErrorViewModel : ObservableRecipient, INavigationAwareEx
{
    private readonly ISettingService _settingService;
    private readonly IParameterDataService _parameterDataService;

    public ErrorViewModel(ISettingService settingsSelectorService, IParameterDataService parameterDataService)
    {
        _settingService = settingsSelectorService;
        _parameterDataService = parameterDataService;
    }

    [ObservableProperty]
    public partial string? FullPathXml { get; set; }

    [ObservableProperty]
    public partial bool Adminmode { get; set; }

    [ObservableProperty]
    public partial bool AuftragsbezogeneXml { get; set; }

    [ObservableProperty]
    public partial bool CheckOut { get; set; }

    [ObservableProperty]
    public partial bool LikeEditParameter { get; set; }

    [ObservableProperty]
    public partial bool HideInfoErrors { get; set; }

    public ErrorPageInfo? ErrorPageInfo { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? ParameterDictionaryInfo { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Exception { get; set; }
    public string? LogFile => GetLastLogFile();

    private string? GetLastLogFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "LiftDataManager");
        if (Directory.Exists(path))
        {
            var logs = Directory.GetFiles(path, "*.json");
            return logs.OrderDescending().FirstOrDefault();
        }
        else
        {
            return string.Empty;
        }
    }

    [ObservableProperty]
    public partial string? PathCFP { get; set; }
    partial void OnPathCFPChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathCFP))
        {
            _settingService.SetSettingsAsync(nameof(PathCFP), value);
        }
    }

    [ObservableProperty]
    public partial string? PathZALift { get; set; }

    partial void OnPathZALiftChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathZALift))
        {
            _settingService.SetSettingsAsync(nameof(PathZALift), value);
        }
    }

    [ObservableProperty]
    public partial string? PathLilo { get; set; }

    partial void OnPathLiloChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathLilo))
        {
            _settingService.SetSettingsAsync(nameof(PathLilo), value);
        }
    }

    [ObservableProperty]
    public partial string? PathExcel { get; set; }

    partial void OnPathExcelChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathExcel))
        {
            _settingService.SetSettingsAsync(nameof(PathExcel), value);
        }
    }

    [ObservableProperty]
    public partial string? PathDataBase { get; set; }

    partial void OnPathDataBaseChanged(string? value)
    {
        value ??= string.Empty;
        if (!string.Equals(value, _settingService.PathDataBase))
        {
            _settingService.SetSettingsAsync(nameof(PathDataBase), value);
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
    public async Task SaveAllParameterAsync()
    {
        if (FullPathXml is not null)
        {
            await _parameterDataService.SaveAllParameterAsync(FullPathXml, Adminmode);
        }
    }

    private void SetErrorValues()
    {
        var currentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>().Response;

        if (currentSpeziProperties != null)
        {
            Adminmode = currentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = currentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = currentSpeziProperties.CheckOut;
            LikeEditParameter = currentSpeziProperties.LikeEditParameter;
            HideInfoErrors = currentSpeziProperties.HideInfoErrors;
            FullPathXml = currentSpeziProperties.FullPathXml;
            var parameterDictionary = _parameterDataService.GetParameterDictionary();
            ParameterDictionaryInfo = parameterDictionary?.Count == 0 ? "ParameterDictionary nicht geladen" : $"{parameterDictionary?.Count} Parameter";
        }

        PathCFP = _settingService.PathCFP;
        PathZALift = _settingService.PathZALift;
        PathLilo = _settingService.PathLilo;
        PathExcel = _settingService.PathExcel;
        PathDataBase = _settingService.PathDataBase;
        CustomAccentColor = _settingService.CustomAccentColor;

        if (ErrorPageInfo is not null)
        {
            ErrorMessage = ErrorPageInfo.ErrorArgs.Message;
            Exception = ErrorPageInfo.ErrorArgs.Exception.ToString();
        }
    }

    [RelayCommand]
    public void AppShutdown()
    {
        App.Current.DisableSaveWarning();
        Application.Current.Exit();
    }

    [RelayCommand]
    public void SendErrorLog()
    {
        var app = new Outlook.Application();
        Outlook.MailItem mailItem = app.CreateItem(Outlook.OlItemType.olMailItem);
        mailItem.To = "roland.buchholz@berchtenbreiter-gmbh.de";
        mailItem.Subject = "LogFile LiftDataManager";
        mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
        mailItem.Body = ErrorMessage;
        mailItem.Attachments.Add(LogFile, Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
        mailItem.Display(false);
        mailItem.Send();
        App.Current.DisableSaveWarning();
        Application.Current.Exit();
    }

    public void OnNavigatedFrom()
    {
        App.Current.DisableSaveWarning();
        Application.Current.Exit();
    }
    public void OnNavigatedTo(object parameter)
    {
        if (parameter != null && parameter is ErrorPageInfo)
        {
            ErrorPageInfo = parameter as ErrorPageInfo;
        }
        SetErrorValues();
    }
}
