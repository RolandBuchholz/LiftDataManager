using LiftDataManager.Core.DataAccessLayer.Models;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class DataBaseEditViewModel : DataViewModelBase, INavigationAware
{
    private readonly ISettingService _settingService;
    private readonly ParameterContext _editableparametercontext;
    private readonly ILogger<DataBaseEditViewModel> _logger;

    public DataBaseEditViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                                 ISettingService settingsSelectorService, ILogger<DataBaseEditViewModel> logger) :
                                 base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        DbContextOptionsBuilder editableOptions = new();
        editableOptions.UseSqlite(App.GetConnectionString(false));
        _editableparametercontext = new ParameterContext(editableOptions.Options);
        _logger = logger;
        parameterDtos ??= new();
        filteredParameterDtos ??= new();
        GetDropdownValues();
    }

    public void DataGrid_CurrentCellChanged(object sender, EventArgs e)
    {
        CanChangeParameters = _editableparametercontext.ChangeTracker.HasChanges();
    }

    [ObservableProperty]
    private string? dataBasePath;

    [ObservableProperty]
    private List<ParameterDto> parameterDtos;

    [ObservableProperty]
    private List<ParameterDto> filteredParameterDtos;

    [ObservableProperty]
    private List<string?>? allTables;

    [ObservableProperty]
    private List<string?>? filteredAllTables;

    [ObservableProperty]
    private IEnumerable<object> databaseTable = Enumerable.Empty<object>();

    [ObservableProperty]
    private string? selectedTable;
    partial void OnSelectedTableChanged(string? value)
    {
        if (value is not null)
        {
            var entityType = TypeFinder.FindLiftmanagerType(value.Substring(0, value.Length - 1));

            if (entityType is not null)
            {
                var table = _editableparametercontext.Query(entityType);
                if (table is not null)
                {
                    DatabaseTable = (IEnumerable<object>)table;
                    CanAddTableValue= true;
                    ShowTable = true;
                }
            }
        }
        else
        {
            DatabaseTable = Enumerable.Empty<object>();
            ShowTable = false;
            CanAddTableValue = false;
        }
    }

    [ObservableProperty]
    private List<ParameterTyp>? parameterTyps;

    [ObservableProperty]
    private ParameterTyp? selectedParameterTyp;
    partial void OnSelectedParameterTypChanged(ParameterTyp? value)
    {
        if (value is not null)
        {
            if (value.Name != "DropDownList")
                SelectedDropdownlistTable = null;
            IsdropdownlistTablesVisible = value.Name == "DropDownList";
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private List<ParameterTypeCode>? parameterTypeCodes;

    [ObservableProperty]
    private ParameterTypeCode? selectedParameterTypeCode;
    partial void OnSelectedParameterTypeCodeChanged(ParameterTypeCode? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private List<ParameterCategory>? parameterCategorys;

    [ObservableProperty]
    private ParameterCategory? selectedParameterCategory;
    partial void OnSelectedParameterCategoryChanged(ParameterCategory? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private string? uniqueName;
    partial void OnUniqueNameChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private string? displayName;
    partial void OnDisplayNameChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private string? parameterValue;

    [ObservableProperty]
    private string? comment;

    [ObservableProperty]
    private bool? isKey;
    partial void OnIsKeyChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private bool? isDefaultUserEditable;
    partial void OnIsDefaultUserEditableChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private List<string?>? dropdownlistTables;

    [ObservableProperty]
    private bool isdropdownlistTablesVisible;

    [ObservableProperty]
    private string? selectedDropdownlistTable;
    partial void OnSelectedDropdownlistTableChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    private string? removeParameterId;
    partial void OnRemoveParameterIdChanged(string? value)
    {
        CanRemoveParameter = !string.IsNullOrWhiteSpace(value) && value != "0";
    }

    [ObservableProperty]
    private bool showTable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveParameterFromDataBaseCommand))]
    private bool canRemoveParameter;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddParameterToDataBaseCommand))]
    private bool canAddParameter;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddValueToTableCommand))]
    private bool canAddTableValue;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeParametersDataCommand))]
    private bool canChangeParameters;

    private void CheckCanAddParameter()
    {
        CanAddParameter = SelectedParameterTyp is not null &&
                          ParameterTypeCodes is not null &&
                          SelectedParameterCategory is not null &&
                          !string.IsNullOrWhiteSpace(UniqueName) &&
                          !string.IsNullOrWhiteSpace(DisplayName) &&
                          IsKey is not null &&
                          IsDefaultUserEditable is not null &&
                          (SelectedParameterTyp.Name != "DropDownList" || !string.IsNullOrWhiteSpace(SelectedDropdownlistTable));
    }

    [ObservableProperty]
    private bool showParameterDeleteMessage;

    [ObservableProperty]
    private string? parameterDeleteMessage;

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
    private string? searchTableInput;
    partial void OnSearchTableInputChanged(string? value)
    {
        if (value is not null)
        {
            FilterTable();
        }
        else
        {
            SearchInput = string.Empty;
            FilterTable();
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

    [RelayCommand(CanExecute = nameof(CanRemoveParameter))]
    private void RemoveParameterFromDataBase()
    {
        var deletableParameterDto = _editableparametercontext.Find<ParameterDto>(Convert.ToInt32(RemoveParameterId));

        if (deletableParameterDto is not null)
        {
            try
            {
                _editableparametercontext.Remove(deletableParameterDto);
                _editableparametercontext.SaveChanges();
                _ = RefreshDataBaseAsync();
                ParameterDeleteMessage = $"Parameter {deletableParameterDto.DisplayName} erfolgreich aus der Datenbank gelöscht !";
                _logger.LogInformation(60171, "parameter {deletableParameterDto.DisplayName} successfully deleted from database", deletableParameterDto.DisplayName);
            }
            catch
            {
                ParameterDeleteMessage = $"Parameter {deletableParameterDto.DisplayName} konnte nicht gelöscht werden!";
                _logger.LogWarning(61072, "Failed to delete parameter {deletableParameterDto.DisplayName} from database", deletableParameterDto.DisplayName);
            }
        }
        else
        {
            ParameterDeleteMessage = $"Parameter mit der Id {RemoveParameterId} konnte in der Datenbank nicht gefunden werden !";
            _logger.LogWarning(61072, "Parameter with Id {RemoveParameterId} not found in the database", RemoveParameterId);
        }
        ShowParameterDeleteMessage = true;
        RemoveParameterId = null;
    }

    [RelayCommand(CanExecute = nameof(CanAddParameter))]
    private void AddParameterToDataBase()
    {
        try
        {
            var newParameterDto = new ParameterDto
            {
                ParameterTyp = SelectedParameterTyp,
                ParameterTypeCode = SelectedParameterTypeCode,
                ParameterCategory = SelectedParameterCategory,
                Name = "var_" + UniqueName,
                DisplayName = DisplayName,
                Value = ParameterValue,
                Comment = Comment,
                IsKey = (bool)IsKey!,
                DefaultUserEditable = (bool)IsDefaultUserEditable!,
                DropdownList = SelectedDropdownlistTable
            };
            _editableparametercontext.Add(newParameterDto);
            _editableparametercontext.SaveChanges();

            _logger.LogInformation(60174, "parameter {DisplayName} successfully add to database", DisplayName);

            _ = RefreshDataBaseAsync();
            SelectedParameterTyp = null;
            SelectedParameterTypeCode = null;
            SelectedParameterCategory = null;
            UniqueName = null;
            DisplayName = null;
            ParameterValue = null;
            Comment = null;
            IsKey = null;
            IsDefaultUserEditable = null;
            SelectedDropdownlistTable = null;
            CanAddParameter = false;
        }
        catch
        {
            _logger.LogWarning(61075, "Failed to add parameter {DisplayName} to database", DisplayName);
        }
    }
    private T DetachEntity<T>(T entity) where T : class
    {
        _editableparametercontext.Entry(entity).State = EntityState.Detached;
        if (entity.GetType().GetProperty("Id") != null)
        {
            entity.GetType().GetProperty("Id")?.SetValue(entity, 0);
            var oldName = entity.GetType().GetProperty("Name")?.GetValue(entity, null);
            entity.GetType().GetProperty("Name")?.SetValue(entity, "Copy_of_" + oldName);
        }
        return entity;
    }

    [RelayCommand(CanExecute = nameof(CanAddTableValue))]
    private void AddValueToTable()
    {
        try
        {
            if (SelectedTable is not null)
            {
                var oldEntity = DatabaseTable.LastOrDefault();
                if (oldEntity is not null)
                {
                    var newEntity = DetachEntity(oldEntity);
                    _editableparametercontext.Add(newEntity);
                    _editableparametercontext.SaveChanges();
                    // TODO Parameteränderung in History schreiben
                    _logger.LogInformation(60176, "Value {DisplayName} successfully copy in table {SelectedTable}", SelectedTable);
                    var temp = SelectedTable;
                    SelectedTable = null;
                    SelectedTable = temp;
                }
            }
        }
        catch
        {

            _logger.LogWarning(61077, "Failed to copy last value in table {SelectedTable}", SelectedTable);
        }
    }

    [RelayCommand(CanExecute = nameof(CanChangeParameters))]
    private void ChangeParametersData()
    {
        if (_editableparametercontext.ChangeTracker.HasChanges())
        {
            try
            {
                var changedEntities = _editableparametercontext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
                foreach (var entry in changedEntities)
                {
                    // TODO Parameteränderung in History schreiben
                    _logger.LogInformation(60176, "ChangeParameter: {entry.DebugView.LongView}", entry.DebugView.LongView);
                }
                _editableparametercontext.SaveChanges();
                CanChangeParameters = false;
            }
            catch
            {
                _logger.LogWarning(61077, "Failed to change parameters");
            }
        }
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

    private void FilterTable()
    {
        if (AllTables is not null)
        {
            if (SearchTableInput is not null)
            {
                FilteredAllTables = AllTables.Where(x => x.Contains(SearchTableInput, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else
            {
                FilteredAllTables = AllTables;
            }
        }
    }

    [RelayCommand]
    private async Task RefreshDataBaseAsync()
    {
        parameterDtos.Clear();
        ParameterDtos = _editableparametercontext.ParameterDtos!
                                     .Include(x => x.ParameterTyp)
                                     .Include(x => x.ParameterTypeCode)
                                     .Include(x => x.ParameterCategory)
                                     .ToList();
        filteredParameterDtos.Clear();
        FilterValue = "None";
        SearchInput = string.Empty;
        if (ParameterDtos is not null)
        {
            FilteredParameterDtos = ParameterDtos;
        }
        await Task.CompletedTask;
    }

    private void GetDropdownValues()
    {
        ParameterTyps = _editableparametercontext.Set<ParameterTyp>().ToList();
        ParameterTypeCodes = _editableparametercontext.Set<ParameterTypeCode>().ToList();
        ParameterCategorys = _editableparametercontext.Set<ParameterCategory>().ToList();
        DropdownlistTables = _editableparametercontext.DropdownValues!.Select(x => x.Base)
                                                                      .Distinct()
                                                                      .ToList();
        var allTablesfromDB = _editableparametercontext.Model.GetEntityTypes().Select(t => t.GetTableName())
                                                                         .Where(x => x is not null)
                                                                         .Order()
                                                                         .ToList();
        var ignoredTables = new List<string>()
        {
            "ParameterCategorys",
            "ParameterTypeCodes",
            "ParameterTyps",
            "ParameterDtos",
            "LoadTable6s",
            "LoadTable7s",
            "PersonsTable8s",
            "LiftDataManagerVersions"
        };

        AllTables = allTablesfromDB.Except(ignoredTables).ToList();


        FilteredAllTables = AllTables;
    }

    public void OnNavigatedTo(object parameter)
    {
        Adminmode = _settingService.Adminmode;
        DataBasePath = _settingService.PathDataBase;
        _ = RefreshDataBaseAsync();
    }

    public void OnNavigatedFrom()
    {
        _editableparametercontext.Dispose();
    }
}
