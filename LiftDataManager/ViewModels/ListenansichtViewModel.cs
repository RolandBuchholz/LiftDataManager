using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class ListenansichtViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IParameterDataService _parameterDataService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        private bool CheckOut;
        public string FullPathXml;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
        public ObservableCollection<Parameter> FilteredParameters { get; set; } = new();

        private Parameter _selected;

        public Parameter Selected
        {
            get
            {
                CheckIsDirty(_selected);
                if (_selected is not null)
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
                    Selected.PropertyChanged -= CheckIsDirty;

                SetProperty(ref _selected, value);
                if (_selected is not null)
                {
                    _selected.PropertyChanged += CheckIsDirty;
                }
            }
        }

        public ListenansichtViewModel(IParameterDataService parameterDataService)
        {
            _parameterDataService = parameterDataService;
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, (r, m) =>
            {
                if (m is not null && m.Value.IsDirty == true)
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                }
            });
            SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
            ShowUnsavedParameters = new RelayCommand(ShowUnsavedParametersView, () => CanShowUnsavedParameters);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
        }

        public IAsyncRelayCommand SaveParameter { get; }
        public IAsyncRelayCommand SaveAllSpeziParameters { get; }
        public IRelayCommand ShowUnsavedParameters { get; }
        public IRelayCommand ShowAllParameters { get; }

        private void CheckIsDirty(object sender, PropertyChangedEventArgs e)
        {
            CheckIsDirty((Parameter)sender);
        }

        private void CheckIsDirty(Parameter Item)
        {
            if (Item is not null && Item.IsDirty)
            {
                CanSaveParameter = true;
            }
            else
            {
                CanSaveParameter = false;
            }

            SaveParameter.NotifyCanExecuteChanged();
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) CheckUnsavedParametres();

        }

        private void CheckUnsavedParametres()
        {
            if (ParamterDictionary.Values.Any(p => p.IsDirty))
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

        private bool _IsUnsavedParametersSelected;
        public bool IsUnsavedParametersSelected
        {
            get => _IsUnsavedParametersSelected;
            set
            {
                SetProperty(ref _IsUnsavedParametersSelected, value);
            }
        }

        private bool _IsItemSelected;
        public bool IsItemSelected
        {
            get => _IsItemSelected;
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

        private async Task SaveParameterAsync()
        {
            var infotext = await _parameterDataService.SaveParameterAsync(Selected, FullPathXml);
            InfoSidebarPanelText += infotext;
            CanSaveParameter = false;
            Selected.IsDirty = false;
            if (IsUnsavedParametersSelected) ShowUnsavedParametersView();
        }

        private async Task SaveAllParameterAsync()
        {
            var infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            InfoSidebarPanelText += infotext;
            CheckUnsavedParametres();
            ShowAllParametersView();
        }

        private string _SearchInput;
        public string SearchInput
        {
            get => _SearchInput;

            set
            {
                SetProperty(ref _SearchInput, value);
                if (ParamterDictionary is not null) { FilterParameter(SearchInput); }
                _CurrentSpeziProperties.SearchInput = SearchInput;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private void ShowAllParametersView()
        {
            SearchInput = null;
            FilterParameter(SearchInput);
            IsUnsavedParametersSelected = false;
        }

        private void FilterParameter(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                FilteredParameters.Clear();
                var allData = ParamterDictionary.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name));

                foreach (var item in allData)
                {
                    FilteredParameters.Add(item);
                }
            }
            else
            {
                FilteredParameters.Clear();
                var filteredData = ParamterDictionary.Values.Where(p => (p.Name != null && p.Name.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Value != null && p.Value.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Comment != null && p.Comment.Contains(searchInput, System.StringComparison.CurrentCultureIgnoreCase)));
                foreach (var item in filteredData)
                {
                    FilteredParameters.Add(item);
                }
            }
        }

        private void ShowUnsavedParametersView()
        {
            FilteredParameters.Clear();
            var unsavedParameter = ParamterDictionary.Values.Where(p => p.IsDirty);

            foreach (var item in unsavedParameter)
            {
                FilteredParameters.Add(item);
            }
            IsUnsavedParametersSelected = true;

        }

        public void OnNavigatedTo(object parameter)
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            if (_CurrentSpeziProperties.FullPathXml is not null) FullPathXml = _CurrentSpeziProperties.FullPathXml;
            if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = _CurrentSpeziProperties.CheckOut;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) CheckUnsavedParametres();
        }

        public void OnNavigatedFrom()
        {
        }

        public void EnsureItemSelected()
        {
            if (Selected == null && FilteredParameters.Count > 0)
                Selected = FilteredParameters.First();
        }
    }
}
