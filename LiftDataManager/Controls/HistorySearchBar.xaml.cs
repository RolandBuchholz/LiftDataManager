using System.Collections.ObjectModel;

namespace LiftDataManager.Controls;

public sealed partial class HistorySearchBar : UserControl
{
    public List<LiftHistoryEntry> FilteredEntrys { get; set; }
    public ObservableCollection<string> Authors { get; set; }

    public HistorySearchBar()
    {
        InitializeComponent();
        FilteredEntrys ??= new();
        Authors ??= new();
    }

    public DateTimeOffset? StartDate
    {
        get { return (DateTimeOffset?)GetValue(StartDateProperty); }
        set
        {
            if (EndDate != DateTimeOffset.MinValue && value > EndDate)
                EndDate = value.Value;
            SetValue(StartDateProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty StartDateProperty =
        DependencyProperty.Register("StartDate", typeof(DateTimeOffset?), typeof(HistorySearchBar), new PropertyMetadata(null));

    public DateTimeOffset? EndDate
    {
        get { return (DateTimeOffset?)GetValue(EndDateProperty); }
        set
        {
            if (value < StartDate)
                StartDate = value.Value;
            SetValue(EndDateProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty EndDateProperty =
        DependencyProperty.Register("EndDate", typeof(DateTimeOffset?), typeof(HistorySearchBar), new PropertyMetadata(null));

    public List<LiftHistoryEntry> ItemSource
    {
        get => (List<LiftHistoryEntry>)GetValue(ItemSourceProperty);
        set
        {
            SetValue(ItemSourceProperty, value);
            InitializeValues();
        }
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(List<LiftHistoryEntry>), typeof(HistorySearchBar), new PropertyMetadata(null));

    public CollectionViewSource ViewSource
    {
        get => (CollectionViewSource)GetValue(ViewSourceProperty);
        set => SetValue(ViewSourceProperty, value);
    }

    public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register("ViewSourceItems", typeof(CollectionViewSource), typeof(HistorySearchBar), new PropertyMetadata(null));

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
        DependencyProperty.Register("SearchInput", typeof(string), typeof(HistorySearchBar), new PropertyMetadata(string.Empty));

    public string Author
    {
        get => (string)GetValue(AuthorProperty);
        set
        {
            SetValue(AuthorProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty AuthorProperty =
        DependencyProperty.Register("Author", typeof(string), typeof(HistorySearchBar), new PropertyMetadata("All Authors"));

    private void InitializeValues()
    {
        SetAuthors();
        FilterParameter(SearchInput);
    }

    private void SetAuthors()
    {
        Authors.Clear();
        Authors.Add("All Authors");
        var authors = ItemSource.Select(x => x.Author).Distinct();
        foreach (var author in authors)
        {
            if (!string.IsNullOrWhiteSpace(author))
                Authors.Add(author);
        }
    }

    private void FilterParameter(string searchInput)
    {
        FilteredEntrys.Clear();
        var start = StartDate.GetValueOrDefault().DateTime;
        var end = EndDate.GetValueOrDefault().DateTime;
        IEnumerable<LiftHistoryEntry> filteredEntrys;
        if (start == DateTime.MinValue && end == DateTime.MinValue)
        {
            filteredEntrys = ItemSource.Where(FilterViewSearchInput(searchInput));
        }
        else
        {
            DateRange range = new(DateOnly.FromDateTime(start), DateOnly.FromDateTime(end));
            filteredEntrys = ItemSource.Where(FilterViewSearchInput(searchInput)).Where(p => range.WithInRange(p.TimeStamp));
        }

        if (ViewSource is not null)
        {
            ViewSource.Source = filteredEntrys.OrderByDescending(x => x.TimeStamp).ToList();
        }
    }

    private Func<LiftHistoryEntry, bool> FilterViewSearchInput(string searchInput)
    {
        if (string.IsNullOrWhiteSpace(searchInput) && Author == "All Authors")
            return p => p.Name != null;
        if (string.IsNullOrWhiteSpace(searchInput))
        {
            return p => p.Name != null && p.Author == Author;
        }
        else
        {
            return p => (p.Name != null && p.Name.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)
                     || (p.DisplayName != null && p.DisplayName.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                     || (p.NewValue != null && p.NewValue.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase))
                     || (p.Comment != null && p.Comment.Contains(searchInput, StringComparison.CurrentCultureIgnoreCase)))
                     && ((Author == "All Authors") || p.Author == Author);
        }
    }
}