using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.Services;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Messenger;
using SpeziInspector.Core.Messenger.Messages;
using SpeziInspector.Core.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpeziInspector.ViewModels
{
    public class HomeViewModel : ObservableRecipient, INavigationAware
    {

        private readonly IParameterDataService _parameterDataService;
        private readonly ISettingService _settingService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();

        public HomeViewModel(IParameterDataService parameterDataService, ISettingService settingsSelectorService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, (r, m) =>
            {
                if (m is not null && m.Value == true) CheckUnsavedParametres();
            });
            _parameterDataService = parameterDataService;
            _settingService = settingsSelectorService;
            LoadDataFromVault = new AsyncRelayCommand(LoadVaultData, () => CanLoadDataFromVault);
            LoadSpeziDataAsync = new AsyncRelayCommand(LoadDataAsync, () => CanLoadSpeziData);
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
        }

        public IAsyncRelayCommand LoadDataFromVault { get; }
        public IAsyncRelayCommand LoadSpeziDataAsync { get; }
        public IAsyncRelayCommand SaveAllSpeziParameters { get; }

        private bool _CanLoadDataFromVault;
        public bool CanLoadDataFromVault
        {
            get => _CanLoadDataFromVault;
            set
            {
                SetProperty(ref _CanLoadDataFromVault, value);
                LoadDataFromVault.NotifyCanExecuteChanged();
            }
        }

        private bool _CanLoadSpeziData = true;
        public bool CanLoadSpeziData
        {
            get => _CanLoadSpeziData;
            set
            {
                SetProperty(ref _CanLoadSpeziData, value);
                LoadSpeziDataAsync.NotifyCanExecuteChanged();
            }
        }

        private bool _CanSaveAllSpeziParameters = false;
        public bool CanSaveAllSpeziParameters
        {
            get => _CanSaveAllSpeziParameters;
            set
            {
                SetProperty(ref _CanSaveAllSpeziParameters, value);
                SaveAllSpeziParameters.NotifyCanExecuteChanged();
            }
        }

        private void CheckUnsavedParametres()
        {
            if (ParamterDictionary.Values.Any(p => p.IsDirty))
            {
                CanSaveAllSpeziParameters = true;
            }
            else
            {
                CanSaveAllSpeziParameters = false;
            }
        }

        private bool _AuftragsbezogeneXml;
        public bool AuftragsbezogeneXml
        {
            get => _AuftragsbezogeneXml;
            set
            {
                SetProperty(ref _AuftragsbezogeneXml, value);
                _CurrentSpeziProperties.AuftragsbezogeneXml = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
                SetFullPathXml();
            }
        }

        private string _FullPathXml;
        public string FullPathXml
        {
            get => _FullPathXml;
            set
            {
                SetProperty(ref _FullPathXml, value);
                _CurrentSpeziProperties.FullPathXml = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private string _SearchInput;
        public string SearchInput
        {
            get => _SearchInput;

            set
            {
                SetProperty(ref _SearchInput, value);
                _CurrentSpeziProperties.SearchInput = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private void SetFullPathXml()
        {
            if (!AuftragsbezogeneXml)
            {
                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
            }
            else
            {
                FullPathXml = @"C:\Work\AUFTRÄGE NEU\Konstruktion\895\8951214\8951214-AutoDeskTransfer.xml";
            }
        }

        private async Task LoadVaultData()
        {
            await Task.CompletedTask;
            Debug.WriteLine("Daten aus Vault geladen :)");
        }

        private async Task LoadDataAsync()
        {
            
            if (FullPathXml is null) { SetFullPathXml(); };
            var data = await _parameterDataService.LoadParameterAsync(FullPathXml);

            foreach (var item in data)
            {
                if (ParamterDictionary.ContainsKey(item.Name))
                {
                    ParamterDictionary[item.Name] = item;
                }
                else
                {
                    ParamterDictionary.Add(item.Name, item);
                }
            }

            _CurrentSpeziProperties.ParamterDictionary = ParamterDictionary;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            //await Task.Delay(5000);
            Debug.WriteLine($"Daten aus {FullPathXml} geladen :)");
        }

        private async Task SaveAllParameterAsync()
        {
            await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            CheckUnsavedParametres();
        }

        private void SetAdminmode()
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _settingService.Adminmode;
            _CurrentSpeziProperties.Adminmode = Adminmode;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }

        public void OnNavigatedTo(object parameter)
        {
            if (_CurrentSpeziProperties is null) SetAdminmode();
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            if (_CurrentSpeziProperties.FullPathXml is not null) { FullPathXml = _CurrentSpeziProperties.FullPathXml; }
            if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
            if (ParamterDictionary.Values.Count == 0) { _ = LoadDataAsync(); }
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) CheckUnsavedParametres();
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
