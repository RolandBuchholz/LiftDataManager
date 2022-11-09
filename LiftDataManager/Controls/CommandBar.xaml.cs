using Cogs.Collections;
using CommunityToolkit.Common.Collections;
using System.Windows.Input;

namespace LiftDataManager.Controls;

public sealed partial class CommandBar : UserControl
{
    public ObservableGroupedCollection<string, Parameter> GroupedFilteredParameters { get; private set; } = new();

    public CommandBar()
    {
        InitializeComponent();
    }

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(CommandBar), new PropertyMetadata(null));

    public CollectionViewSource ViewSource
    {
        get => (CollectionViewSource)GetValue(ViewSourceProperty);
        set => SetValue(ViewSourceProperty, value);
    }

    public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register("ViewSourceItems", typeof(CollectionViewSource), typeof(CommandBar), new PropertyMetadata(null));

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
        DependencyProperty.Register("SearchInput", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

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
        DependencyProperty.Register("GroupingValue", typeof(string), typeof(CommandBar), new PropertyMetadata("abc"));

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
        DependencyProperty.Register("FilterValue", typeof(string), typeof(CommandBar), new PropertyMetadata("None"));

    public bool CanShowUnsavedParameters
    {
        get => (bool)GetValue(CanShowUnsavedParametersProperty);
        set
        {
            SetValue(CanShowUnsavedParametersProperty, value);

            if (!value && IsUnsavedParametersSelected)
            {
                SearchInput = string.Empty;
                FilterParameter(SearchInput);
                IsUnsavedParametersSelected = false;
            }
        }
    }

    public static readonly DependencyProperty CanShowUnsavedParametersProperty =
        DependencyProperty.Register("CanShowUnsavedParameters", typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public bool IsUnsavedParametersSelected
    {
        get => (bool)GetValue(IsUnsavedParametersSelectedProperty);
        set
        {
            SetValue(IsUnsavedParametersSelectedProperty, value);
            btn_AllAppsButton.IsEnabled = value;
        }
    }

    public static readonly DependencyProperty IsUnsavedParametersSelectedProperty =
        DependencyProperty.Register("IsUnsavedParametersSelected", typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public bool CanShowErrorsParameters
    {
        get => (bool)GetValue(CanShowErrorsParametersProperty);
        set
        {
            SetValue(CanShowErrorsParametersProperty, value);

            //if (!value && IsUnsavedParametersSelected)
            //{
            //    SearchInput = string.Empty;
            //    FilterParameter(SearchInput);
            //    IsUnsavedParametersSelected = false;
            //}
        }
    }

    public static readonly DependencyProperty CanShowErrorsParametersProperty =
        DependencyProperty.Register("CanShowErrorsParameters", typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

    public ICommand SaveAllCommand
    {
        get => (ICommand)GetValue(SaveAllCommandProperty);
        set => SetValue(SaveAllCommandProperty, value);
    }

    public static readonly DependencyProperty SaveAllCommandProperty =
        DependencyProperty.Register("SaveAllCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

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
        var groupedParameters = ItemSource.Values.Where(FilterViewSearchInput(searchInput)).
                                                GroupBy(GroupView()).
                                                OrderBy(g => g.Key);
        foreach (var group in groupedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }

        if (ViewSource is not null)
        {
            ViewSource.Source = GroupedFilteredParameters;
        }
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
                    result = Enum.TryParse(FilterValue, true, out ParameterBase.ParameterTypValue filterTypEnum);
                    if (result)
                    {
                        return p => p.Name != null && p.ParameterTyp == filterTypEnum;
                    }
                    return p => p.Name != null;

                default:
                    result = Enum.TryParse(FilterValue, true, out ParameterBase.ParameterCategoryValue filterCatEnum);
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
                                                || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase));

                case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                    result = Enum.TryParse(FilterValue, true, out ParameterBase.ParameterTypValue filterTypEnum);
                    if (result)
                    {
                        return p => ((p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Value != null && p.Value.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                                                    || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)))
                                                    && p.ParameterTyp == filterTypEnum;

                    }
                    return p => p.Name != null;

                default:
                    result = Enum.TryParse(FilterValue, true, out ParameterBase.ParameterCategoryValue filterCatEnum);
                    if (result)
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

    private void ShowAllAppsButton_Click(object sender, RoutedEventArgs e)
    {
        SearchInput = string.Empty;
        FilterParameter(SearchInput);
        IsUnsavedParametersSelected = false;
    }

    private void ShowErrorsAppsButton_Click(object sender, RoutedEventArgs e)
    {
        SetErrorsParameterView();
        IsUnsavedParametersSelected = true;
    }

    private void ShowUnsavedAppsButton_Click(object sender, RoutedEventArgs e)
    {
        SetUnsavedParameterView();
        IsUnsavedParametersSelected = true;
    }

    private void SetUnsavedParameterView()
    {
        GroupedFilteredParameters.Clear();
        var unsavedParameters = ItemSource.Values.Where(p => p.IsDirty).
                                                GroupBy(GroupView()).
                                                OrderBy(g => g.Key);
        foreach (var group in unsavedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }
        ViewSource.Source = GroupedFilteredParameters;
    }

    private void SetErrorsParameterView()
    {
        GroupedFilteredParameters.Clear();
        var unsavedParameters = ItemSource.Values.Where(p => p.HasErrors).
                                                GroupBy(GroupView()).
                                                OrderBy(g => g.Key);
        foreach (var group in unsavedParameters)
        {
            GroupedFilteredParameters.Add(new ObservableGroup<string, Parameter>(group.Key, group));
        }
        ViewSource.Source = GroupedFilteredParameters;
    }
}