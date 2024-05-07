using Cogs.Collections;
using CommunityToolkit.Mvvm.Collections;
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
            DrawupFilterView(SelectedFilter, SearchInput);
        }
    }

    public static readonly DependencyProperty SearchInputProperty =
        DependencyProperty.Register(nameof(SearchInput), typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

    public SelectorBarItem? SelectedFilter
    {
        get { return (SelectorBarItem)GetValue(SelectedFilterProperty); }
        set
        {
            if (value is not null && value.Text.StartsWith("Request"))
            {
                value = SelectorBarFilter.Items.SingleOrDefault(x => x.Text == value.Text.Replace("Request", ""));
            }

            SetValue(SelectedFilterProperty, value);
        }
    }

    public static readonly DependencyProperty SelectedFilterProperty =
        DependencyProperty.Register(nameof(SelectedFilter), typeof(SelectorBarItem), typeof(CommandBar), new PropertyMetadata(default));

    public string GroupingValue
    {
        get => (string)GetValue(GroupingValueProperty);
        set
        {
            SetValue(GroupingValueProperty, value);
            DrawupFilterView(SelectedFilter, SearchInput);
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
            DrawupFilterView(SelectedFilter, SearchInput);
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
            DrawupFilterView(SelectedFilter, SearchInput);
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
            DrawupFilterView(SelectedFilter, SearchInput);
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
            DrawupFilterView(SelectedFilter, SearchInput);
            if (ViewSource is not null && SelectedFilter?.Text == "Highlighted")
                ResetParameterView();
        }
    }

    public static readonly DependencyProperty CanShowHighlightedParametersProperty =
        DependencyProperty.Register(nameof(CanShowHighlightedParameters), typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

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
    private void SelectorBarFilter_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        DrawupFilterView(sender.SelectedItem, SearchInput);
    }
    private void ResetParameterView()
    {
        if (ViewSource.View is not null && ViewSource.View.Count == 0)
        {
            SelectedFilter = SelectorBarFilter.Items.FirstOrDefault();
            SearchInput = string.Empty;
        }
    }
    private void DrawupFilterView(SelectorBarItem? selectedFilterItem, string searchValue)
    {
        if (selectedFilterItem is null)
            return;
        GroupedFilteredParameters.Clear();
        IEnumerable<Parameter> filteredParameters = Enumerable.Empty<Parameter>();
        switch (selectedFilterItem.Text)
        {
            case "All":
                filteredParameters = ItemSource.Values;
                break;
            case "Highlighted":
                filteredParameters = ItemSource.Values.Where(p => p.IsKey);
                break;
            case "Validation Errors":
                filteredParameters = ItemSource.Values.Where(p => p.HasErrors);
                break;
            case "Unsaved":
                filteredParameters = ItemSource.Values.Where(p => p.IsDirty);
                break;
            default:
                return;
        }
        var groupedParameters = filteredParameters.Where(FilterView(FilterValue))
                                                  .Where(FilterViewSearchValue(searchValue))
                                                  .GroupBy(GroupView(GroupingValue))
                                                  .OrderBy(g => g.Key);
        if (groupedParameters is null)
            return;
        foreach (var group in groupedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }
        if (ViewSource is not null)
            ViewSource.Source = GroupedFilteredParameters;
    }

    private static Func<Parameter, bool> FilterView(string filter)
    {
        switch (filter)
        {
            case "None":
                return p => p.Name != null;

            case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                if (Enum.TryParse(filter, true, out ParameterTypValue filterTypEnum))
                {
                    return p => p.Name != null && p.ParameterTyp == filterTypEnum;
                }
                return p => p.Name != null;

            default:
                if (ParameterCategoryValue.TryFromName(filter, true, out ParameterCategoryValue filterCatEnum))
                {
                    return p => p.Name != null && p.ParameterCategory == filterCatEnum;
                }
                return p => p.Name != null;
        }
    }

    private static Func<Parameter, string> GroupView(string group)
    {
        return group switch
        {
            "abc" => g => g.DisplayName[0].ToString().ToUpper(new CultureInfo("de-DE", false)),
            "typ" => g => g.ParameterTyp.ToString(),
            "cat" => g => g.ParameterCategory.ToString(),
            _ => g => g.DisplayName[0].ToString().ToUpper(new CultureInfo("de-DE", false)),
        };
    }
    private static Func<Parameter, bool> FilterViewSearchValue(string searchValue)
    {
        return string.IsNullOrWhiteSpace(searchValue) ?
            p => true :
            p => (p.Name != null && p.Name.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
            || (p.DisplayName != null && p.DisplayName.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
            || (p.Value != null && p.Value.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
            || (p.Comment != null && p.Comment.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase));
    }
}