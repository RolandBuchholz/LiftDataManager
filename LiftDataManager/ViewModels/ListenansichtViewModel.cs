using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class ListenansichtViewModel : DataViewModelBase, INavigationAware
    {
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

        public ListenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
             base(parameterDataService, dialogService, navigationService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
            {
                if (m is not null && m.Value.IsDirty)
                {
                    SetInfoSidebarPanelText(m);
                    await CheckUnsavedParametresAsync();
                }
            });
            SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
            ShowUnsavedParameters = new RelayCommand(ShowUnsavedParametersView, () => CanShowUnsavedParameters);
        }

        public IAsyncRelayCommand SaveParameter { get; }
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
        }

        override protected async Task CheckUnsavedParametresAsync()
        {
            if (LikeEditParameter && AuftragsbezogeneXml)
            {
                bool dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

                if (CheckOut)
                {
                    CanShowUnsavedParameters = dirty;
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
                    }
                }

            }
        }

        private bool _IsUnsavedParametersSelected;
        public bool IsUnsavedParametersSelected
        {
            get => _IsUnsavedParametersSelected;
            set => SetProperty(ref _IsUnsavedParametersSelected, value);
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

        private bool _IsItemSelected;
        public bool IsItemSelected
        {
            get => _IsItemSelected;
            set => SetProperty(ref _IsItemSelected, value);
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

        private async Task SaveParameterAsync()
        {
            string infotext = await _parameterDataService.SaveParameterAsync(Selected, FullPathXml);
            InfoSidebarPanelText += infotext;
            CanSaveParameter = false;
            Selected.IsDirty = false;
            await CheckUnsavedParametresAsync();
            if (IsUnsavedParametersSelected) ShowUnsavedParametersView();
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
            SearchInput = _CurrentSpeziProperties.SearchInput;
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
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
