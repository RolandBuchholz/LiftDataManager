using Windows.System;

namespace LiftDataManager.ViewModels;

public partial class ThemeSettingViewModel : ObservableRecipient, INavigationAwareEx
{
    private readonly IThemeService _themeService;
    private readonly ISettingService _settingService;
    private readonly IDialogService _dialogService;
    private CurrentSpeziProperties _currentSpeziProperties;
    public ThemeSettingViewModel(IThemeService themeService, IDialogService dialogService, ISettingService settingService)
    {
        _currentSpeziProperties ??= new();
        _themeService = themeService;
        _settingService = settingService;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    public partial bool CustomAccentColor { get; set; }
    partial void OnCustomAccentColorChanged(bool value)
    {
        _settingService.SetSettingsAsync(nameof(CustomAccentColor), value);
        if (_currentSpeziProperties is not null)
        {
            _currentSpeziProperties.CustomAccentColor = value;
            Messenger.Send(new SpeziPropertiesChangedMessage(_currentSpeziProperties));
        }
    }
    [RelayCommand]
    private void OnBackdropChanged(object sender)
    {
        _themeService.OnBackdropComboBoxSelectionChanged(sender);
    }

    [RelayCommand]
    private void OnThemeChanged(object sender)
    {
        _themeService.OnThemeComboBoxSelectionChanged(sender);
    }
    [RelayCommand]
    private static async Task OpenWindowsColorSettings()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
    }

    [RelayCommand]
    private async Task SwitchAccentColorAsync()
    {
        await _dialogService.MessageDialogAsync("Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    public void SetDefaultTheme(ComboBox cmbTheme)
    {
        _themeService.SetThemeComboBoxDefaultItem(cmbTheme);
    }

    public void SetDefaultBackdrop(ComboBox cmbBackdrop)
    {
        _themeService.SetBackdropComboBoxDefaultItem(cmbBackdrop);
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
