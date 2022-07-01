﻿using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace LiftDataManager.ViewModels;

public class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private const string adminpasswort = "2342";
    private readonly IThemeSelectorService _themeSelectorService;
    private ElementTheme _elementTheme;
    private CurrentSpeziProperties _CurrentSpeziProperties;
    private readonly ISettingService _settingService;
    private readonly IAuswahlParameterDataService _auswahlParameterDataService;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService, IAuswahlParameterDataService auswahlParameterDataService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _settingService = settingsSelectorService;
        _auswahlParameterDataService = auswahlParameterDataService;
        VersionDescription = GetVersionDescription();

        PinDialog = new RelayCommand<ContentDialog>(PinDialogAsync);
        UpdateAuswahlParameter = new AsyncRelayCommand(UpdateAuswahlParameterAsync, () => true);
        SwitchAccentColorCommand = new RelayCommand(SwitchAccentColor);
    }

    public IRelayCommand PinDialog
    {
        get;
    }
    public IAsyncRelayCommand UpdateAuswahlParameter
    {
        get;
    }
    public IRelayCommand SwitchAccentColorCommand
    {
        get;
    }
    private ICommand _switchThemeCommand;


    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    private string _versionDescription;

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand
    {
        get
        {
            if (_switchThemeCommand == null)
            {
                _switchThemeCommand = new RelayCommand<ElementTheme>(
                    async (param) =>
                    {
                        if (ElementTheme != param)
                        {
                            ElementTheme = param;
                            await _themeSelectorService.SetThemeAsync(param);
                        }
                    });
            }

            return _switchThemeCommand;
        }
    }

    private async void PinDialogAsync(ContentDialog pwdDialog)
    {
        if (!Adminmode)
        {
            var result = await pwdDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Adminmode = true;
                InfoText += "Adminmode aktiviert\n";
            }
            else
            {
                Adminmode = false;
                InfoText += "Adminmode deaktiviert\n";
            }
        }
    }

    private bool _CustomColor;
    public bool CustomAccentColor
    {
        get => _CustomColor;
        set
        {
            SetProperty(ref _CustomColor, value);
            _settingService.SetSettingsAsync("AccentColor", value);
            _CurrentSpeziProperties.CustomAccentColor = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private void SwitchAccentColor()
    {
 
        if (CustomAccentColor)
        {
            Application.Current.Resources["SystemAccentColor"] = Color.FromArgb(255, 0, 99, 177);
        }
        else
        {
            var uiSettings = new UISettings();
            Application.Current.Resources["SystemAccentColor"] = uiSettings.GetColorValue(UIColorType.Accent);
        }
    }
    private async Task UpdateAuswahlParameterAsync()
    {
        var watch = Stopwatch.StartNew();
        InfoText += "Daten aus Spezifikation werden geladen\n";
        InfoText += "Loading ...\n";
        var result = await Task.Run(() => _auswahlParameterDataService.UpdateAuswahlparameterAsync());
        var updateDataInfo = new StringBuilder();
        updateDataInfo.AppendLine("Aktualisierte Daten: ");
        updateDataInfo.AppendLine("--------------------");
        updateDataInfo.AppendLine(result);
        InfoText += updateDataInfo;
        var stopTimeMs = watch.ElapsedMilliseconds;
        InfoText += $"Downloadtime:  {stopTimeMs} ms\n";
        InfoText += "Daten erfolgreich geladen\n";
    }

    private bool _Adminmode;
    public bool Adminmode
    {
        get => _Adminmode;
        set
        {
            SetProperty(ref _Adminmode, value);
            _settingService.SetSettingsAsync("Adminmode", value);
            _CurrentSpeziProperties.Adminmode = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    private bool _CanSwitchToAdminmode = false;
    public bool CanSwitchToAdminmode
    {
        get => _CanSwitchToAdminmode;
        set => SetProperty(ref _CanSwitchToAdminmode, value);
    }
    private string _PasswortInfoText = "Kein PIN eingegeben";
    public string PasswortInfoText
    {
        get => _PasswortInfoText;
        set => SetProperty(ref _PasswortInfoText, value);
    }

    private string _InfoText;
    public string InfoText
    {
        get => _InfoText;
        set => SetProperty(ref _InfoText, value);
    }

    private string _PasswortInput;
    public string PasswortInput
    {
        get => _PasswortInput;
        set
        {
            SetProperty(ref _PasswortInput, value);
            CheckpasswortInput();
        }
    }
    private bool _AdminmodeWarningAccepted;
    public bool AdminmodeWarningAccepted
    {
        get => _AdminmodeWarningAccepted;
        set
        {
            SetProperty(ref _AdminmodeWarningAccepted, value);
            CheckCanSwitchToAdminmode();
        }
    }

    private void CheckpasswortInput()
    {

        switch (PasswortInput)
        {
            case "":
                PasswortInfoText = "Kein PIN eingegeben";
                CheckCanSwitchToAdminmode();
                break;
            case adminpasswort:
                PasswortInfoText = "Adminmode Pin korrekt Zugriff gewährt";
                CheckCanSwitchToAdminmode();
                break;
            default:
                PasswortInfoText = "Incorrecter Adminmode Pin";
                CheckCanSwitchToAdminmode();
                break;
        }

    }
    private void CheckCanSwitchToAdminmode()
    {

        if (AdminmodeWarningAccepted == true && PasswortInfoText == "Adminmode Pin korrekt Zugriff gewährt")
        {
            CanSwitchToAdminmode = true;
        }
        else
        {
            CanSwitchToAdminmode = false;
        }
    }

    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();
        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public void OnNavigatedTo(object parameter)
    {
        _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        Adminmode = _settingService.Adminmode;
        CustomAccentColor = _settingService.CustomAccentColor;
    }
    public void OnNavigatedFrom()
    {
    }
}

