using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Models;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SpeziInspector.ViewModels
{
    public class ListenansichtViewModel : ObservableRecipient, INavigationAware
    {
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        public string FullPathXml;
        public ObservableCollection<Parameter> ParamterList { get; set; }
        public ObservableCollection<Parameter> FilteredParameters { get; set; } = new();
        private Parameter _selected;

        public Parameter Selected
        {
            get { return _selected; }
            set 
            {
                if (Selected is not null)
                {
                    Selected.PropertyChanged -= CheckIsDirty;
                }

                SetProperty(ref _selected, value);
                _selected.PropertyChanged += CheckIsDirty;
            }
        }

        public ListenansichtViewModel()
        {
            SaveParameter = new RelayCommand(SaveParameterAsync, () => CanSaveParameter);
        }

        public IRelayCommand SaveParameter { get; }

        private void CheckIsDirty(object sender, PropertyChangedEventArgs e)
        {
            if (Selected.IsDirty)
            {
                CanSaveParameter = true;
            }
        }

        private bool _CanSaveParameter;
        public bool CanSaveParameter
        {
            get => _CanSaveParameter;
            set
            {
                SetProperty(ref _CanSaveParameter, value);
                SaveParameter.NotifyCanExecuteChanged();
            }
        }

        private void SaveParameterAsync()
        {
            Debug.WriteLine(Selected.Name + " in XML gespeichert :)");
            CanSaveParameter = false;
            Selected.IsDirty = false;
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

            if (_CurrentSpeziProperties.FullPathXml is not null) { FullPathXml = _CurrentSpeziProperties.FullPathXml; }
            if (_CurrentSpeziProperties.ParamterList is not null) { ParamterList = _CurrentSpeziProperties.ParamterList; }
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            SearchInput = _CurrentSpeziProperties.SearchInput;
        }

        public void OnNavigatedFrom()
        {
        }

        public void EnsureItemSelected()
        {
            if (Selected == null && FilteredParameters.Count > 0)
            {
                Selected = FilteredParameters.First();
            }
        }
    }
}
