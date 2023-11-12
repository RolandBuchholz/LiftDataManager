using Cogs.Collections;
using System.Collections.ObjectModel;

namespace LiftDataManager.Controls;

public sealed partial class HistorySearchBar : UserControl
{
    const string defaultAuthorName = "All Authors";
    const string defaultRevisionName = "All Revisions";
    
    public List<LiftHistoryEntry> FilteredEntrys { get; set; }
    public ObservableCollection<string> Authors { get; set; }
    private ObservableDictionary<string, DateTime> RevisionsDictionary { get; set; }

    public HistorySearchBar()
    {
        InitializeComponent();
        FilteredEntrys ??= new();
        Authors ??= new();
        RevisionsDictionary ??= new();
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
        DependencyProperty.Register(nameof(StartDate), typeof(DateTimeOffset?), typeof(HistorySearchBar), new PropertyMetadata(null));

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
        DependencyProperty.Register(nameof(EndDate), typeof(DateTimeOffset?), typeof(HistorySearchBar), new PropertyMetadata(null));

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
        DependencyProperty.Register(nameof(ItemSource), typeof(List<LiftHistoryEntry>), typeof(HistorySearchBar), new PropertyMetadata(null));

    public CollectionViewSource ViewSource
    {
        get => (CollectionViewSource)GetValue(ViewSourceProperty);
        set => SetValue(ViewSourceProperty, value);
    }

    public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register(nameof(ViewSource), typeof(CollectionViewSource), typeof(HistorySearchBar), new PropertyMetadata(null));

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
        DependencyProperty.Register(nameof(SearchInput), typeof(string), typeof(HistorySearchBar), new PropertyMetadata(string.Empty));

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
        DependencyProperty.Register(nameof(Author), typeof(string), typeof(HistorySearchBar), new PropertyMetadata(defaultAuthorName));

    public string Revision
    {
        get => (string)GetValue(RevisionProperty);
        set
        {
            SetValue(RevisionProperty, value);
            FilterParameter(SearchInput);
        }
    }

    public static readonly DependencyProperty RevisionProperty =
        DependencyProperty.Register(nameof(Revision), typeof(string), typeof(HistorySearchBar), new PropertyMetadata(defaultRevisionName));

    private void InitializeValues()
    {
        SetAuthors();
        SetRevisions();
        FilterParameter(SearchInput);
    }

    private void SetAuthors()
    {
        Authors.Clear();
        Authors.Add(defaultAuthorName);
        var authors = ItemSource.Select(x => x.Author).Distinct();
        foreach (var author in authors)
        {
            if (!string.IsNullOrWhiteSpace(author))
                Authors.Add(author);
        }
    }

    private void SetRevisions()
    {
        RevisionsDictionary.Clear();
        RevisionsDictionary.TryAdd(defaultRevisionName, DateTime.MinValue);
        var revisions = ItemSource.Where(x => x.Name == "var_Index");

        foreach (var revision in revisions)
        {
            if (!string.IsNullOrWhiteSpace(revision.NewValue))
            {
                RevisionsDictionary.TryAdd($"Stand : {revision.NewValue}", revision.TimeStamp);
            }
            else
            {
                RevisionsDictionary.TryAdd("Erster Stand", DateTime.MinValue);
            }      
        }
    }

    private void FilterParameter(string searchInput)
    {
        FilteredEntrys.Clear();
        IEnumerable<LiftHistoryEntry> filteredEntrys;
        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.MaxValue;

        if (StartDate is not null)
        {
            var currentStartTime = StartDate.GetValueOrDefault().DateTime;
            start = currentStartTime.Subtract(currentStartTime.TimeOfDay);
        }
        if (EndDate is not null)
        {
            var currentEndTime = EndDate.GetValueOrDefault().DateTime;
            end = currentEndTime.AddDays(1).Subtract(currentEndTime.TimeOfDay);
        }

        if (!string.Equals(Revision, defaultRevisionName))
        {
            DateTime startRev = DateTime.MinValue;
            DateTime endRev = DateTime.MaxValue;

            if (string.Equals(Revision, "Erster Stand"))
            {
                if (RevisionsDictionary.TryGetValue($"Stand : A", out DateTime endValue))
                {
                    endRev = endValue;
                }
            }
            else
            {
                if (RevisionsDictionary.TryGetValue(Revision, out DateTime startValue))
                {
                    startRev = startValue;
                }
                var nextRevision = RevisionHelper.GetNextRevision(Revision.Replace("Stand :", ""));
                if (RevisionsDictionary.TryGetValue($"Stand : {nextRevision}", out DateTime endValue))
                {
                    endRev = endValue;
                }
            }

            start = start > startRev ? start : startRev;
            end = end < endRev ? end : endRev;
        }

        if (start == DateTime.MinValue && end == DateTime.MinValue)
        {
            filteredEntrys = ItemSource.Where(FilterViewSearchInput(searchInput));
        }
        else
        {
            DateRange range = new(start, end);
            filteredEntrys = ItemSource.Where(FilterViewSearchInput(searchInput)).Where(p => range.WithInRange(p.TimeStamp));
        }

        if (ViewSource is not null)
        {
            ViewSource.Source = filteredEntrys.OrderByDescending(x => x.TimeStamp).ToList();
        }
    }

    private Func<LiftHistoryEntry, bool> FilterViewSearchInput(string searchInput)
    {
        if (string.IsNullOrWhiteSpace(searchInput) && Author == defaultAuthorName)
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
                     && ((Author == defaultAuthorName) || p.Author == Author);
        }
    }

    private void ResetFilterClick(object sender, RoutedEventArgs e)
    {
        Author = defaultAuthorName;
        Revision = defaultRevisionName;
        StartDate = null;
        EndDate = null;
        SearchInput = string.Empty;
    }
}