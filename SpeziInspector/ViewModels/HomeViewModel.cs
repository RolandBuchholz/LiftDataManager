using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.Services;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Models;
using SpeziInspector.Helpers;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SpeziInspector.ViewModels
{
    public class HomeViewModel : ObservableRecipient, INavigationAware
    {

        private readonly IParameterDataService _parameterDataService;
        private readonly ISettingService _settingService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        public ObservableCollection<Parameter> ParamterList { get; set; } = new();
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();

        public HomeViewModel(IParameterDataService parameterDataService, ISettingService settingsSelectorService)
        {
            _parameterDataService = parameterDataService;
            _settingService = settingsSelectorService;
            LoadDataFromVault = new RelayCommand(LoadVaultData, () => CanLoadDataFromVault);
            LoadSpeziDataAsync = new RelayCommand(LoadDataAsync, () => CanLoadSpeziData);
            SaveAllSpeziParameters = new RelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);

            MyDict = new Dictionary<string, string>
                {
                    { "First","Test1"},
                    { "Second","Test2" }

                };
        }

        public IRelayCommand LoadDataFromVault { get; }
        public IRelayCommand LoadSpeziDataAsync { get; }
        public IRelayCommand SaveAllSpeziParameters { get; }

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
            if (ParamterList.Any(p => p.IsDirty))
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
                //CanLoadDataFromVault = value;
                //CanLoadSpeziData = !value; 
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
                // FilterParameter(_SearchInput);
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
                FullPathXml = @"C:\Work\Administration\Spezifikation\8951268-AutoDeskTransfer.xml";
            }
        }

        private void LoadVaultData()
        {
            Debug.WriteLine("Daten aus Vault geladen :)");
        }

        private async void LoadDataAsync()
        {
            ParamterList.Clear();
            if (FullPathXml is null) { SetFullPathXml(); };
            var data = await _parameterDataService.LoadParameterAsync(FullPathXml);

            foreach (var item in data)
            {
                ParamterList.Add(item);
                try
                {
                    ParamterDictionary.Add(item.Name, item);
                }
                catch
                { 
                    Debug.WriteLine(item.Name+ " sind bereits im Dictionary"); 
                }

                //MyDict = new Dictionary<string, string>
                //{
                //    { "First","Test1"},
                //    { "Second","Test2" }

                //};
            }

            //string key = "var_Index";
            //var test = ParamterDictionary[key];
            SetHomeParameter();

            _CurrentSpeziProperties.ParamterList = ParamterList;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));

            Debug.WriteLine($"Daten aus {FullPathXml} geladen :)");
        }

        private void SaveAllParameterAsync()
        {
            _parameterDataService.SaveAllParameterAsync(ParamterList, FullPathXml);
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
            if (_CurrentSpeziProperties.ParamterList is not null) { ParamterList = _CurrentSpeziProperties.ParamterList; }
            SetHomeParameter();
            if (_CurrentSpeziProperties.ParamterList is not null) CheckUnsavedParametres();
        }

        public void OnNavigatedFrom()
        {
        }

        private void SetHomeParameter()
        {

            if (ParamterList.Count > 0) Kennwort = (ParamterList.FirstOrDefault(p => p.Name == "var_Kennwort")).Value;
            Projekt = ParamterList.FirstOrDefault(p => p.Name == "var_Projekt");
            Betreiber = ParamterList.FirstOrDefault(p => p.Name == "var_Betreiber");
            Nutzlast = ParamterList.FirstOrDefault(p => p.Name == "var_Q");
            Fahrkorbgewicht = ParamterList.FirstOrDefault(p => p.Name == "var_F");
            Nenngeschwindigkeit = ParamterList.FirstOrDefault(p => p.Name == "var_v");
            AnzahlPersonen = ParamterList.FirstOrDefault(p => p.Name == "var_Personen");
            Schachtbreite = ParamterList.FirstOrDefault(p => p.Name == "var_SB");
            Schachtiefe = ParamterList.FirstOrDefault(p => p.Name == "var_ST");
            Schachtgrube = ParamterList.FirstOrDefault(p => p.Name == "var_SG");
            Schachtkopf = ParamterList.FirstOrDefault(p => p.Name == "var_SK");
            Foederhoehe = ParamterList.FirstOrDefault(p => p.Name == "var_FH");
            Kabinenbreite = ParamterList.FirstOrDefault(p => p.Name == "var_KBI");
            Kabinentiefe = ParamterList.FirstOrDefault(p => p.Name == "var_KTI");
            Kabinenhoehe = ParamterList.FirstOrDefault(p => p.Name == "var_KHLicht");
            Kabinenflaeche = ParamterList.FirstOrDefault(p => p.Name == "var_A_Kabine");
            Kommentare = ParamterList.FirstOrDefault(p => p.Name == "var_Kommentare");
        }


        private Dictionary<string, string> _MyDict;
        public Dictionary<string,string> MyDict
        {
            get { return _MyDict; }
            set { _MyDict = value; } 
        }



        private string _var_Kennwort;
        public string Kennwort
        {
            get => _var_Kennwort;
            set
            {
                SetProperty(ref _var_Kennwort, value);
                ParamterList.FirstOrDefault(p => p.Name == "var_Kennwort").Value = value;
                CheckUnsavedParametres();
            }
        }

        private Parameter _var_Projekt;
        public Parameter Projekt
        {
            get => _var_Projekt;
            set
            {
                SetProperty(ref _var_Projekt, value);
                CheckUnsavedParametres();
            }
        }

        private Parameter _var_Betreiber;
        public Parameter Betreiber
        {
            get => _var_Betreiber;
            set => SetProperty(ref _var_Betreiber, value);
        }

        private Parameter _var_Q;
        public Parameter Nutzlast
        {
            get => _var_Q;
            set => SetProperty(ref _var_Q, value);
        }

        private Parameter _var_F;
        public Parameter Fahrkorbgewicht
        {
            get => _var_F;
            set => SetProperty(ref _var_F, value);
        }

        private Parameter _var_v;
        public Parameter Nenngeschwindigkeit
        {
            get => _var_v;
            set => SetProperty(ref _var_v, value);
        }

        private Parameter _var_Personen;
        public Parameter AnzahlPersonen
        {
            get => _var_Personen;
            set => SetProperty(ref _var_Personen, value);
        }

        private Parameter _var_SB;
        public Parameter Schachtbreite
        {
            get => _var_SB;
            set => SetProperty(ref _var_SB, value);
        }

        private Parameter _var_ST;
        public Parameter Schachtiefe
        {
            get => _var_ST;
            set => SetProperty(ref _var_ST, value);
        }

        private Parameter _var_SG;
        public Parameter Schachtgrube
        {
            get => _var_SG;
            set => SetProperty(ref _var_SG, value);
        }

        private Parameter _var_SK;
        public Parameter Schachtkopf
        {
            get => _var_SK;
            set => SetProperty(ref _var_SK, value);
        }

        private Parameter _var_FH;
        public Parameter Foederhoehe
        {
            get => _var_FH;
            set => SetProperty(ref _var_FH, value);
        }

        private Parameter _var_KBI;
        public Parameter Kabinenbreite
        {
            get => _var_KBI;
            set => SetProperty(ref _var_KBI, value);
        }

        private Parameter _var_KTI;
        public Parameter Kabinentiefe
        {
            get => _var_KTI;
            set => SetProperty(ref _var_KTI, value);
        }

        private Parameter _var_KHLicht;
        public Parameter Kabinenhoehe
        {
            get => _var_KHLicht;
            set => SetProperty(ref _var_KHLicht, value);
        }

        private Parameter _var_A_Kabine;
        public Parameter Kabinenflaeche
        {
            get => _var_A_Kabine;
            set => SetProperty(ref _var_A_Kabine, value);
        }

        private Parameter _var_Kommentare;
        public Parameter Kommentare
        {
            get => _var_Kommentare;
            set => SetProperty(ref _var_Kommentare, value);
        }
    }
}
