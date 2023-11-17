using LiftDataManager.Core.DataAccessLayer.Models;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class DataBaseEditViewModel : DataViewModelBase, INavigationAware
{
    private readonly ISettingService _settingService;
    private readonly IVaultDataService _vaultDataService;
    private ParameterContext? _editableparametercontext;
    private readonly ILogger<DataBaseEditViewModel> _logger;

    public DataBaseEditViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                                 ISettingService settingsSelectorService, IVaultDataService vaultDataService, ILogger<DataBaseEditViewModel> logger) :
                                 base(parameterDataService, dialogService, navigationService)
    {
        _settingService = settingsSelectorService;
        _vaultDataService = vaultDataService;
        _logger = logger;
        parameterDtos ??= new();
        filteredParameterDtos ??= new();
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
    private List<DatabaseTableValueModification>? tableHistory;

    [ObservableProperty]
    private string? selectedTable;
    partial void OnSelectedTableChanged(string? value)
    {
        RefreshSelectedTable(value);
    }

    private void RefreshSelectedTable(string? tableName)
    {
        DatabaseTable = Enumerable.Empty<object>();
        if (tableName is not null)
        {
            if (_editableparametercontext is null)
                return;
            var entityType = TypeFinder.FindLiftmanagerType(tableName[..^1]);

            if (entityType is not null)
            {
                var table = _editableparametercontext.Query(entityType);
                if (table is not null)
                {
                    DatabaseTable = (IEnumerable<object>)table;
                    CanAddTableValue = true;
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

    [RelayCommand]
    public void CheckParameterChanged()
    {
        if (_editableparametercontext is null)
            return;
        CanChangeParameters = _editableparametercontext.ChangeTracker.HasChanges();
    }

    [RelayCommand(CanExecute = nameof(CanRemoveParameter))]
    private void RemoveParameterFromDataBase()
    {
        if (_editableparametercontext is null)
            return;
        var id = Convert.ToInt32(RemoveParameterId);
        var deletableParameterDto = _editableparametercontext.Find<ParameterDto>(id);

        if (deletableParameterDto is not null)
        {
            try
            {
                _editableparametercontext.Remove(deletableParameterDto);
                _editableparametercontext.SaveChanges();
                _ = RefreshDataBaseAsync();
                ParameterDeleteMessage = $"Parameter {deletableParameterDto.DisplayName} erfolgreich aus der Datenbank gelöscht !";
                SetDatabaseTableValueModification("delete", "ParameterDtos", id, deletableParameterDto.DisplayName!);
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

    [RelayCommand]
    private void RemoveRowFromDataBaseTable(object deletableRow)
    {
        if (_editableparametercontext is null)
            return;
        if (deletableRow is not null)
        {
            var type = deletableRow.GetType();
            var tableName = type.Name + "s";
            var idEntity = ((BaseEntity)deletableRow).Id;
            var nameEntity = ((BaseEntity)deletableRow).Name;

            try
            {
                _editableparametercontext.Remove(deletableRow);
                _editableparametercontext.SaveChanges();
                RefreshSelectedTable(SelectedTable);
                _logger.LogInformation(60177, " from table: {table} Id: {tableId} Name: {nameEntity} successfully deleted", tableName, idEntity, nameEntity);
                SetDatabaseTableValueModification("delete", tableName, idEntity, nameEntity);
            }
            catch
            {
                _logger.LogWarning(61078, "Failed to delete value {name} from table {table}", nameEntity, tableName);
            }
        }
        else
        {
            _logger.LogWarning(61079, "Value not found in database");
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddParameter))]
    private void AddParameterToDataBase()
    {
        if (_editableparametercontext is null)
            return;
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
            var addedParameterDto = _editableparametercontext.Add(newParameterDto);
            _editableparametercontext.SaveChanges();
            var newEntity = _editableparametercontext.Entry(addedParameterDto.Entity);
            SetDatabaseTableValueModification("add", "ParameterDtos", newEntity.Entity.Id, newEntity.Entity.DisplayName!, newEntity.DebugView.LongView);
            _logger.LogInformation(60174, "parameter: {newEntity} successfully add to database", newEntity.DebugView.LongView);
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
        if (_editableparametercontext is null)
        {
            return entity;
        }
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
        if (_editableparametercontext is null)
            return;
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
                    SetDatabaseTableValueModification("copy", SelectedTable, ((BaseEntity)newEntity).Id, ((BaseEntity)newEntity).Name[8..], ((BaseEntity)newEntity).Name);
                    _logger.LogInformation(60176, "Value: {Name} successfully copy in table: {SelectedTable}", ((BaseEntity)newEntity).Name, SelectedTable);
                    RefreshSelectedTable(SelectedTable);
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
        if (_editableparametercontext is null)
            return;
        if (_editableparametercontext.ChangeTracker.HasChanges())
        {
            try
            {
                var changedEntities = _editableparametercontext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
                foreach (var entity in changedEntities)
                {
                    var type = entity.Entity.GetType();
                    var tableName = type.Name + "s";
                    var idEntity = ((BaseEntity)entity.Entity).Id;
                    var nameEntity = ((BaseEntity)entity.Entity).Name;
                    var newEntityValue = entity.DebugView.LongView;

                    SetDatabaseTableValueModification("modify", tableName, idEntity, nameEntity, newEntityValue);
                    _logger.LogInformation(60176, "ChangeParameter: {entry.DebugView.LongView}", entity.DebugView.LongView);
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

    private void SetDatabaseTableValueModification(string operation, string tableName, int id, string entityName, string? newEntityValue = null)
    {
        if (_editableparametercontext is null)
            return;
        var timestamp = DateTime.Now.ToString();
        var userName = string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) ? "no user detected" : System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        var newValueModification = new DatabaseTableValueModification
        {
            Timestamp = timestamp,
            Operation = operation,
            Name = userName,
            TableName = tableName,
            EntityId = id,
            EntityName = entityName,
            NewEntityValue = newEntityValue
        };
        _editableparametercontext.Add(newValueModification);
        _editableparametercontext.SaveChanges();
    }

    private void FilterParameterDtos()
    {
        if (string.IsNullOrWhiteSpace(SearchInput))
        {
            FilteredParameterDtos = FilterValue switch
            {
                "None" => ParameterDtos,
                "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList" => ParameterDtos.Where(x => x.ParameterTyp!.Name == filterValue).ToList(),
                "AllgemeineDaten" or "Schacht" or "Bausatz" or "Fahrkorb" or "Tueren" or "AntriebSteuerungNotruf" or "Signalisation"
                                                       or "Wartung" or "MontageTUEV" or "RWA" or "FilterSonstiges" or "KommentareVault" or "CFP" => ParameterDtos.Where(x => x.ParameterCategory!.Name == filterValue).ToList(),
                _ => ParameterDtos,
            };
        }
        else
        {
            FilteredParameterDtos = FilterValue switch
            {
                "None" => ParameterDtos.Where(
                                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)).ToList(),
                "Text" or "NumberOnly" or "Date" or "Boolean" or "DropDownList" => ParameterDtos.Where(
                                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase))
                                                                         .Where(x => x.ParameterTyp!.Name == filterValue).ToList(),
                "AllgemeineDaten" or "Schacht" or "Bausatz" or "Fahrkorb" or "Tueren" or "AntriebSteuerungNotruf" or "Signalisation"
                                           or "Wartung" or "MontageTUEV" or "RWA" or "FilterSonstiges" or "KommentareVault" or "CFP" => ParameterDtos.Where(
                                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase))
                                                                        .Where(x => x.ParameterCategory!.Name == filterValue).ToList(),
                _ => ParameterDtos.Where(
                                        p => p.Name is not null && p.Name.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.DisplayName is not null && p.DisplayName.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Value is not null && p.Value.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)
                                          || p.Comment is not null && p.Comment.Contains(SearchInput, StringComparison.CurrentCultureIgnoreCase)).ToList(),
            };
        }
    }

    private void FilterTable()
    {
        if (AllTables is not null)
        {
            if (SearchTableInput is not null)
            {
                FilteredAllTables = AllTables.Where(x => x is not null && x.Contains(SearchTableInput, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
        if (_editableparametercontext is null)
            return;
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

    [RelayCommand]
    public void LoadDatabaseTableValueModificationHistory()
    {
        if (_editableparametercontext is null)
            return;
        TableHistory = _editableparametercontext.Set<DatabaseTableValueModification>().OrderByDescending(x => x.Id)
                                                                                      .ToList();
    }

    private void GetDropdownValues()
    {
        if (_editableparametercontext is null)
            return;
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
            "LiftDataManagerVersions",
            "DatabaseTableValueModifications"
        };

        AllTables = allTablesfromDB.Except(ignoredTables).ToList();


        FilteredAllTables = AllTables;
    }

    [RelayCommand]
    public async Task UpdateAutotransferXmlAsync()
    {
        var nameXml = "AutoDeskTransfer.xml";
        var downloadResult = await _vaultDataService.GetFileAsync(nameXml, false, true);

        if (downloadResult.ExitCode == 0 && downloadResult.CheckOutState == "CheckedOutByCurrentUser")
        {
            var updateResult = await _parameterDataService!.UpdateAutodeskTransferAsync(downloadResult.FullFileName!, parameterDtos);

            if (updateResult)
            {
                var uploadResult = await _vaultDataService.SetFileAsync(nameXml, true);
                if (uploadResult.ExitCode == 0)
                {
                    _logger.LogInformation(60177, "file upload: {uploadResult.FileName} successful", uploadResult.FileName);
                }
                else
                {
                    _logger.LogError(61078, "file upload: {uploadResult.FileName} failed", uploadResult.FileName);
                }
            }
        }
    }

    private void CreateEditableparametercontext()
    {
        DbContextOptionsBuilder editableOptions = new();
        editableOptions.UseSqlite(App.GetConnectionString(false));
        _editableparametercontext = new ParameterContext(editableOptions.Options);
    }

    public void OnNavigatedTo(object parameter)
    {
        Adminmode = _settingService.Adminmode;
        DataBasePath = _settingService.PathDataBase;
        if (Adminmode)
        {
            CreateEditableparametercontext();
            GetDropdownValues();
            _ = RefreshDataBaseAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        _editableparametercontext?.Dispose();
    }
}