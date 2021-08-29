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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeziInspector.ViewModels
{
    public class DatenansichtViewModel : ObservableRecipient, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly IParameterDataService _parameterDataService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        public string FullPathXml;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
        public ObservableCollection<Parameter> FilteredParameters { get; set; } = new();
        private ICommand _itemClickCommand;
        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Parameter>(OnItemClick));
        public DatenansichtViewModel(INavigationService navigationService, IParameterDataService parameterDataService)
        {
            _navigationService = navigationService;
            _parameterDataService = parameterDataService;
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
            ShowUnsavedParameters = new RelayCommand(ShowUnsavedParametersView, () => CanShowUnsavedParameters);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
        }

        public IAsyncRelayCommand SaveAllSpeziParameters { get; }
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

        private bool _IsItemSelected;
        public bool IsItemSelected
        {
            get => _IsItemSelected;
            set
            {
                SetProperty(ref _IsItemSelected, value);
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

        private async Task SaveAllParameterAsync()
        {
            await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            CheckUnsavedParametres();
            ShowAllParametersView();
        }

        private void ShowAllParametersView()
        {
            SearchInput = null;
            FilterParameter(SearchInput);
            IsUnsavedParametersSelected = false;
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

        private string _SearchInput;
        public string SearchInput
        {
            get => _SearchInput;

            set
            {
                SetProperty(ref _SearchInput, value);
                if (ParamterDictionary.Values is not null) { FilterParameter(SearchInput); }
                _CurrentSpeziProperties.SearchInput = SearchInput;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }

        private string _SearchInputInfoSidebarPanelText;
        public string SearchInputInfoSidebarPanelText
        {
            get => _SearchInputInfoSidebarPanelText;

            set
            {
                SetProperty(ref _SearchInputInfoSidebarPanelText, value);
                _CurrentSpeziProperties.SearchInputInfoSidebarPanelText = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
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

        public void OnNavigatedTo(object parameter)
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();

            if (_CurrentSpeziProperties.FullPathXml is not null) FullPathXml = _CurrentSpeziProperties.FullPathXml;
            if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            SearchInput = _CurrentSpeziProperties.SearchInput;
            SearchInputInfoSidebarPanelText = _CurrentSpeziProperties.SearchInputInfoSidebarPanelText;
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) CheckUnsavedParametres();
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnItemClick(Parameter clickedItem)
        {
            if (clickedItem != null)
            {
                _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
                _navigationService.NavigateTo(typeof(DatenansichtDetailViewModel).FullName, clickedItem.Name);
            }
        }
    }
}
