using DevWinUI;

namespace LiftDataManager.ViewModels;

public partial class ThemeSettingViewModel : ObservableRecipient, INavigationAwareEx
{
    public readonly IThemeService ThemeService;
    private readonly ISettingService _settingService;
    private readonly IDialogService _dialogService;
    private CurrentSpeziProperties _currentSpeziProperties;
    public ThemeSettingViewModel(IThemeService themeService, IDialogService dialogService, ISettingService settingService)
    {
        _currentSpeziProperties ??= new();
        ThemeService = themeService;
        _settingService = settingService;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    public partial bool CustomAccentColor { get; set; }

    [ObservableProperty]
    public partial SolidColorBrush TintColor { get; set; } = new SolidColorBrush();
    partial void OnCustomAccentColorChanged(bool value)
    {
        _settingService.SetSettingsAsync(nameof(CustomAccentColor), value);
        if (_currentSpeziProperties is not null)
        {
            _currentSpeziProperties.CustomAccentColor = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(_currentSpeziProperties));
        }
    }

    public DropdownColorPicker? MainDropdownColorPicker { get; set; }

    [RelayCommand]
    private async Task SwitchAccentColorAsync()
    {
        await _dialogService.MessageDialogAsync("Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    [RelayCommand]
    private void MainDropdownColorPickerLoaded(DropdownColorPicker dropdownColorPicker)
    {
        MainDropdownColorPicker = dropdownColorPicker;
    }

    [RelayCommand]
    private void TintColorChanged(DropdownColorPickerColorChangedEventArgs e)
    {
        SetTintColor(e.Color);
    }

    [RelayCommand]
    private void TintColorPaletteItemClick(ColorPaletteColorChangedEventArgs e)
    {
        SetTintColor(e.Color);
        MainDropdownColorPicker?.Color = e.Color;
    }

    private void SetTintColor(Color color)
    {
        if (color.ToString().Contains("#FF000000") || color.ToString().Contains("#000000"))
        {
            ThemeService.ResetBackdropProperties();
        }
        else
        {
            ThemeService.SetBackdropTintColor(color);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        _currentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        CustomAccentColor = _settingService.CustomAccentColor;
    }
    public void OnNavigatedFrom()
    {
    }
}
