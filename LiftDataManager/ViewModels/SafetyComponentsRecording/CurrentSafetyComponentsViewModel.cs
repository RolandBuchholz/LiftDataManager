using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class CurrentSafetyComponentsViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    private readonly IPdfService _pdfService;
    private readonly ILogger<CurrentSafetyComponentsViewModel> _logger;
    public ObservableCollection<ObservableDBSafetyComponentRecord> ListOfSafetyComponents { get; set; } = [];

    public CurrentSafetyComponentsViewModel(IParameterDataService parameterDataService, SafetyComponentRecordContext safetyComponentRecordContext, IDialogService dialogService, IInfoCenterService infoCenterService, IPdfService pdfService, ILogger<CurrentSafetyComponentsViewModel> logger,
                              ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _safetyComponentRecordContext = safetyComponentRecordContext;
        _pdfService = pdfService;
        _logger = logger;
    }

    [ObservableProperty]
    public partial PivotItem? SelectedPivotItem { get; set; }
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {
                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    [ObservableProperty]
    public partial LiftCommission? CurrentLiftCommission { get; set; }
    [ObservableProperty]
    public partial string? LiftName { get; set; }
    partial void OnLiftNameChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.Name, value))
        {
            CurrentLiftCommission?.Name = value is null ? string.Empty : value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial int LiftInstallerID { get; set; }
    partial void OnLiftInstallerIDChanged(int value)
    {
        if (!Equals(CurrentLiftCommission?.LiftInstallerID, value))
        {
            CurrentLiftCommission?.LiftInstallerID = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial string? SAISEquipment { get; set; }
    partial void OnSAISEquipmentChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.SAISEquipment, value))
        {
            CurrentLiftCommission?.SAISEquipment = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial string? Street { get; set; }
    partial void OnStreetChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.Street, value))
        {
            CurrentLiftCommission?.Street = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial string? HouseNumber { get; set; }
    partial void OnHouseNumberChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.HouseNumber, value))
        {
            CurrentLiftCommission?.HouseNumber = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial int ZIPCode { get; set; }
    partial void OnZIPCodeChanged(int value)
    {
        if (!Equals(CurrentLiftCommission?.ZIPCode, value))
        {
            CurrentLiftCommission?.ZIPCode = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial string? City { get; set; }
    partial void OnCityChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.City, value))
        {
            CurrentLiftCommission?.City = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }
    [ObservableProperty]
    public partial string? Country { get; set; }
    partial void OnCountryChanged(string? value)
    {
        if (!string.Equals(CurrentLiftCommission?.Country, value))
        {
            CurrentLiftCommission?.Country = value;
            _safetyComponentRecordContext.SaveChanges();
        }
    }

    private async Task GetCurrentSafetyComponentsFromDatabaseAsync()
    {
        if (string.IsNullOrWhiteSpace(SpezifikationsNumber))
        {
            return;
        }

        CurrentLiftCommission = _safetyComponentRecordContext.LiftCommissions?.Where(x => x.Name == SpezifikationsNumber)
                                                                      .Include(i => i.SafetyComponentRecords!)
                                                                      .ThenInclude(t => t.SafetyComponentManfacturer)
                                                                      .FirstOrDefault();

        var saisNumber = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_SAISEquipment");
        if (CurrentLiftCommission is null)
        {
            CurrentLiftCommission = new LiftCommission()
            {
                Name = SpezifikationsNumber,
                LiftInstallerID = 1491,
                SAISEquipment = saisNumber,
                Country = "DE",
                SafetyComponentRecords = []
            };
            await _safetyComponentRecordContext.LiftCommissions!.AddAsync(CurrentLiftCommission);
            await _safetyComponentRecordContext.SaveChangesAsync();
        }

        if (string.IsNullOrWhiteSpace(saisNumber) &&
            !string.IsNullOrWhiteSpace(CurrentLiftCommission.SAISEquipment))
        {
            ParameterDictionary["var_SAISEquipment"].Value = CurrentLiftCommission.SAISEquipment;
        }

        LiftName = CurrentLiftCommission.Name;
        LiftInstallerID = CurrentLiftCommission.LiftInstallerID;
        SAISEquipment = CurrentLiftCommission.SAISEquipment;
        Street = CurrentLiftCommission.Street;
        HouseNumber = CurrentLiftCommission.HouseNumber;
        ZIPCode = CurrentLiftCommission.ZIPCode;
        City = CurrentLiftCommission.City;
        Country = CurrentLiftCommission.Country;
        if (CurrentLiftCommission is not null && CurrentLiftCommission.SafetyComponentRecords is not null)
        {
            foreach (var record in CurrentLiftCommission.SafetyComponentRecords)
            {
                ListOfSafetyComponents.Add(new ObservableDBSafetyComponentRecord(record, _safetyComponentRecordContext));
            }
        }

        await Task.CompletedTask;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportSafetyComponentRecordCommand))]
    public partial bool CanImportSafetyComponentRecords { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteAllSafetyComponentRecordsCommand))]
    public partial bool CanDeleteAllSafetyComponentRecords { get; set; }

    [RelayCommand(CanExecute = nameof(CanImportSafetyComponentRecords))]
    private async Task ImportSafetyComponentRecordAsync()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AddSafetyComponentRecordAsync()
    {
        var safetyComponentRecord = new SafetyComponentRecord()
        {
            Name = "New",
            IdentificationNumber = "New",
            Imported = "- - -",
            CreationDate = DateTime.Now,
            Active = true,
            BatchNumber = string.Empty,
            CompleteRecord = false,
            LiftCommissionId = 0,
            SerialNumber = string.Empty,
            Release = 0,
            Revision = 0,
            SafetyComponentManfacturerId = 0,
        };

        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task DeleteSafetyComponentRecordAsync(object sender)
    {
        var result = await _dialogService.WarningDialogAsync("Sicherheitskomponete löschen",
            """
            Achtung:
            Sicherheitskomponete wird endgültig gelöscht.

            Möchten Sie den Vorgang wirklich fortsetzen?
            """,
            "Löschen",
            "Abbrechen");
        if (result is not true)
        {
            return;
        }

        if (sender is ObservableDBSafetyComponentRecord safetyComponentRecord)
        {
            _safetyComponentRecordContext.Remove(safetyComponentRecord.GetSafetyComponentDB());
            await _safetyComponentRecordContext.SaveChangesAsync();
            ListOfSafetyComponents.Remove(safetyComponentRecord);
            await CheckCanExecuteCommandsAsync();
        }
        await Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanDeleteAllSafetyComponentRecords))]
    private async Task DeleteAllSafetyComponentRecordsAsync()
    {
        var result = await _dialogService.WarningDialogAsync("Alle Sicherheitskomponeten löschen",
        """
            Achtung:
            Alle Sicherheitskomponeten werden endgültig gelöscht.

            Möchten Sie den Vorgang wirklich fortsetzen?
        """,
        "Löschen",
        "Abbrechen");
        if (result is not true)
        {
            return;
        }
        var elemetsToDelete = new List<SafetyComponentRecord>();
        foreach (var safetyComponent in ListOfSafetyComponents)
        {
            elemetsToDelete.Add(safetyComponent.GetSafetyComponentDB());
        }
        _safetyComponentRecordContext.RemoveRange(elemetsToDelete);
        await _safetyComponentRecordContext.SaveChangesAsync();
        ListOfSafetyComponents.Clear();
        await CheckCanExecuteCommandsAsync();
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CreateSafetyComponentRecordPDFAsync()
    {
        _pdfService.GenerateSafetyComponentsReport(ParameterDictionary, CurrentLiftCommission);
        await Task.CompletedTask;
    }

    private async Task CheckCanExecuteCommandsAsync()
    {
        CanImportSafetyComponentRecords = ListOfSafetyComponents.Count == 0;
        CanDeleteAllSafetyComponentRecords = ListOfSafetyComponents.Count > 0;
        await Task.CompletedTask;
    }

    public async void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        await GetCurrentSafetyComponentsFromDatabaseAsync();
        await CheckCanExecuteCommandsAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}