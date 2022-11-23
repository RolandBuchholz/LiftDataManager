using LiftDataManager.Models;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace LiftDataManager.ViewModels;

public partial class ErrorViewModel : DataViewModelBase, INavigationAware
{
    private readonly ISettingService _settingService;


    public ErrorViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ISettingService settingsSelectorService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
    }

    public ErrorPageInfo? ErrorPageInfo { get; set; }
    public bool CustomAccentColor { get; set; }
    public string? ParamterDictionaryInfo { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Exception { get; set; }

    public bool CanConnectDataBase => _parameterDataService!.CanConnectDataBase();

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

    private void SetErrorValues()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();

        if(CurrentSpeziProperties != null )
        {
            Adminmode = CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml= CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = CurrentSpeziProperties.CheckOut;
            LikeEditParameter= CurrentSpeziProperties.LikeEditParameter;
            HideInfoErrors= CurrentSpeziProperties.HideInfoErrors;
            ParamterDictionaryInfo = CurrentSpeziProperties.ParamterDictionary is null ? "ParamterDictionary nicht geladen" : $"{ParamterDictionary!.Count} Parameter";
        }

        PathCFP = _settingService.PathCFP;
        PathZALift = _settingService.PathZALift;
        PathLilo = _settingService.PathLilo;
        PathExcel = _settingService.PathExcel;
        PathDataBase = _settingService.PathDataBase;
        CustomAccentColor = _settingService.CustomAccentColor;

        if (ErrorPageInfo is not null )
        {
            ErrorMessage = ErrorPageInfo.ErrorArgs.Message;
            Exception = ErrorPageInfo.ErrorArgs.Exception.ToString();
        }
        
    }

    [RelayCommand]
    public void AppShutdown()
    {
        Application.Current.Exit();
    }

    [RelayCommand]
    public void SendErrorLog()
    {
        Application.Current.Exit();
    }

    public void OnNavigatedFrom()
    {
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
