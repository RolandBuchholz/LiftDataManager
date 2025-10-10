using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.Models;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class SafetyComponentsRecordingViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    //public ObservableCollection<Abbreviation> Abbreviations { get; set; } = [];
    //public ObservableGroupedCollection<string, Abbreviation> GroupedFilteredAbbreviations { get; private set; } = [];
    //public CollectionViewSource GroupedAbbreviations { get; set; }


    public SafetyComponentsRecordingViewModel(IParameterDataService parameterDataService, SafetyComponentRecordContext safetyComponentRecordContext, IDialogService dialogService, IInfoCenterService infoCenterService,
                              ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        //GroupedAbbreviations = new CollectionViewSource
        //{
        //    IsSourceGrouped = true
        //};
        _safetyComponentRecordContext = safetyComponentRecordContext;
    }

    //[ObservableProperty]
    //public partial string? SearchValue { get; set; }
    //partial void OnSearchValueChanged(string? value)
    //{
    //    GroupAndFilterAbbreviationsAsync().SafeFireAndForget();
    //}

    //private async Task GetAbbreviationsFromDatabaseAsync()
    //{
    //    var abbreviations = _parametercontext.Set<Abbreviation>();
    //    Abbreviations.AddRange(abbreviations);
    //    await Task.CompletedTask;
    //}

    //private async Task GroupAndFilterAbbreviationsAsync()
    //{
    //    var groupedAbbreviations = Abbreviations.Where(FilterViewSearchValue(SearchValue))
    //                                            .GroupBy(g => g.ShortName[0].ToString().ToUpper(new CultureInfo("de-DE", false)))
    //                                            .OrderBy(g => g.Key);
    //    GroupedFilteredAbbreviations.Clear();
    //    foreach (var group in groupedAbbreviations)
    //    {
    //        GroupedFilteredAbbreviations.Add(new ObservableGroup<string, Abbreviation>(group.Key, group));
    //    }
    //    GroupedAbbreviations.Source = GroupedFilteredAbbreviations;
    //    await Task.CompletedTask;
    //}

    //private static Func<Abbreviation, bool> FilterViewSearchValue(string? searchValue)
    //{
    //    return string.IsNullOrWhiteSpace(searchValue) ?
    //        p => true :
    //        p => (p.Name != null && p.Name.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
    //          || (p.ShortName != null && p.ShortName.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase));
    //}

    public async void OnNavigatedTo(object parameter)
    {
        //await GetAbbreviationsFromDatabaseAsync();
        //await GroupAndFilterAbbreviationsAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {

    }
}