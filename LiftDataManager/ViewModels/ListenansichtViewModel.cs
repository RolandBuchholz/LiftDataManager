using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class ListenansichtViewModel : DataViewModelBase, INavigationAware
    {
        public ObservableGroupedCollection<string, Parameter> GroupedFilteredParameters { get; private set; } = new();
        public CollectionViewSource GroupedItems { get; private set; }

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

            GroupedItems = new();
            GroupedItems.IsSourceGrouped = true;

            SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
            ShowAllParameters = new RelayCommand(ShowAllParametersView);
            ShowUnsavedParameters = new RelayCommand(ShowUnsavedParametersView, () => CanShowUnsavedParameters);
            SetParameterFilter = new RelayCommand<string>(SetParameterFilterView);
            GroupParameter = new RelayCommand<string>(GroupParameterView);
        }

        public IAsyncRelayCommand SaveParameter { get; }
        public IRelayCommand ShowUnsavedParameters { get; }
        public IRelayCommand ShowAllParameters { get; }
        public IRelayCommand SetParameterFilter { get; }
        public IRelayCommand GroupParameter { get; }

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

        private async Task SaveParameterAsync()
        {
            string infotext = await _parameterDataService.SaveParameterAsync(Selected, FullPathXml);
            InfoSidebarPanelText += infotext;
            CanSaveParameter = false;
            Selected.IsDirty = false;
            await CheckUnsavedParametresAsync();
            if (IsUnsavedParametersSelected) ShowUnsavedParametersView();
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
                                                             GroupBy(g => g.Name.Replace("var_", "")[0].ToString().ToUpper()).
                                                             OrderBy(g => g.Key);
            foreach (var group in unsavedParameter)
            {
                GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
            }
            IsUnsavedParametersSelected = true;
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
            GroupedItems.Source = GroupedFilteredParameters;
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

        public void EnsureItemSelected()
        {
            if (Selected == null && GroupedFilteredParameters.Count > 0)
                Selected = GroupedFilteredParameters.FirstOrDefault().ElementAt(0);
        }
    }
}
