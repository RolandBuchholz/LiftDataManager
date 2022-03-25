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
using LiftDataManager.Helpers.Dialogs;
using LiftDataManager.Services;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class HomeViewModel : ObservableRecipient, INavigationAware
    {

        private readonly IParameterDataService _parameterDataService;
        private readonly ISettingService _settingService;
        private readonly IVaultDataService _vaultDataService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();

        public HomeViewModel(IParameterDataService parameterDataService, ISettingService settingsSelectorService, IVaultDataService vaultDataService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, (r, m) =>
            {
                if (m is not null && m.Value.IsDirty)
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                    CheckUnsavedParametres();
                }
            });
            _parameterDataService = parameterDataService;
            _settingService = settingsSelectorService;
            _vaultDataService = vaultDataService;
            ClearSpeziData = new AsyncRelayCommand(ClearData, () => CanClearData);
            LoadSpeziDataAsync = new AsyncRelayCommand(LoadDataAsync, () => CanLoadSpeziData);
            UploadSpeziDataAsync = new AsyncRelayCommand(UploadDataAsync, () => CanUpLoadSpeziData && AuftragsbezogeneXml);
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);

        }

        public IAsyncRelayCommand ClearSpeziData { get; }
        public IAsyncRelayCommand LoadSpeziDataAsync { get; }
        public IAsyncRelayCommand UploadSpeziDataAsync { get; }
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
                UploadSpeziDataAsync.NotifyCanExecuteChanged();
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

      private bool _CheckOut;
        public bool CheckOut
        {
            get => _CheckOut;
            set
            {
                SetProperty(ref _CheckOut, value);
                _CurrentSpeziProperties.CheckOut = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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

        private async Task SetFullPathXmlAsync()
        {
            if (!CanLoadSpeziData)
            {
                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
            }
            else
            {
                CheckOut = false;
                string searchPattern = SpezifikationName + "-AutoDeskTransfer.xml";
                var watch = Stopwatch.StartNew();
                var workspaceSearch = await SearchWorkspaceAsync(searchPattern);
                var stopTimeMs = watch.ElapsedMilliseconds;

                if (workspaceSearch.Length == 0)
                {
                    InfoSidebarPanelText += $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)\n";

                    var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, true);

                    if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                    {
                        FullPathXml = downloadResult.FullFileName;
                        InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                        AuftragsbezogeneXml = true;
                    }
                    else
                    {
                        await StandardDialogs.LiftDataManagerdownloadInfo(downloadResult);
                        InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                        InfoSidebarPanelText += $"Standard Daten geladen\n";
                        FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                        AuftragsbezogeneXml = false;
                    }
                }

                else if (workspaceSearch.Length == 1)
                {
                    InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                    string autoDeskTransferpath = workspaceSearch[0];
                    FileInfo AutoDeskTransferInfo = new(autoDeskTransferpath);
                    if (!AutoDeskTransferInfo.IsReadOnly)
                    {
                        FullPathXml = workspaceSearch[0];
                        InfoSidebarPanelText += $"Die Daten {searchPattern} wurden aus dem Arbeitsberech geladen\n";
                        AuftragsbezogeneXml = true;
                        CheckOut = true;
                    }
                    else
                    {
                        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, true);

                        if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                        {
                            FullPathXml = downloadResult.FullFileName; 
                            InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                            AuftragsbezogeneXml = true;
                        }
                        else
                        {
                            await StandardDialogs.LiftDataManagerdownloadInfo(downloadResult);
                            InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            AuftragsbezogeneXml = false;
                        }
                    }
                }
                else
                {
                    InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                    InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";

                    var confirmed = await App.MainRoot.ConfirmationDialogAsync(
                                            $"Es wurden mehrere {searchPattern} Dateien gefunden?",
                                             "XML aus Vault herunterladen",
                                                "Abbrechen");
                    if (confirmed)
                    {
                        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, true);

                        if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                        {
                            FullPathXml = downloadResult.FullFileName;
                            InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                            AuftragsbezogeneXml = true;
                        }
                        else
                        {
                            await StandardDialogs.LiftDataManagerdownloadInfo(downloadResult);
                            InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                            InfoSidebarPanelText += $"Standard Daten geladen\n";
                            FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                            AuftragsbezogeneXml = false;
                        }
                    }
                    else
                    {
                        InfoSidebarPanelText += $"Standard Daten geladen\n";
                        FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                        AuftragsbezogeneXml = false;
                    }
                }
            }
        }

        private async Task ClearData()
        {
            bool delete = true;
            
            if (CanSaveAllSpeziParameters || CheckOut)
            {
                delete = await App.MainRoot.WarningDialogAsync(
                        $"Warnung es droht Datenverlust",
                        $"Es sind nichtgespeicherte Parameter vorhanden!\n" +
                        $"Die Datei wurde noch nicht ins Vault hochgeladen!\n" +
                        $"Der Befehl >Auschecken Rückgänig< wird ausgeführt!\n" +
                        $"\n" +
                        $"Soll der Forgang fortgesetzt werden?",
                        "Fortsetzen", "Abbrechen") ;
            }

            if (delete)
            {
                InfoSidebarPanelText += $"Daten werden auf die Standardwerte zurückgesetzt\n";
                InfoSidebarPanelText += $"----------\n";
                if (CheckOut)
                {
                    var downloadResult = await _vaultDataService.UndoFileAsync(Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", ""));
                    if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                    {
                        if (File.Exists(FullPathXml))
                        {
                            FileInfo FileInfo = new FileInfo(FullPathXml);
                            if (FileInfo.IsReadOnly)
                            {
                               FileInfo.IsReadOnly = false;
                            }
                            File.Delete(FullPathXml); 
                        }
                        AuftragsbezogeneXml = false;
                        CanLoadSpeziData = false;
                        CanSaveAllSpeziParameters = false;
                        CheckOut = false;
                        await LoadDataAsync();
                    }
                    else
                    {
                        await StandardDialogs.LiftDataManagerdownloadInfo(downloadResult);
                        InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                    }
                }
                else
                {
                    AuftragsbezogeneXml = false;
                    CanLoadSpeziData = false;
                    CanSaveAllSpeziParameters = false;
                    CheckOut = false;
                    await LoadDataAsync();
                }
            }
        }

        private async Task UploadDataAsync()
        {
            InfoSidebarPanelText += $"Spezifikation wird hochgeladen\n";
            InfoSidebarPanelText += $"----------\n";
            await ClearData();
        }

        private async Task LoadDataAsync()
        {
            await SetFullPathXmlAsync();

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

        private async Task<string[]> SearchWorkspaceAsync(string searchPattern)
        {
            InfoSidebarPanelText += $"Suche im Arbeitsbereich gestartet\n";

            string path;
            if (SpezifikationStatusTyp == "Auftrag")
            {
                path = @"C:\Work\AUFTRÄGE NEU\Konstruktion";
            }
            else
            {
                path = @"C:\Work\AUFTRÄGE NEU\Angebote";
            }

            var searchResult = await Task.Run(() => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories));
            return searchResult;
        }

        public void OnNavigatedTo(object parameter)
        {
            if (_CurrentSpeziProperties is null) { SetAdminmode(); }
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = _CurrentSpeziProperties.CheckOut;
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
