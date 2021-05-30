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
        public ObservableCollection<Parameter> UnsavedParameters { get; set; } = new();

        private Parameter _selected;

        public Parameter Selected
        {
            get 
            {
                CheckIsDirty(_selected);
                if(_selected is not null)
                {
                    IsItemSelected = true;
                }
                else
                {
                    IsItemSelected = false;
                }
                return _selected; 
            }
            set 
            {
                if (Selected is not null)
                {
                    Selected.PropertyChanged -= CheckIsDirty;
                }
                SetProperty(ref _selected, value);
                if (_selected is not null)
                {
                    _selected.PropertyChanged += CheckIsDirty;
                }
            }
        }

        public ListenansichtViewModel()
        {
            SaveParameter = new RelayCommand(SaveParameterAsync, () => CanSaveParameter);
            SaveAllSpeziParameters = new RelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters);
            ShowUnsavedParameters= new RelayCommand(AddUnsavedParameters, () => CanShowUnsavedParameters);
        }

        public IRelayCommand SaveParameter { get; }
        public IRelayCommand SaveAllSpeziParameters { get; }
        public IRelayCommand ShowUnsavedParameters { get; }

        private void CheckIsDirty(object sender, PropertyChangedEventArgs e)
        {
            CheckIsDirty((Parameter)sender);
        }

        private void CheckIsDirty(Parameter Item)
        {
            if (Item is not null && Item.IsDirty)
            {
                CanSaveParameter = true;
                if (!UnsavedParameters.Contains(Item))
                {
                    UnsavedParameters.Add(Item);
                }
                
            }
            else
            {
                CanSaveParameter = false;
            }
            SaveParameter.NotifyCanExecuteChanged();
        }

        private bool _IsItemSelected;
        public bool IsItemSelected 
        { get => _IsItemSelected;
            set
            {
                SetProperty(ref _IsItemSelected, value);
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

        private bool _CanShowUnsavedParameters =true;
        public bool CanShowUnsavedParameters
        {
            get => _CanShowUnsavedParameters;
            set
            {
                SetProperty(ref _CanShowUnsavedParameters, value);
                ShowUnsavedParameters.NotifyCanExecuteChanged();
            }
        }

        private void SaveParameterAsync()
        {
            Debug.WriteLine(Selected.Name + " in XML gespeichert :)");
            CanSaveParameter = false;
            Selected.IsDirty = false;
            UnsavedParameters.Remove(Selected);
            AddUnsavedParameters();
        }

        private void SaveAllParameterAsync()
        {
            Debug.WriteLine("Daten werden in XML gespeichert :)");
            UnsavedParameters.Clear();
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



        private void AddUnsavedParameters() 
        {
            FilteredParameters.Clear();
            var unsavedParameter = UnsavedParameters.Where(p => !string.IsNullOrWhiteSpace(p.Name));

            foreach (var item in unsavedParameter)
            {
                FilteredParameters.Add(item);
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
