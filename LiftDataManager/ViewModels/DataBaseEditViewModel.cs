using LiftDataManager.Core.DataAccessLayer.Models;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using WinUI.TableView;

namespace LiftDataManager.ViewModels;

public partial class DataBaseEditViewModel : DataViewModelBase, INavigationAwareEx
{
    private readonly IVaultDataService _vaultDataService;
    private readonly ParameterContext _parametercontext;
    private readonly ParameterEditContext _parameterEditContext;
    private readonly ILogger<DataBaseEditViewModel> _logger;

    [ObservableProperty]
    public partial List<ParameterDto> ParameterDtos { get; set; }
    public DataBaseEditViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                 ISettingService settingService, ILogger<DataViewModelBase> baseLogger, IVaultDataService vaultDataService,
                                 ILogger<DataBaseEditViewModel> logger, ParameterContext parametercontext, ParameterEditContext parameterEditContext) :
                                 base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _vaultDataService = vaultDataService;
        _logger = logger;
        _parametercontext = parametercontext;
        _parameterEditContext = parameterEditContext;
        ParameterDtos ??= [];
        DatabaseTable ??= [];
    }

    [RelayCommand]
    public async Task DataBaseEditViewModelUnloaded()
    {
        if (_parameterEditContext.Database.GetDbConnection() is SqliteConnection editConn)
        {
            SqliteConnection.ClearPool(editConn);
        }
        _parameterEditContext.Database.CloseConnection();
        var copyResult = await ProcessHelpers.CopyDataBaseToWorkSpace(_parameterEditContext);
        if (copyResult)
        {
            _logger.LogInformation(60177, "Copy database successful to lokal workspace");
        }
        else
        {
            _logger.LogWarning(61075, "Copy database failed to lokal workspace");
        }
        if (_parametercontext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
    }

    public void DataBaseTable_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is TableView tableView)
        {
            DataBaseTableView = tableView;
        }
    }

    [ObservableProperty]
    public partial bool CanEditDatabase { get; set; }

    [ObservableProperty]
    public partial string? DataBasePath { get; set; }

    [ObservableProperty]
    public partial List<string?>? AllTables { get; set; }

    [ObservableProperty]
    public partial List<string?>? FilteredAllTables { get; set; }

    public TableView? DataBaseTableView { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<dynamic> DatabaseTable { get; set; }

    [ObservableProperty]
    public partial List<DatabaseTableValueModification>? TableHistory { get; set; }

    [ObservableProperty]
    public partial string? SelectedTable { get; set; }
    partial void OnSelectedTableChanged(string? value)
    {
        RefreshSelectedTable(value);
    }
    private void RefreshSelectedTable(string? tableName)
    {
        DatabaseTable = [];
        DataBaseTableView?.AutoGenerateColumns = false;
        if (tableName is not null)
        {
            var entityType = TypeFinder.FindLiftmanagerType(tableName[..^1]);

            if (entityType is not null)
            {
                var table = _parameterEditContext.Query(entityType);
                if (table is not null)
                {
                    foreach (var row in table)
                    {
                        DatabaseTable.Add(row);
                    }
                    CanAddTableValue = true;
                    ShowTable = true;
                }
            }
        }
        else
        {
            DatabaseTable = [];
            ShowTable = false;
            CanAddTableValue = false;
        }
        DataBaseTableView?.AutoGenerateColumns = true;
    }

    private readonly string[] skipPropertys = [ "Id", "OrderSelection", "Name", "DisplayName",
                                               "IsFavorite", "IsObsolete" , "SchindlerCertified", "LiftTypes",
                                               "GuideModelTypes" , "DriveSystems" , "LiftDoorGroups", "LiftDoorOpeningDirection",
                                               "TypeExaminationCertificate", "CarDoors" , "ShaftDoors", "OverspeedGovernors" ,
                                               "LiftPositionSystems" , "SafetyGearModelTypes", "CarFloorColorTyps", "CarFrameTypes",
                                               "CarDoorHeaderDepth","CarDoorHeaderHeight", "DriveType", "CarFrameBaseType", "ZipCodes",
                                               "ZiehlAbeggDrives", "DriveSystemType"];
    public void DataBaseTables_AutoGeneratingColumn(object sender, WinUI.TableView.TableViewAutoGeneratingColumnEventArgs e)
    {
        if (e.Column is null || skipPropertys.Contains(e.PropertyName))
        {
            e.Cancel = true;
        }
    }

    [ObservableProperty]
    public partial List<ParameterTyp>? ParameterTyps { get; set; }

    [ObservableProperty]
    public partial ParameterTyp? SelectedParameterTyp { get; set; }
    partial void OnSelectedParameterTypChanged(ParameterTyp? value)
    {
        if (value is not null)
        {
            if (value.Name != "DropDownList")
            {
                SelectedDropdownlistTable = null;
            }
            IsdropdownlistTablesVisible = value.Name == "DropDownList";
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial List<ParameterTypeCode>? ParameterTypeCodes { get; set; }

    [ObservableProperty]
    public partial ParameterTypeCode? SelectedParameterTypeCode { get; set; }
    partial void OnSelectedParameterTypeCodeChanged(ParameterTypeCode? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial List<ParameterCategory>? ParameterCategorys { get; set; }

    [ObservableProperty]
    public partial ParameterCategory? SelectedParameterCategory { get; set; }
    partial void OnSelectedParameterCategoryChanged(ParameterCategory? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial string? UniqueName { get; set; }
    partial void OnUniqueNameChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial string? DisplayName { get; set; }
    partial void OnDisplayNameChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial string? ParameterValue { get; set; }

    [ObservableProperty]
    public partial string? Comment { get; set; }

    [ObservableProperty]
    public partial bool? IsKey { get; set; }
    partial void OnIsKeyChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial bool? IsCarDesignRelated { get; set; }
    partial void OnIsCarDesignRelatedChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial bool? IsDispoPlanRelated { get; set; }
    partial void OnIsDispoPlanRelatedChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial bool? IsLiftPanelRelated { get; set; }
    partial void OnIsLiftPanelRelatedChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial bool? IsDefaultUserEditable { get; set; }
    partial void OnIsDefaultUserEditableChanged(bool? value)
    {
        if (value is not null)
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial List<string>? DropdownlistTables { get; set; }

    [ObservableProperty]
    public partial bool IsdropdownlistTablesVisible { get; set; }

    [ObservableProperty]
    public partial string? SelectedDropdownlistTable { get; set; }
    partial void OnSelectedDropdownlistTableChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            CheckCanAddParameter();
        }
    }

    [ObservableProperty]
    public partial string? RemoveParameterId { get; set; }
    partial void OnRemoveParameterIdChanged(string? value)
    {
        CanRemoveParameter = !string.IsNullOrWhiteSpace(value) && value != "0";
    }

    [ObservableProperty]
    public partial bool ShowTable { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveParameterFromDataBaseCommand))]
    public partial bool CanRemoveParameter { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddParameterToDataBaseCommand))]
    public partial bool CanAddParameter { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddValueToTableCommand))]
    public partial bool CanAddTableValue { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeParametersDataCommand))]
    public partial bool CanChangeParameters { get; set; }

    private void CheckCanAddParameter()
    {
        CanAddParameter = SelectedParameterTyp is not null &&
                          ParameterTypeCodes is not null &&
                          SelectedParameterCategory is not null &&
                          !string.IsNullOrWhiteSpace(UniqueName) &&
                          !string.IsNullOrWhiteSpace(DisplayName) &&
                          IsKey is not null &&
                          IsDefaultUserEditable is not null &&
                          IsCarDesignRelated is not null &&
                          IsDispoPlanRelated is not null &&
                          IsLiftPanelRelated is not null &&
                          (SelectedParameterTyp.Name != "DropDownList" || !string.IsNullOrWhiteSpace(SelectedDropdownlistTable));
    }

    [ObservableProperty]
    public partial bool ShowParameterDeleteMessage { get; set; }

    [ObservableProperty]
    public partial string? ParameterDeleteMessage { get; set; }

    [ObservableProperty]
    public partial string? SearchTableInput { get; set; }
    partial void OnSearchTableInputChanged(string? value)
    {
        FilterTable();
    }

    [RelayCommand]
    public async Task CheckParameterChangedAsync()
    {
        CanChangeParameters = _parameterEditContext.ChangeTracker.HasChanges();
        await Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveParameter))]
    private void RemoveParameterFromDataBase()
    {
        var id = Convert.ToInt32(RemoveParameterId);
        var deletableParameterDto = _parameterEditContext.Find<ParameterDto>(id);

        if (deletableParameterDto is not null)
        {
            try
            {
                _parameterEditContext.Remove(deletableParameterDto);
                _parameterEditContext.SaveChanges();
                RefreshDataBaseAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
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
        if (deletableRow is not null)
        {
            var type = deletableRow.GetType();
            var tableName = type.Name + "s";
            var idEntity = ((BaseEntity)deletableRow).Id;
            var nameEntity = ((BaseEntity)deletableRow).Name;

            try
            {
                _parameterEditContext.Remove(deletableRow);
                _parameterEditContext.SaveChanges();
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
        try
        {
            var newParameterDto = new ParameterDto
            {
                ParameterTyp = SelectedParameterTyp!,
                ParameterTypeCode = SelectedParameterTypeCode!,
                ParameterCategory = SelectedParameterCategory!,
                Name = "var_" + UniqueName,
                DisplayName = DisplayName!,
                Value = ParameterValue,
                Comment = Comment,
                IsKey = (bool)IsKey!,
                DefaultUserEditable = (bool)IsDefaultUserEditable!,
                CarDesignRelated = (bool)IsCarDesignRelated!,
                DispoPlanRelated = (bool)IsDispoPlanRelated!,
                LiftPanelRelated = (bool)IsCarDesignRelated!,
                DropdownList = SelectedDropdownlistTable
            };
            var addedParameterDto = _parameterEditContext.Add(newParameterDto);
            _parameterEditContext.SaveChanges();
            var newEntity = _parameterEditContext.Entry(addedParameterDto.Entity);
            SetDatabaseTableValueModification("add", "ParameterDtos", newEntity.Entity.Id, newEntity.Entity.DisplayName!, newEntity.DebugView.LongView);
            _logger.LogInformation(60174, "parameter: {newEntity} successfully add to database", newEntity.DebugView.LongView);
            RefreshDataBaseAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
            SelectedParameterTyp = null;
            SelectedParameterTypeCode = null;
            SelectedParameterCategory = null;
            UniqueName = null;
            DisplayName = null;
            ParameterValue = null;
            Comment = null;
            IsKey = null;
            IsDefaultUserEditable = null;
            IsCarDesignRelated = null;
            IsDispoPlanRelated = null;
            IsLiftPanelRelated = null;
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
        _parameterEditContext.Entry(entity).State = EntityState.Detached;
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
                    _parameterEditContext.Add(newEntity);
                    _parameterEditContext.SaveChanges();
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
        if (_parameterEditContext.ChangeTracker.HasChanges())
        {
            try
            {
                var changedEntities = _parameterEditContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
                foreach (var entity in changedEntities)
                {
                    var type = entity.Entity.GetType();
                    var tableName = type.Name + "s";
                    var idEntity = ((BaseEntity)entity.Entity).Id;
                    var nameEntity = ((BaseEntity)entity.Entity).Name;
                    var newEntityValue = entity.DebugView.LongView;

                    SetDatabaseTableValueModification("modify", tableName, idEntity, nameEntity, newEntityValue);
                    _logger.LogInformation(60176, "ChangeParameter: {newEntityValue}", newEntityValue);
                }
                _parameterEditContext.SaveChanges();
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
        _parameterEditContext.Add(newValueModification);
        _parameterEditContext.SaveChanges();
    }

    private void FilterTable()
    {
        if (AllTables is not null)
        {
            if (SearchTableInput is not null)
            {
                FilteredAllTables = [.. AllTables.Where(x => x is not null && x.Contains(SearchTableInput, StringComparison.CurrentCultureIgnoreCase))];
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
        ParameterDtos.Clear();
        ParameterDtos = [.. _parameterEditContext.ParameterDtos!
                                     .Include(x => x.ParameterTyp)
                                     .Include(x => x.ParameterTypeCode)
                                     .Include(x => x.ParameterCategory)];
        await Task.CompletedTask;
    }

    [RelayCommand]
    public void LoadDatabaseTableValueModificationHistory()
    {
        TableHistory = [.. _parameterEditContext.Set<DatabaseTableValueModification>().OrderByDescending(x => x.Id)];
    }

    private void GetDropdownValues()
    {
        ParameterTyps = [.. _parameterEditContext.Set<ParameterTyp>()];
        ParameterTypeCodes = [.. _parameterEditContext.Set<ParameterTypeCode>()];
        ParameterCategorys = [.. _parameterEditContext.Set<ParameterCategory>()];
        DropdownlistTables = [.. _parameterEditContext.DropdownValues!.Select(x => x.Base).Distinct()];
        var allTablesfromDB = _parameterEditContext.Model.GetEntityTypes().Select(t => t.GetTableName())
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

        AllTables = [.. allTablesfromDB.Except(ignoredTables)];


        FilteredAllTables = AllTables;
    }

    [RelayCommand]
    public async Task UpdateAutotransferXmlAsync()
    {
        var nameXml = "AutoDeskTransfer.xml";
        var downloadResult = await _vaultDataService.GetFileAsync(nameXml, false, true);

        if (downloadResult.ExitCode == 0 && downloadResult.CheckOutState == "CheckedOutByCurrentUser")
        {
            var updateResult = await _parameterDataService.UpdateAutodeskTransferAsync(downloadResult.FullFileName!, ParameterDtos);

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

    public async void OnNavigatedTo(object parameter)
    {
        Adminmode = _settingService.Adminmode;
        DataBasePath = _settingService.PathDataBase;
        var vaultDisabled = _settingService.VaultDisabled;
        CanEditDatabase = Adminmode & !vaultDisabled;
        if (Adminmode)
        {
            GetDropdownValues();
            await RefreshDataBaseAsync();
        }
    }

    public void OnNavigatedFrom()
    {
    }
}