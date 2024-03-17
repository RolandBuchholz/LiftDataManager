using Cogs.Collections;
using CommunityToolkit.Mvvm.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LiftDataManager.Controls;

public sealed partial class CommandBar : UserControl
{
    public ObservableGroupedCollection<string, Parameter> GroupedFilteredParameters { get; private set; } = new();

    public CommandBar()
    {
        InitializeComponent();
        Loaded += CommandBar_Loaded;
    }

    private void CommandBar_Loaded(object sender, RoutedEventArgs e) 
    { 
        SelectedFilter ??= SelectorBarFilter.Items.FirstOrDefault();
    }

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(nameof(ItemSource), typeof(ObservableDictionary<string, Parameter>), typeof(CommandBar), new PropertyMetadata(null));

    public CollectionViewSource ViewSource
    {
        get => (CollectionViewSource)GetValue(ViewSourceProperty);
        set => SetValue(ViewSourceProperty, value);
    }

    public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register(nameof(ViewSource), typeof(CollectionViewSource), typeof(CommandBar), new PropertyMetadata(null));

    public string SearchInput
    {
        get => (string)GetValue(SearchInputProperty);
        set
        {
            SetValue(SearchInputProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty SearchInputProperty =
        DependencyProperty.Register(nameof(SearchInput), typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

    public string GroupingValue
    {
        get => (string)GetValue(GroupingValueProperty);
        set
        {
            SetValue(GroupingValueProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty GroupingValueProperty =
        DependencyProperty.Register(nameof(GroupingValue), typeof(string), typeof(CommandBar), new PropertyMetadata("abc"));

    public string FilterValue
    {
        get => (string)GetValue(FilterValueProperty);
        set
        {
            SetValue(FilterValueProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty FilterValueProperty =
        DependencyProperty.Register(nameof(FilterValue), typeof(string), typeof(CommandBar), new PropertyMetadata("None"));

    public bool CanShowUnsavedParameters
    {
        get => (bool)GetValue(CanShowUnsavedParametersProperty);
        set
        {
            SetValue(CanShowUnsavedParametersProperty, value);
            RefreshView(SelectedFilter);
            if (ViewSource is not null && SelectedFilter?.Text == "Unsaved")
                ResetParameterView();
        }
    }

    public static readonly DependencyProperty CanShowUnsavedParametersProperty =
        DependencyProperty.Register(nameof(CanShowUnsavedParameters), typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public bool CanShowErrorsParameters
    {
        get => (bool)GetValue(CanShowErrorsParametersProperty);
        set
        {
            SetValue(CanShowErrorsParametersProperty, value);
            RefreshView(SelectedFilter);
            if (ViewSource is not null && SelectedFilter?.Text == "Validation Errors")
                ResetParameterView();
        }
    }

    public static readonly DependencyProperty CanShowErrorsParametersProperty =
        DependencyProperty.Register(nameof(CanShowErrorsParameters), typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public bool CanShowHighlightedParameters
    {
        get => (bool)GetValue(CanShowHighlightedParametersProperty);
        set
        {
            SetValue(CanShowHighlightedParametersProperty, value);
            RefreshView(SelectedFilter);RefreshView(SelectedFilter);
            if (ViewSource is not null && SelectedFilter?.Text == "Highlighted")
                ResetParameterView();
        }
    }

    public static readonly DependencyProperty CanShowHighlightedParametersProperty =
        DependencyProperty.Register(nameof(CanShowHighlightedParameters), typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public SelectorBarItem? SelectedFilter
    {
        get { return (SelectorBarItem)GetValue(SelectedFilterProperty); }
        set
        {
            if(value is not null && value.Text.StartsWith("Request"))
            {
                value = SelectorBarFilter.Items.SingleOrDefault(x => x.Text == value.Text.Replace("Request",""));
            }

            SetValue(SelectedFilterProperty, value); 
        }
    }

    public static readonly DependencyProperty SelectedFilterProperty =
        DependencyProperty.Register(nameof(SelectedFilter), typeof(SelectorBarItem), typeof(CommandBar), new PropertyMetadata(default));

    private void RefreshView(SelectorBarItem? selectedFilterItem, [CallerMemberName] string memberName = "")
    {
        if (selectedFilterItem is null)
            return;
        switch (selectedFilterItem.Text)
        {
            case "All":
                if (string.Equals(memberName, nameof(SelectorBarFilter_SelectionChanged)))
                    SearchInput = string.Empty;
                FilterParameter(SearchInput);
                return;
            case "Highlighted":
                SetHighlightedParameterView();
                return;
            case "Validation Errors":
                SetErrorsParameterView();
                return;
            case "Unsaved":
                SetUnsavedParameterView();
                return;
            default:
                return;
        }
    }

    public ICommand SaveAllCommand
    {
        get => (ICommand)GetValue(SaveAllCommandProperty);
        set => SetValue(SaveAllCommandProperty, value);
    }

    public static readonly DependencyProperty SaveAllCommandProperty =
        DependencyProperty.Register(nameof(SaveAllCommand), typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

    private void SetFilter_Click(object sender, RoutedEventArgs e)
    {
        FilterValue = ((MenuFlyoutItem)sender).Name;
    }

    private void GroupParameter_Click(object sender, RoutedEventArgs e)
    {
        GroupingValue = ((MenuFlyoutItem)sender).Name;
    }

    private void FilterParameter(string searchInput)
    {
        GroupedFilteredParameters.Clear();
        var groupedParameters = ItemSource?.Values.Where(FilterViewSearchInput(searchInput)).
                                                GroupBy(GroupView()).
                                                OrderBy(g => g.Key);
        if (groupedParameters is null)
            return;
        foreach (var group in groupedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }

        if (ViewSource is not null)
            ViewSource.Source = GroupedFilteredParameters;
    }

    private Func<Parameter, bool> FilterViewSearchInput(string searchInput)
    {
        if (string.IsNullOrWhiteSpace(searchInput))
        {
            switch (FilterValue)
            {
                case "None":
                    return p => p.Name != null;

                case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                    if (Enum.TryParse(FilterValue, true, out ParameterTypValue filterTypEnum))
                    {
                        return p => p.Name != null && p.ParameterTyp == filterTypEnum;
                    }
                    return p => p.Name != null;

                default:
                    if (ParameterCategoryValue.TryFromName(FilterValue, true, out ParameterCategoryValue filterCatEnum))
                    {
                        return p => p.Name != null && p.ParameterCategory == filterCatEnum;
                    }
                    return p => p.Name != null;
            }
        }
        else
        {
            switch (FilterValue)
            {
                case "None":
                    return p => (p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase));

                case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                    if (Enum.TryParse(FilterValue, true, out ParameterTypValue filterTypEnum))
                    {
                        return p => ((p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)))
                                                    && p.ParameterTyp == filterTypEnum;

                    }
                    return p => p.Name != null;

                default:
                    if (ParameterCategoryValue.TryFromName(FilterValue, true, out ParameterCategoryValue filterCatEnum))
                    {
                        return p => ((p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
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
            "abc" => g => g.DisplayName![0].ToString().ToUpper(new CultureInfo("de-DE", false)),
            "typ" => g => g.ParameterTyp.ToString(),
            "cat" => g => g.ParameterCategory.ToString(),
            _ => g => g.DisplayName![0].ToString().ToUpper(new CultureInfo("de-DE", false)),
        };
    }

    private void SetUnsavedParameterView()
    {
        GroupedFilteredParameters.Clear();
        var unsavedParameters = ItemSource.Values.Where(p => p.IsDirty)
                                                 .GroupBy(GroupView())
                                                 .OrderBy(g => g.Key);
        foreach (var group in unsavedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }
        if (ViewSource is not null)
            ViewSource.Source = GroupedFilteredParameters;
    }

    private void SetErrorsParameterView()
    {
        GroupedFilteredParameters.Clear();
        var errorParameters = ItemSource.Values.Where(p => p.HasErrors)
                                               .GroupBy(GroupView())
                                               .OrderBy(g => g.Key);
        foreach (var group in errorParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }
        if (ViewSource is not null)
            ViewSource.Source = GroupedFilteredParameters;
    }

    private void SetHighlightedParameterView()
    {
        GroupedFilteredParameters.Clear();
        var highlightedParameters = ItemSource?.Values.Where(p => p.IsKey)
                                               .GroupBy(GroupView())
                                               .OrderBy(g => g.Key);
        if (highlightedParameters is not null)
        {
            foreach (var group in highlightedParameters)
            {
                GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
            }
        }
        if (ViewSource is not null)
            ViewSource.Source = GroupedFilteredParameters;
    }

    private void SelectorBarFilter_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        RefreshView(sender.SelectedItem);
    }

    private void ResetParameterView()
    {
        if (ViewSource.View is not null && ViewSource.View.Count == 0)
        {
            SearchInput = string.Empty;
            FilterParameter(SearchInput);
            SelectedFilter = SelectorBarFilter.Items.FirstOrDefault();
        }
    }
}