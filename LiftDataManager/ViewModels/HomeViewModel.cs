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
using System;
using System.Diagnostics;
using System.Globalization;
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
        private readonly IDialogService _dialogService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode { get; set; }
        private bool OpenReadOnly { get; set; } = true;
        public bool CheckoutDialogIsOpen { get; private set; }
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();

        public HomeViewModel(IParameterDataService parameterDataService, ISettingService settingsSelectorService, IVaultDataService vaultDataService, IDialogService dialogService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
            {
                if (m is not null && m.Value.IsDirty)
                {
                    SetInfoSidebarPanelText(m);
                    await CheckUnsavedParametresAsync();
                }
            });
            _parameterDataService = parameterDataService;
            _settingService = settingsSelectorService;
            _vaultDataService = vaultDataService;
            _dialogService = dialogService;
            CheckOutSpeziDataAsync = new AsyncRelayCommand(CheckOutAsync, () => CanCheckOut);
            ClearSpeziDataAsync = new AsyncRelayCommand(ClearDataAsync, () => CanClearData);
            LoadSpeziDataAsync = new AsyncRelayCommand(LoadDataAsync, () => CanLoadSpeziData);
            UploadSpeziDataAsync = new AsyncRelayCommand(UploadDataAsync, () => CanUpLoadSpeziData && AuftragsbezogeneXml);
            SaveAllSpeziParametersAsync = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
        }

        public IAsyncRelayCommand CheckOutSpeziDataAsync { get; }
        public IAsyncRelayCommand ClearSpeziDataAsync { get; }
        public IAsyncRelayCommand LoadSpeziDataAsync { get; }
        public IAsyncRelayCommand UploadSpeziDataAsync { get; }
        public IAsyncRelayCommand SaveAllSpeziParametersAsync { get; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }

        private bool _CanCheckOut;
        public bool CanCheckOut
        {
            get => _CanCheckOut;
            set
            {
                SetProperty(ref _CanCheckOut, value);
                CheckOutSpeziDataAsync.NotifyCanExecuteChanged();
            }
        }

        private bool _CanClearData;
        public bool CanClearData
        {
            get => _CanClearData;
            set
            {
                SetProperty(ref _CanClearData, value);
                ClearSpeziDataAsync.NotifyCanExecuteChanged();
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
                SaveAllSpeziParametersAsync.NotifyCanExecuteChanged();
            }
        }

        private async Task CheckUnsavedParametresAsync()
        {
            if (LikeEditParameter && AuftragsbezogeneXml)
            {
                bool dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

                if (CheckOut)
                {
                    CanSaveAllSpeziParameters = dirty;
                }
                else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
                {
                    CheckoutDialogIsOpen = true;
                    bool dialogResult = await _dialogService.WarningDialogAsync(App.MainRoot,
                                        $"Datei eingechecked (schreibgeschützt)",
                                        $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                        $"Es sind keine Änderungen möglich!\n" +
                                        $"\n" +
                                        $"Soll die AutodeskTransferXml ausgechecked werden?",
                                        "Auschecken", "Schreibgeschützt bearbeiten");
                    if (dialogResult)
                    {
                        IsBusy = true;
                        OpenReadOnly = false;
                        Parameter storedParmeter = null;
                        if (ParamterDictionary.Values.Where(x => x.IsDirty).Count() == 1)
                        {
                            storedParmeter = ParamterDictionary.Values.First(x => x.IsDirty);
                        }
                        await LoadDataAsync();
                        if (storedParmeter != null)
                        {
                            ParamterDictionary[storedParmeter.Name] = storedParmeter;
                            CanSaveAllSpeziParameters = dirty;
                        };
                        CheckoutDialogIsOpen = false;
                        IsBusy = false;
                    }
                    else
                    {
                        CheckoutDialogIsOpen = false;
                        LikeEditParameter = false;
                    }
                }

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
                CanClearData = value;
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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
                CanUpLoadSpeziData = value;
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private bool _LikeEditParameter;
        public bool LikeEditParameter
        {
            get => _LikeEditParameter;
            set
            {
                SetProperty(ref _LikeEditParameter, value);
                _CurrentSpeziProperties.LikeEditParameter = value;
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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
                _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private async Task SetFullPathXmlAsync(bool ReadOnly = true)
        {
            if (!AuftragsbezogeneXml && string.IsNullOrEmpty(SpezifikationName))
            {
                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                SpezifikationName = string.Empty;
            }
            else
            {
                CheckOut = false;
                string searchPattern = SpezifikationName + "-AutoDeskTransfer.xml";
                Stopwatch watch = Stopwatch.StartNew();
                var workspaceSearch = await SearchWorkspaceAsync(searchPattern);
                var stopTimeMs = watch.ElapsedMilliseconds;

                switch (workspaceSearch.Length)
                {
                    case 0:
                        {
                            InfoSidebarPanelText += $"{searchPattern} nicht im Arbeitsbereich vorhanden. (searchtime: {stopTimeMs} ms)\n";

                            DownloadInfo downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                            if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                            {
                                FullPathXml = downloadResult.FullFileName;
                                InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                AuftragsbezogeneXml = true;
                            }
                            else
                            {
                                await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot, downloadResult);
                                InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                InfoSidebarPanelText += $"Standard Daten geladen\n";
                                FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                AuftragsbezogeneXml = false;
                            }

                            break;
                        }

                    case 1:
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
                                DownloadInfo downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                                if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                                {
                                    FullPathXml = downloadResult.FullFileName;
                                    InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                    AuftragsbezogeneXml = true;
                                    CheckOut = downloadResult.IsCheckOut;
                                }
                                else
                                {
                                    await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot, downloadResult);
                                    InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                                    InfoSidebarPanelText += $"Standard Daten geladen\n";
                                    FullPathXml = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
                                    AuftragsbezogeneXml = false;
                                }
                            }

                            break;
                        }

                    default:
                        {
                            InfoSidebarPanelText += $"Suche im Arbeitsbereich beendet {stopTimeMs} ms\n";
                            InfoSidebarPanelText += $"Mehrere Dateien mit dem Namen {searchPattern} wurden gefunden\n";

                            bool confirmed = await _dialogService.ConfirmationDialogAsync(App.MainRoot,
                                                    $"Es wurden mehrere {searchPattern} Dateien gefunden?",
                                                     "XML aus Vault herunterladen",
                                                        "Abbrechen");
                            if (confirmed)
                            {
                                DownloadInfo downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName, ReadOnly);

                                if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                                {
                                    FullPathXml = downloadResult.FullFileName;
                                    InfoSidebarPanelText += $"{FullPathXml.Replace(@"C:\Work\AUFTRÄGE NEU\", "")} geladen\n";
                                    AuftragsbezogeneXml = true;
                                }
                                else
                                {
                                    await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot, downloadResult);
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
                                SpezifikationName = string.Empty;
                                AuftragsbezogeneXml = false;
                            }

                            break;
                        }
                }
            }
        }

        private async Task CheckOutAsync()
        {
            OpenReadOnly = false;
            await LoadDataAsync();
        }

        private async Task ClearDataAsync()
        {
            bool delete = true;

            if (CanSaveAllSpeziParameters || CheckOut)
            {
                delete = await _dialogService.WarningDialogAsync(App.MainRoot,
                        $"Warnung es droht Datenverlust",
                        $"Es sind nichtgespeicherte Parameter vorhanden!\n" +
                        $"Die Datei wurde noch nicht ins Vault hochgeladen!\n" +
                        $"Der Befehl >Auschecken Rückgänig< wird ausgeführt!\n" +
                        $"\n" +
                        $"Soll der Vorgang fortgesetzt werden?",
                        "Fortsetzen", "Abbrechen");
            }

            if (delete)
            {
                InfoSidebarPanelText += $"Daten werden auf die Standardwerte zurückgesetzt\n";
                InfoSidebarPanelText += $"----------\n";
                if (CheckOut)
                {
                    DownloadInfo downloadResult = await _vaultDataService.UndoFileAsync(Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", ""));
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
                        SpezifikationName = string.Empty;
                        CanLoadSpeziData = false;
                        CanSaveAllSpeziParameters = false;
                        CheckOut = false;
                        LikeEditParameter = true;
                        await LoadDataAsync();

                    }
                    else
                    {
                        await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot, downloadResult);
                        InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                    }
                }
                else
                {
                    AuftragsbezogeneXml = false;
                    CanLoadSpeziData = false;
                    CanSaveAllSpeziParameters = false;
                    CheckOut = false;
                    CanCheckOut = false;
                    LikeEditParameter = true;
                    SpezifikationName = string.Empty;
                    await LoadDataAsync();
                }
            }
        }

        private async Task UploadDataAsync()
        {
            await Task.Delay(30);
            if (CheckOut)
            {
                Stopwatch watch = Stopwatch.StartNew();
                DownloadInfo downloadResult = await _vaultDataService.SetFileAsync(SpezifikationName);
                long stopTimeMs = watch.ElapsedMilliseconds;

                if (downloadResult.ExitState == DownloadInfo.ExitCodeEnum.NoError)
                {
                    InfoSidebarPanelText += $"Spezifikation wurde hochgeladen ({stopTimeMs} ms)\n";
                    InfoSidebarPanelText += $"----------\n";
                    InfoSidebarPanelText += $"Standard Daten geladen\n";
                    AuftragsbezogeneXml = false;
                    CheckOut = false;
                    LikeEditParameter = true;
                    SpezifikationName = string.Empty;
                    await LoadDataAsync();
                }
                else
                {
                    await _dialogService.LiftDataManagerdownloadInfoAsync(App.MainRoot, downloadResult);
                    InfoSidebarPanelText += $"Fehler: {downloadResult.ExitState}\n";
                    InfoSidebarPanelText += $"----------\n";
                }
            }
        }

        private async Task LoadDataAsync()
        {
            await SetFullPathXmlAsync(OpenReadOnly);

            System.Collections.Generic.IEnumerable<Parameter> data = await _parameterDataService.LoadParameterAsync(FullPathXml);

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
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            InfoSidebarPanelText += $"Daten aus {FullPathXml} geladen \n";
            InfoSidebarPanelText += $"----------\n";
            LikeEditParameter = true;
            OpenReadOnly = true;
            CanCheckOut = !CheckOut && AuftragsbezogeneXml;
        }

        private void SetInfoSidebarPanelText(ParameterDirtyMessage m)
        {
            if (m.Value.ParameterTyp == Core.Models.Parameter.ParameterTypValue.Date)
            {
                string datetimeOld;
                string datetimeNew;
                try
                {
                    if (m.Value.OldValue is not null)
                    {
                        double excelDateOld = Convert.ToDouble(m.Value.OldValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                        datetimeOld = DateTime.FromOADate(excelDateOld).ToShortDateString();
                    }
                    else
                    {
                        datetimeOld = string.Empty;
                    }

                    if (m.Value.NewValue is not null)
                    {
                        double excelDateNew = Convert.ToDouble(m.Value.NewValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                        datetimeNew = DateTime.FromOADate(excelDateNew).ToShortDateString();
                    }
                    else
                    {
                        datetimeNew = string.Empty;
                    }
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {datetimeOld} => {datetimeNew} geändert \n";
                }
                catch
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                }
            }
            else
            {
                InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
            }
        }


        private async Task SaveAllParameterAsync()
        {
            string infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            InfoSidebarPanelText += infotext;
            await CheckUnsavedParametresAsync();
        }

        private void SetAdminmode()
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _settingService.Adminmode;
            _CurrentSpeziProperties.Adminmode = Adminmode;
            _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
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

            string[] searchResult = await Task.Run(() => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories));
            return searchResult;
        }

        public void OnNavigatedTo(object parameter)
        {
            if (_CurrentSpeziProperties is null) { SetAdminmode(); }
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = _CurrentSpeziProperties.CheckOut;
            LikeEditParameter = _CurrentSpeziProperties.LikeEditParameter;
            SpezifikationStatusTyp = _CurrentSpeziProperties.SpezifikationStatusTyp;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
            if (_CurrentSpeziProperties.FullPathXml is not null)
            {
                FullPathXml = _CurrentSpeziProperties.FullPathXml;
                _SpezifikationName = Path.GetFileNameWithoutExtension(FullPathXml).Replace("-AutoDeskTransfer", "");
            }
            if (_CurrentSpeziProperties.ParamterDictionary is not null) { ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary; }
            if (ParamterDictionary.Values.Count == 0) { _ = LoadDataAsync(); }
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) { _ = CheckUnsavedParametresAsync(); }
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
