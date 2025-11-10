using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.Models;

public partial class ObservableDBSafetyComponentRecord : ObservableObject
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    private bool _initializeData;
    public bool SkipDataBaseUpdate { get; set; }

    public ObservableDBSafetyComponentRecord(SafetyComponentRecord safetyComponentRecord, SafetyComponentRecordContext safetyComponentRecordContext)
    {
        _safetyComponentRecordContext = safetyComponentRecordContext;
        CurrentSafetyComponentRecord = safetyComponentRecord;
        _initializeData = true;
        Id = safetyComponentRecord.Id;
        Name = safetyComponentRecord.Name;
        LiftCommissionId = safetyComponentRecord.LiftCommissionId;
        LiftCommission = safetyComponentRecord.LiftCommission;
        CompleteRecord = safetyComponentRecord.CompleteRecord;
        SchindlerCertified = safetyComponentRecord.SchindlerCertified;
        Release = safetyComponentRecord.Release;
        Revision = safetyComponentRecord.Revision;
        IdentificationNumber = safetyComponentRecord.IdentificationNumber;
        SerialNumber = safetyComponentRecord.SerialNumber;
        BatchNumber = safetyComponentRecord.BatchNumber;
        Imported = safetyComponentRecord.Imported;
        CreationDate = safetyComponentRecord.CreationDate;
        Active = safetyComponentRecord.Active;
        SafetyComponentManufacturerId = safetyComponentRecord.SafetyComponentManufacturerId;
        SafetyComponentManufacturer = safetyComponentRecord.SafetyComponentManufacturer;
        _initializeData = false;
        CompleteRecord = CheckRecordisCompleted();
    }

    public SafetyComponentRecord GetSafetyComponentDB() 
    {
        return CurrentSafetyComponentRecord;
    }

    public int GetSafetyComponentDBId()
    {
        return Id;
    }

    public SafetyComponentRecord CurrentSafetyComponentRecord { get; set; }
    
    public int Id { get; set; }
    
    [ObservableProperty]
    public partial string? Name { get; set; }
    partial void OnNameChanged(string? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        CompleteRecord = CheckRecordisCompleted();
        UpdateSafetyComponentRecordDatabase(nameof(Name));
    }

    public int LiftCommissionId { get; set; }

    public LiftCommission? LiftCommission { get; set; }

    [ObservableProperty]
    public partial bool SchindlerCertified { get; set; }

    [ObservableProperty]
    public partial bool CompleteRecord { get; set; }
    partial void OnCompleteRecordChanged(bool value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        UpdateSafetyComponentRecordDatabase(nameof(CompleteRecord));
    }

    [ObservableProperty]
    public partial int Release { get; set; }
    partial void OnReleaseChanged(int value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        UpdateSafetyComponentRecordDatabase(nameof(Release));
    }

    [ObservableProperty]
    public partial int Revision { get; set; }
    partial void OnRevisionChanged(int value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        UpdateSafetyComponentRecordDatabase(nameof(Revision));
    }

    [ObservableProperty]
    public partial string? IdentificationNumber { get; set; }
    partial void OnIdentificationNumberChanged(string? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        CompleteRecord = CheckRecordisCompleted();
        UpdateSafetyComponentRecordDatabase(nameof(IdentificationNumber));
    }

    [ObservableProperty]
    public partial string? SerialNumber { get; set; }
    partial void OnSerialNumberChanged(string? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        CompleteRecord = CheckRecordisCompleted();
        UpdateSafetyComponentRecordDatabase(nameof(SerialNumber));
    }

    [ObservableProperty]
    public partial string? BatchNumber { get; set; }
    partial void OnBatchNumberChanged(string? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        CompleteRecord = CheckRecordisCompleted();
        UpdateSafetyComponentRecordDatabase(nameof(BatchNumber));
    }

    [ObservableProperty]
    public partial string? Imported { get; set; }
    partial void OnImportedChanged(string? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        UpdateSafetyComponentRecordDatabase(nameof(Imported));
    }

    [ObservableProperty]
    public partial DateTime? CreationDate { get; set; }
    partial void OnCreationDateChanged(DateTime? value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        CompleteRecord = CheckRecordisCompleted();
        UpdateSafetyComponentRecordDatabase(nameof(CreationDate));
    }

    [ObservableProperty]
    public partial bool Active { get; set; }
    partial void OnActiveChanged(bool value)
    {
        if (_initializeData || SkipDataBaseUpdate)
        {
            return;
        }
        UpdateSafetyComponentRecordDatabase(nameof(Active));
    }

    [ObservableProperty]
    public partial int SafetyComponentManufacturerId { get; set; }

    [ObservableProperty]
    public partial SafetyComponentManufacturer? SafetyComponentManufacturer { get; set; }

    public bool CheckRecordisCompleted()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(IdentificationNumber) &&
               SafetyComponentManufacturer is not null &&
               CreationDate is not null &&
               (!string.IsNullOrWhiteSpace(SerialNumber) || !string.IsNullOrWhiteSpace(BatchNumber));
    }

    private void UpdateSafetyComponentRecordDatabase(string membername)
    {
        switch (membername)
        {
            case nameof(Active):
                CurrentSafetyComponentRecord.Active = Active;
                break;
            case nameof(CompleteRecord):
                CurrentSafetyComponentRecord.CompleteRecord = CompleteRecord;
                break;
            case nameof(Name):
                CurrentSafetyComponentRecord.Name = Name is null ? string.Empty : Name;
                break;
            case nameof(Release):
                CurrentSafetyComponentRecord.Release = Release;
                break;
            case nameof(Revision):
                CurrentSafetyComponentRecord.Revision = Revision;
                break;
            case nameof(IdentificationNumber):
                CurrentSafetyComponentRecord.IdentificationNumber = IdentificationNumber;
                break;
            case nameof(SerialNumber):
                CurrentSafetyComponentRecord.SerialNumber = SerialNumber;
                break;
            case nameof(BatchNumber):
                CurrentSafetyComponentRecord.BatchNumber = BatchNumber;
                break;
            //Todo Hersteller
            case nameof(Imported):
                CurrentSafetyComponentRecord.Imported = Imported;
                break;
            case nameof(CreationDate):
                CurrentSafetyComponentRecord.CreationDate = CreationDate;
                break;
            default:
                break;
        }

        _safetyComponentRecordContext.Update(CurrentSafetyComponentRecord);
        _safetyComponentRecordContext.SaveChanges();
    }
}
