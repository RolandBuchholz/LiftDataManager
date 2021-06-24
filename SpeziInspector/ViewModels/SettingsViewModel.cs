using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpeziInspector.Contracts.Services;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;
using System;
using System.Windows.Input;
using Windows.ApplicationModel;

namespace SpeziInspector.ViewModels
{
    public class SettingsViewModel : ObservableRecipient, INavigationAware
    {
        private const string adminpasswort = "2342";
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISettingService _settingService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private ElementTheme _elementTheme;

        public SettingsViewModel(IThemeSelectorService themeSelectorService, ISettingService settingsSelectorService)
        {
            _themeSelectorService = themeSelectorService;
            _settingService = settingsSelectorService;
            _elementTheme = _themeSelectorService.Theme;
            VersionDescription = GetVersionDescription();
            PinDialog = new RelayCommand<ContentDialog>(PinDialogAsync);
        }

        public IRelayCommand PinDialog { get; }

        private async void PinDialogAsync(ContentDialog pwdDialog)
        {
            if (!Adminmode)
            {
                var result = await pwdDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Adminmode = true;
                }
                else
                {
                    Adminmode = false;
                }
            }
        }

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

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

        private bool _Adminmode;
        public bool Adminmode
        {
            get => _Adminmode;
            set
            {
                SetProperty(ref _Adminmode, value);
                _settingService.SetSettingsAsync(value);
                _CurrentSpeziProperties.Adminmode = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private bool _CanSwitchToAdminmode = false;
        public bool CanSwitchToAdminmode
        {
            get => _CanSwitchToAdminmode;
            set
            {
                SetProperty(ref _CanSwitchToAdminmode, value);
            }
        }

        private string _PasswortInfoText = "Kein PIN eingegeben";
        public string PasswortInfoText
        {
            get => _PasswortInfoText;
            set
            {
                SetProperty(ref _PasswortInfoText, value);
            }
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

        private string GetVersionDescription()
        {
            var appName = "Spezifikations Inspector";
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public void OnNavigatedTo(object parameter)
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            //Adminmode = _CurrentSpeziProperties.Adminmode;
            Adminmode = _settingService.Adminmode;
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
