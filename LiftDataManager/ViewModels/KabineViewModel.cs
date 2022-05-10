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
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class KabineViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IParameterDataService _parameterDataService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        public bool Adminmode { get; set; }
        public bool AuftragsbezogeneXml { get; set; }
        public bool CheckOut { get; set; }
        public bool LikeEditParameter { get; set; }
        public string FullPathXml { get; set; }
        public bool CheckoutDialogIsOpen { get; private set; }
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }

        public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
            {
                if (m is not null && m.Value.IsDirty)
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                    await CheckUnsavedParametresAsync();
                }
            });
            _parameterDataService = parameterDataService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            SaveAllSpeziParametersAsync = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
        }

        public IAsyncRelayCommand SaveAllSpeziParametersAsync { get; }

        private bool _CanSaveAllSpeziParameters;
        public bool CanSaveAllSpeziParameters
        {
            get => _CanSaveAllSpeziParameters;
            set
            {
                SetProperty(ref _CanSaveAllSpeziParameters, value);
                SaveAllSpeziParametersAsync.NotifyCanExecuteChanged();
            }
        }

        private async Task SaveAllParameterAsync()
        {
            string infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            InfoSidebarPanelText += infotext;
            await CheckUnsavedParametresAsync();
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

        private async Task CheckUnsavedParametresAsync()
        {
            if (LikeEditParameter && AuftragsbezogeneXml)
            {
                bool dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

                if (CheckOut)
                {
                    CanSaveAllSpeziParameters = dirty;
                }
                else if (dirty && !CheckoutDialogIsOpen)
                {
                    CheckoutDialogIsOpen = true;
                    bool dialogResult = await _dialogService.WarningDialogAsync(App.MainRoot,
                                        $"Datei eingechecked (schreibgeschützt)",
                                        $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                        $"Es sind keine Änderungen möglich!\n" +
                                        $"\n" +
                                        $"Soll zur HomeAnsicht gewechselt werden um die Datei aus zu checken?",
                                        "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                    if (dialogResult)
                    {
                        CheckoutDialogIsOpen = false;
                        _navigationService.NavigateTo("LiftDataManager.ViewModels.HomeViewModel"); 
                    }
                    else
                    {
                        CheckoutDialogIsOpen = false;
                        LikeEditParameter = false;
                        _CurrentSpeziProperties.LikeEditParameter = LikeEditParameter;
                        _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
                        
                    }
                }
            }
        }

        //private void CheckDeckenhoehe()
        //{
        //    Debug.WriteLine("Kabinendecke wird überprüft");
        //    decimal bodenHoehe = Convert.ToDecimal(ParamterDictionary["var_KU"].Value);
        //    decimal kabinenHoeheInnen = Convert.ToDecimal(ParamterDictionary["var_KHLicht"].Value);
        //    decimal kabinenHoeheAussen = Convert.ToDecimal(ParamterDictionary["var_KHA"].Value);
        //    decimal deckenhoehe = Convert.ToDecimal(_Deckenhoehe);

        //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && kabinenHoeheAussen > 0 && deckenhoehe == 0)
        //    {
        //        Debug.WriteLine("Kabinendecke wird berechnet");
        //        decimal berechneteDeckenhoehe = kabinenHoeheAussen - kabinenHoeheInnen - bodenHoehe;
        //        _Deckenhoehe = Convert.ToString(berechneteDeckenhoehe);
        //    }

        //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && deckenhoehe > 0 && kabinenHoeheAussen ==0)
        //    {
        //        Debug.WriteLine("Kabinenaussen wird berechnet");
        //        decimal berechneteHoeheAussen = bodenHoehe + kabinenHoeheInnen + deckenhoehe;
        //        ParamterDictionary["var_KHA"].Value = Convert.ToString(berechneteHoeheAussen);
        //    }

        //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && deckenhoehe > 0 && kabinenHoeheAussen > 0)
        //    {
        //        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe != kabinenHoeheAussen)
        //        {
        //            Debug.WriteLine("Kabinenaussen wird angepasst");
        //            decimal berechneteHoeheAussen = bodenHoehe + kabinenHoeheInnen + deckenhoehe;
        //            ParamterDictionary["var_KHA"].Value = Convert.ToString(berechneteHoeheAussen);
        //        }
        //    }
        //}

        public void OnNavigatedTo(object parameter)
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            if (_CurrentSpeziProperties.FullPathXml is not null) FullPathXml = _CurrentSpeziProperties.FullPathXml;
            if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = _CurrentSpeziProperties.CheckOut;
            LikeEditParameter = _CurrentSpeziProperties.LikeEditParameter;
            InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
