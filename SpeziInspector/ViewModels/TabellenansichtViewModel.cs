using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Models;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SpeziInspector.ViewModels
{
    public class TabellenansichtViewModel : ObservableRecipient, INavigationAware
    {
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        public string FullPathXml;
        public ObservableCollection<Parameter> ParamterList { get; set; }
        public ObservableCollection<Parameter> FilteredParameters { get; set; } = new();
        public ObservableCollection<Parameter> UnsavedParameters { get; set; } = new();

        public TabellenansichtViewModel()
        {
            SaveAllSpeziParameters = new RelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters);
            ShowUnsavedParameters = new RelayCommand(AddUnsavedParameters, () => CanShowUnsavedParameters);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
        }

        public IRelayCommand SaveAllSpeziParameters { get; }
        public IRelayCommand ShowUnsavedParameters { get; }
        public IRelayCommand ShowAllParameters { get; }


        private bool _IsUnsavedParametersSelected;
        public bool IsUnsavedParametersSelected
        {
            get => _IsUnsavedParametersSelected;
            set
            {
                SetProperty(ref _IsUnsavedParametersSelected, value);
            }
        }

        private bool _CanSaveAllSpeziParameters;
        public bool CanSaveAllSpeziParameters
        {
            get => _CanSaveAllSpeziParameters;
            set
            {
                SetProperty(ref _CanSaveAllSpeziParameters, value);
                SaveAllSpeziParameters.NotifyCanExecuteChanged();
            }
        }

        private bool _CanShowUnsavedParameters;
        public bool CanShowUnsavedParameters
        {
            get => _CanShowUnsavedParameters;
            set
            {
                SetProperty(ref _CanShowUnsavedParameters, value);
                ShowUnsavedParameters.NotifyCanExecuteChanged();
            }
        }

        private void SaveAllParameterAsync()
        {
            Debug.WriteLine("Daten werden in XML gespeichert :)");
            UnsavedParameters.Clear();
        }

        private void ShowAllParametersView()
        {
            SearchInput = null;
            FilterParameter(SearchInput);
            IsUnsavedParametersSelected = false;
        }

        private void AddUnsavedParameters()
        {
            FilteredParameters.Clear();
            var unsavedParameter = ParamterList.Where(p => p.IsDirty);

            foreach (var item in unsavedParameter)
            {
                FilteredParameters.Add(item);
            }
            IsUnsavedParametersSelected = true;
        }


        private void CheckUnsavedParametres()
        {
            if (ParamterList.Any(p => p.IsDirty))
            {
                CanShowUnsavedParameters = true;
                CanSaveAllSpeziParameters = true;
            }
            else
            {
                CanShowUnsavedParameters = false;
                CanSaveAllSpeziParameters = false;
            }

        }

        private string _SearchInput;
        public string SearchInput
        {
            get => _SearchInput;

            set
            {
                SetProperty(ref _SearchInput, value);
                if (ParamterList is not null) { FilterParameter(SearchInput); }
                _CurrentSpeziProperties.SearchInput = SearchInput;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private void FilterParameter(string searchInput)
        {

            if (string.IsNullOrWhiteSpace(searchInput))
            {
                FilteredParameters.Clear();
                var allData = ParamterList.Where(p => !string.IsNullOrWhiteSpace(p.Name));

                foreach (var item in allData)
                {
                    FilteredParameters.Add(item);
                }
            }
            else
            {
                FilteredParameters.Clear();
                var filteredData = ParamterList.Where(p => (p.Name != null && p.Name.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Value != null && p.Value.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Comment != null && p.Comment.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase)));

                foreach (var item in filteredData)
                {
                    FilteredParameters.Add(item);
                }

            }
        }

        public void OnNavigatedTo(object parameter)
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();

            if (_CurrentSpeziProperties.FullPathXml is not null)  FullPathXml = _CurrentSpeziProperties.FullPathXml; 
            if (_CurrentSpeziProperties.ParamterList is not null)  ParamterList = _CurrentSpeziProperties.ParamterList; 
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            if (_CurrentSpeziProperties.ParamterList is not null) CheckUnsavedParametres();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
