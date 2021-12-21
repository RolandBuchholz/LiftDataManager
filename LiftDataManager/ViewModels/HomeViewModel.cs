using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
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
                if (m is not null && m.Value.IsDirty == true)
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                    CheckUnsavedParametres();
                }
            });
            _parameterDataService = parameterDataService;
            _settingService = settingsSelectorService;
            ClearSpeziData = new AsyncRelayCommand(ClearData, () => CanClearData);
            LoadSpeziDataAsync = new AsyncRelayCommand(LoadDataAsync, () => CanLoadSpeziData);
            UpLoadSpeziDataAsync = new AsyncRelayCommand(LoadUpDataAsync, () => CanUpLoadSpeziData && AuftragsbezogeneXml);
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);

        }

        public IAsyncRelayCommand ClearSpeziData { get; }
        public IAsyncRelayCommand LoadSpeziDataAsync { get; }
        public IAsyncRelayCommand UpLoadSpeziDataAsync { get; }
        public IAsyncRelayCommand SaveAllSpeziParameters { get; }

        private bool _CanCanClearData;
        public bool CanClearData
        {
            get => _CanCanClearData;
            set
            {
                SetProperty(ref _CanCanClearData, value);
                ClearSpeziData.NotifyCanExecuteChanged();
            }
        }

        private bool _CanLoadSpeziData;
        public bool CanLoadSpeziData
        {
            get => _CanLoadSpeziData;
            set
            {
                SetProperty(ref _CanLoadSpeziData, value);
                LoadSpeziDataAsync.NotifyCanExecuteChanged();
            }
        }

        private bool _CanUpLoadSpeziData;
        public bool CanUpLoadSpeziData
        {
            get => _CanUpLoadSpeziData;
            set
            {
                SetProperty(ref _CanUpLoadSpeziData, value);
                UpLoadSpeziDataAsync.NotifyCanExecuteChanged();
            }
        }

        private bool _CanSaveAllSpeziParameters;
        public bool CanSaveAllSpeziParameters
        {
            get => _CanSaveAllSpeziParameters;
            set
            {
                SetProperty(ref _CanSaveAllSpeziParameters, value);
                CanUpLoadSpeziData = !value;
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
                CanClearData = value;
            }
        }

        private string _SpezifikationStatusTyp;
        public string SpezifikationStatusTyp
        {
            get
            {
                if (_SpezifikationStatusTyp is null) { _SpezifikationStatusTyp = "Auftrag"; }
                return _SpezifikationStatusTyp;
            }
            set
            {
                if (_SpezifikationStatusTyp != value) { SpezifikationName = string.Empty; }
                SetProperty(ref _SpezifikationStatusTyp, value);
                _CurrentSpeziProperties.SpezifikationStatusTyp = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private string _SpezifikationName;
        public string SpezifikationName
        {
            get => _SpezifikationName;

            set
            {
                SetProperty(ref _SpezifikationName, value);
                CanLoadSpeziData = ((SpezifikationName.Length >= 6) && (SpezifikationStatusTyp == "Auftrag")) || ((SpezifikationName.Length == 10) && (SpezifikationStatusTyp == "Angebot"));
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

        private string _InfoSidebarPanelText;
        public string InfoSidebarPanelText
        {
            get => _InfoSidebarPanelText;

            set
            {
                SetProperty(ref _InfoSidebarPanelText, value);
                _CurrentSpeziProperties.InfoSidebarPanelText = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private void SetFullPathXml()
        {
            if (!CanLoadSpeziData)
            {
                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
            }
            else
            {
                InfoSidebarPanelText += $"Suche im Arbeitsbereich gestartet\n";
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string path;
                if (SpezifikationStatusTyp == "Auftrag")
                {
                    path = @"C:\Work\AUFTRÄGE NEU\Konstruktion";
                }
                else
                {
                    path = @"C:\Work\AUFTRÄGE NEU\Angebote";
                }
                string searchPattern = SpezifikationName + "-AutoDeskTransfer.xml";
                var searchResult = Directory.GetFiles(path, searchPattern,SearchOption.AllDirectories);
                var stopTimeMs = watch.ElapsedMilliseconds;
                if (searchResult.Length == 0)
                {
                    //ToDo VaultSuche
                    InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                    InfoSidebarPanelText += $"Die Datei {searchPattern} wurde nicht gefunden\n";
                    InfoSidebarPanelText += $"Standard Daten geladen\n";
                    FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                    AuftragsbezogeneXml = false;
                }
                
                else if (searchResult.Length == 1)
                {
                    InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                    string autoDeskTransferpath = searchResult[0];
                    FileInfo AutoDeskTransferInfo = new FileInfo(autoDeskTransferpath);
                    if (!AutoDeskTransferInfo.IsReadOnly)
                    {
                        FullPathXml = searchResult[0];
                        InfoSidebarPanelText += $"Die Daten {searchPattern} wurden aus dem Arbeitsberech geladen\n";
                        AuftragsbezogeneXml = true;
                    }
                    else
                    {
                        //ToDo VaultSuche
                        InfoSidebarPanelText += $"Daten sind schreibgeschützt\n";
                        InfoSidebarPanelText += $"Standard Daten geladen\n";
                        FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                        AuftragsbezogeneXml = false;
                    }
                }
                else
                {
                    //ToDo VaultSuche
                    InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                    InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";
                    InfoSidebarPanelText += $"Standard Daten geladen\n";
                    FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                    AuftragsbezogeneXml = false;
                }
            }
        }

        private async Task ClearData()
        {
            InfoSidebarPanelText += $"Daten wurden auf die Standardwerte zurückgesetzt\n";
            InfoSidebarPanelText += $"----------\n";
            AuftragsbezogeneXml = false;
            CanLoadSpeziData = false;
            CanSaveAllSpeziParameters = false;
            await LoadDataAsync();
        }

        private async Task LoadUpDataAsync()
        {
            InfoSidebarPanelText += $"Spezifikation wird hochgeladen\n";
            InfoSidebarPanelText += $"----------\n";
            await ClearData();
        }

        private async Task LoadDataAsync()
        {
            SetFullPathXml();

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
            InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
            InfoSidebarPanelText += $"----------\n";
            SpezifikationName = string.Empty;
        }

        private async Task SaveAllParameterAsync()
        {
            var infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            InfoSidebarPanelText += infotext;
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
            if (_CurrentSpeziProperties is null) { SetAdminmode(); }
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            SpezifikationStatusTyp = _CurrentSpeziProperties.SpezifikationStatusTyp;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
            if (_CurrentSpeziProperties.FullPathXml is not null) { FullPathXml = _CurrentSpeziProperties.FullPathXml; }
            if (_CurrentSpeziProperties.ParamterDictionary is not null) { ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary; }
            if (ParamterDictionary.Values.Count == 0) { _ = LoadDataAsync(); }
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) { CheckUnsavedParametres(); }
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
