using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class TabellenansichtViewModel : DataViewModelBase, INavigationAware
    {
        private readonly IParameterDataService _parameterDataService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public bool CheckoutDialogIsOpen { get; private set; }

        public ObservableCollection<Parameter> FilteredParameters { get; set; } = new();

        public TabellenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService)
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
            SaveAllSpeziParameters = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
            ShowUnsavedParameters = new RelayCommand(AddUnsavedParameters, () => CanShowUnsavedParameters);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
        }

        public IAsyncRelayCommand SaveAllSpeziParameters { get; }
        public IRelayCommand ShowUnsavedParameters { get; }
        public IRelayCommand ShowAllParameters { get; }

        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }

        private bool _IsUnsavedParametersSelected;
        public bool IsUnsavedParametersSelected
        {
            get => _IsUnsavedParametersSelected;
            set => SetProperty(ref _IsUnsavedParametersSelected, value);
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

        private async Task SaveAllParameterAsync()
        {
            var infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
            InfoSidebarPanelText += infotext;
            await CheckUnsavedParametresAsync();
            ShowAllParametersView();
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
            var unsavedParameter = ParamterDictionary.Values.Where(p => p.IsDirty);

            foreach (var item in unsavedParameter)
            {
                FilteredParameters.Add(item);
            }
            IsUnsavedParametersSelected = true;
        }

        private async Task CheckUnsavedParametresAsync()
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
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
