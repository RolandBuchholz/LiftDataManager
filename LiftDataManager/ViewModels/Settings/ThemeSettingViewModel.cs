using Windows.System;

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

    [RelayCommand]
    private async Task SwitchAccentColorAsync()
    {
        await _dialogService.MessageDialogAsync("Switch Accent Color", "Accentfarbe wurde geändert und wird nach einen Appneustart aktiviert");
    }

    [RelayCommand]
    private void TintColorChanged(ColorChangedEventArgs args)
    {
        TintColor = new SolidColorBrush(args.NewColor);
        ThemeService.SetBackdropTintColor(args.NewColor);
    }

    [RelayCommand]
    private void TintColorPaletteItemClick(ItemClickEventArgs e)
    {
        if (e.ClickedItem is ColorPaletteItem color)
        {
            if (color.Hex is null || color.Hex.Contains("#000000"))
            {
                ThemeService.ResetBackdropProperties();
            }
            else
            {
                ThemeService.SetBackdropTintColor(color.Color);
            }
            TintColor = new SolidColorBrush(color.Color);
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
