using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiftDataManager.ViewModels
{
    public class DatenansichtViewModel : DataViewModelBase, INavigationAware
    {
        public ObservableGroupedCollection<string, Parameter> GroupedFilteredParameters { get; private set; } = new();
        private ICommand _itemClickCommand;
        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Parameter>(OnItemClick));

        public DatenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
             base(parameterDataService, dialogService, navigationService)
        {
            ShowUnsavedParameters = new RelayCommand(ShowUnsavedParametersView, () => CanShowUnsavedParameters);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
            SetParameterFilter = new RelayCommand<string>(SetParameterFilterView);
            GroupParameter = new RelayCommand<string>(GroupParameterView);
        }

        public IRelayCommand ShowUnsavedParameters { get; }
        public IRelayCommand ShowAllParameters { get; }
        public IRelayCommand SetParameterFilter { get; }
        public IRelayCommand GroupParameter { get; }

        private bool _IsUnsavedParametersSelected;
        public bool IsUnsavedParametersSelected
        {
            get => _IsUnsavedParametersSelected;
            set => SetProperty(ref _IsUnsavedParametersSelected, value);
        }

        private bool _IsItemSelected;
        public bool IsItemSelected
        {
            get => _IsItemSelected;
            set => SetProperty(ref _IsItemSelected, value);
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

        private int _ParameterFound;
        public int ParameterFound
        {
            get => _ParameterFound;
            set => SetProperty(ref _ParameterFound, value);
        }

        private string _FilterValue = "None";
        public string FilterValue
        {
            get => _FilterValue;
            set => SetProperty(ref _FilterValue, value);
        }

        private string _GroupingValue = "abc";
        public string GroupingValue
        {
            get => _GroupingValue;
            set => SetProperty(ref _GroupingValue, value);
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

        private void ShowAllParametersView()
        {
            SearchInput = null;
            FilterParameter(SearchInput);
            IsUnsavedParametersSelected = false;
        }

        private void ShowUnsavedParametersView()
        {
            GroupedFilteredParameters.Clear();
            var unsavedParameter = ParamterDictionary.Values.Where(p => p.IsDirty).
                                                             GroupBy(GroupView()).
                                                             OrderBy(g => g.Key);

            int parameterFound = 0;
            foreach (var group in unsavedParameter)
            {
                GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
                parameterFound += group.Count();
            }
            ParameterFound = parameterFound;
            IsUnsavedParametersSelected = true;
        }

        private void SetParameterFilterView(string filter)
        {
            FilterValue = filter;
            FilterParameter(SearchInput);
        }

        private void GroupParameterView(string group)
        {
            GroupingValue = group;
            FilterParameter(SearchInput);
        }

        private void FilterParameter(string searchInput)
        {
            int parameterFound = 0;

            GroupedFilteredParameters.Clear();
            var groupedParameters = ParamterDictionary.Values.Where(FilterViewSearchInput(searchInput)).
                                                    GroupBy(GroupView()).
                                                    OrderBy(g => g.Key);
            foreach (var group in groupedParameters)
            {
                GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
                parameterFound += group.Count();
            }

            ParameterFound = parameterFound;
        }

        private Func<Parameter, bool> FilterViewSearchInput(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                bool result;
                switch (FilterValue)
                {
                    case "None":
                        return p => p.Name != null;

                    case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                        result = Enum.TryParse(FilterValue, true, out Parameter.ParameterTypValue filterTypEnum);
                        if (result)
                        {
                            return p => p.Name != null && p.ParameterTyp == filterTypEnum;
                        }
                        return p => p.Name != null;

                    default:
                        result = Enum.TryParse(FilterValue, true, out Parameter.ParameterCategoryValue filterCatEnum);
                        if (result)
                        {
                            return p => p.Name != null && p.ParameterCategory == filterCatEnum;
                        }
                        return p => p.Name != null;
                }
            }
            else
            {
                bool result;
                switch (FilterValue)
                {
                    case "None":
                        return p => (p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase));

                    case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                        result = Enum.TryParse(FilterValue, true, out Parameter.ParameterTypValue filterTypEnum);
                        if (result)
                        {
                            return p => ((p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)))
                                                        && p.ParameterTyp == filterTypEnum;

                        }
                        return p => p.Name != null;

                    default:
                        result = Enum.TryParse(FilterValue, true, out Parameter.ParameterCategoryValue filterCatEnum);
                        if (result)
                        {
                            return p => ((p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                        || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)))
                                                        && p.ParameterCategory == filterCatEnum;
                        }
                        return p => p.Name != null;
                }
            }
        }

        private Func<Parameter, string> GroupView()
        {
            return GroupingValue switch
            {
                "abc" => g => g.Name.Replace("var_", "")[0].ToString().ToUpper(new CultureInfo("de-DE", false)),
                "typ" => g => g.ParameterTyp.ToString(),
                "cat" => g => g.ParameterCategory.ToString(),
                _ => g => g.Name.Replace("var_", "")[0].ToString().ToUpper(new CultureInfo("de-DE", false)),
            };
        }

        public void OnNavigatedTo(object parameter)
        {
            SynchronizeViewModelParameter();
            SearchInput = _CurrentSpeziProperties.SearchInput;
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
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
