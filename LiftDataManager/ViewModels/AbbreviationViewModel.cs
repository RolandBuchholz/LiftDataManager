using CommunityToolkit.Mvvm.Collections;
using LiftDataManager.Core.DataAccessLayer.Models;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class AbbreviationViewModel : ObservableObject, INavigationAwareEx
{
    private readonly ParameterContext _parametercontext;
    public ObservableCollection<Abbreviation> Abbreviations { get; set; } = [];
    public ObservableGroupedCollection<string, Abbreviation> GroupedFilteredAbbreviations { get; private set; } = [];
    public CollectionViewSource GroupedAbbreviations { get; set; }


    public AbbreviationViewModel(ParameterContext parametercontext)
    {
        GroupedAbbreviations = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
        _parametercontext = parametercontext;
    }

    [ObservableProperty]
    public partial string? SearchValue { get; set; }
    partial void OnSearchValueChanged(string? value)
    {
        GroupAndFilterAbbreviationsAsync().SafeFireAndForget();
    }

    private async Task GetAbbreviationsFromDatabaseAsync()
    {
        var abbreviations = _parametercontext.Set<Abbreviation>();
        Abbreviations.AddRange(abbreviations);
        await Task.CompletedTask;
    }

    private async Task GroupAndFilterAbbreviationsAsync()
    {
        var groupedAbbreviations = Abbreviations.Where(FilterViewSearchValue(SearchValue))
                                                .GroupBy(g => g.ShortName[0].ToString().ToUpper(new CultureInfo("de-DE", false)))
                                                .OrderBy(g => g.Key);
        GroupedFilteredAbbreviations.Clear();
        foreach (var group in groupedAbbreviations)
        {
            GroupedFilteredAbbreviations.Add(new ObservableGroup<string, Abbreviation>(group.Key, group));
        }
        GroupedAbbreviations.Source = GroupedFilteredAbbreviations;
        await Task.CompletedTask;
    }

    private static Func<Abbreviation, bool> FilterViewSearchValue(string? searchValue)
    {
        return string.IsNullOrWhiteSpace(searchValue) ?
            p => true :
            p => (p.Name != null && p.Name.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
              || (p.ShortName != null && p.ShortName.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase));
    }

    public async void OnNavigatedTo(object parameter)
    {
        await GetAbbreviationsFromDatabaseAsync();
        await GroupAndFilterAbbreviationsAsync();
    }

    public void OnNavigatedFrom()
    {

    }
}