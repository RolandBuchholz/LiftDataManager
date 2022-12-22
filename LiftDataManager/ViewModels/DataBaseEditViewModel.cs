using LiftDataManager.Core.DataAccessLayer.Models;

namespace LiftDataManager.ViewModels;

public partial class DataBaseEditViewModel : DataViewModelBase, INavigationAware
{
    private readonly ISettingService _settingService;
    private readonly ParameterContext _parametercontext;

    public DataBaseEditViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, 
                                 ISettingService settingsSelectorService, ParameterContext parametercontext) :
                                 base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        _parametercontext = parametercontext;
        parameterDtos ??= new();
        filteredParameterDtos ??= new();
    }

    [ObservableProperty]
    private string? dataBasePath;

    [ObservableProperty]
    private List<ParameterDto> parameterDtos;

    [ObservableProperty]
    private List<ParameterDto>filteredParameterDtos;

    [ObservableProperty]
    private string? searchInput;
    partial void OnSearchInputChanged(string? value)
    {
        if (value is not null)
        {
            FilterParameterDtos();
        }
        else
        {
            SearchInput = string.Empty;
            FilterParameterDtos();
        }
    }

    [ObservableProperty]
    private string? filterValue;
    partial void OnFilterValueChanged(string? value)
    {
        if (value is not null)
        {
            FilterParameterDtos();
        }
    }

    [RelayCommand]
    private void SetFilter(string filterValue)
    {
        FilterValue = filterValue;
    }

    private void FilterParameterDtos()
    {
        if (string.IsNullOrWhiteSpace(SearchInput))
        {
            switch (FilterValue)
            {
                case "None":
                    FilteredParameterDtos = ParameterDtos;
                    break;
                case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                    FilteredParameterDtos = ParameterDtos.Where(x => x.ParameterTyp!.Name == filterValue).ToList();
                    break;
                case "AllgemeineDaten" or "Schacht" or "Bausatz" or "Fahrkorb" or "Tueren" or "AntriebSteuerungNotruf" or "Signalisation" 
                                       or "Wartung" or "MontageTUEV" or "RWA" or "FilterSonstiges" or "KommentareVault" or "CFP":
                    FilteredParameterDtos = ParameterDtos.Where(x => x.ParameterCategory!.Name == filterValue).ToList();
                    break;
                default:
                    FilteredParameterDtos = ParameterDtos;
                    break;
            }
        }
        else
        {
            switch (FilterValue)
            {
                case "None":
                    FilteredParameterDtos = ParameterDtos.Where(
                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList":
                    FilteredParameterDtos = ParameterDtos.Where(
                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase))
                                                         .Where(x => x.ParameterTyp!.Name == filterValue).ToList();
                    break;
                case "AllgemeineDaten" or "Schacht" or "Bausatz" or "Fahrkorb" or "Tueren" or "AntriebSteuerungNotruf" or "Signalisation"
                           or "Wartung" or "MontageTUEV" or "RWA" or "FilterSonstiges" or "KommentareVault" or "CFP":
                    FilteredParameterDtos = ParameterDtos.Where(
                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase))
                                                        .Where(x => x.ParameterCategory!.Name == filterValue).ToList();
                    break;
                default:
                    FilteredParameterDtos = ParameterDtos.Where(
                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
            }
        }
    }

    [RelayCommand]
    private async Task RefreshDataBaseAsync()
    {
        parameterDtos.Clear();
        ParameterDtos = _parametercontext.ParameterDtos!
                                     .Include(x => x.ParameterTyp)
                                     .Include(x => x.ParameterTypeCode)
                                     .Include(x => x.ParameterCategory)
                                     .ToList();
        filteredParameterDtos.Clear();
        FilterValue = "None";
        SearchInput = string.Empty;
        if(ParameterDtos is not null)
        {
            FilteredParameterDtos = ParameterDtos;
        }
        await Task.CompletedTask;
    }

    public void OnNavigatedTo(object parameter)
    {
        Adminmode = _settingService.Adminmode;
        DataBasePath = _settingService.PathDataBase;
    }

    public void OnNavigatedFrom()
    {
    }
}
