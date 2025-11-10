using LiftDataManager.Core.DataAccessLayer.Models;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class GenerateSafetyComponentRecordDialogViewModel : ObservableObject
{
    private readonly ParameterContext _parametercontext;
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    public int LiftCommissionId { get; set; }
    public List<SafetyComponentTyp> SafetyComponentTyps { get; set; }
    public ObservableCollection<SafetyComponentEntity> SafetyComponents { get; set; }

    private readonly ILogger<GenerateSafetyComponentRecordDialogViewModel> _logger;

    public GenerateSafetyComponentRecordDialogViewModel(ParameterContext parametercontext, SafetyComponentRecordContext safetyComponentRecordContext, ILogger<GenerateSafetyComponentRecordDialogViewModel> logger)
    {
        _parametercontext = parametercontext;
        _safetyComponentRecordContext = safetyComponentRecordContext;
        SafetyComponentTyps = [.. _parametercontext.Set<SafetyComponentTyp>()];
        SafetyComponents = [];
        SelectedSafetyComponentTyp = SafetyComponentTyps.FirstOrDefault();
        _logger = logger;
    }

    [RelayCommand]
    public async Task GenerateSafetyComponentRecordDialogLoadedAsync(GenerateSafetyComponentRecordDialog sender)
    {
        LiftCommissionId = sender.LiftCommissionId;
        if (_safetyComponentRecordContext.LiftCommissions is null)
        {
            return;
        }
        var currentLiftCommission = await _safetyComponentRecordContext.LiftCommissions.FindAsync(LiftCommissionId);
        if (currentLiftCommission is null)
        {
            return;
        }
        var newSafetyComponentRecord = new SafetyComponentRecord()
        {
            Name = string.Empty,
            CreationDate = DateTime.Now,
            IdentificationNumber = string.Empty,
            Imported = "- - -",
            Release = 0,
            Revision = 0,
            BatchNumber = string.Empty,
            SerialNumber = string.Empty,
            SafetyComponentManufacturerId = 1,
            LiftCommissionId = LiftCommissionId,
            LiftCommission = currentLiftCommission,
            Active = true,
            CompleteRecord = false,
        };
        NewObservableDBSafetyComponentRecord = new ObservableDBSafetyComponentRecord(newSafetyComponentRecord, _safetyComponentRecordContext)
        {
            SkipDataBaseUpdate = true
        };
        await Task.CompletedTask;
    }

    [ObservableProperty]
    public partial ObservableDBSafetyComponentRecord? NewObservableDBSafetyComponentRecord { get; set; }

    [ObservableProperty]
    public partial SafetyComponentTyp? SelectedSafetyComponentTyp { get; set; }
    partial void OnSelectedSafetyComponentTypChanged(SafetyComponentTyp? value)
    {
        SafetyComponents.Clear();
        if (value is null)
        {
            SafetyComponentSelectionVisibility = false;
            StepBarIndex = 0;
            return;
        }
        SafetyComponentSelectionVisibility = value.Id != 1;
        StepBarIndex = value.Id != 1 ? 1 : 0;

        IEnumerable<SafetyComponentEntity> safetyComponents = value.Id switch
        {
            1 => [],
            2 => _parametercontext.Set<SafetyGearModelType>().Where(x => x.SafetyGearTypeId == 1),
            3 => _parametercontext.Set<SafetyGearModelType>().Where(x => x.SafetyGearTypeId == 2),
            4 => _parametercontext.Set<LiftBuffer>().Where(x => x.Manufacturer == "P+S"),
            5 => _parametercontext.Set<LiftBuffer>().Where(x => x.Manufacturer == "OLEO"),
            6 => _parametercontext.Set<OverspeedGovernor>().AsEnumerable(),
            7 => _parametercontext.Set<CarDoor>().AsEnumerable(),
            8 => _parametercontext.Set<ShaftDoor>().AsEnumerable(),
            9 => [],
            10 => [],
            11 => [],
            12 => _parametercontext.Set<SafetyGearModelType>().Where(x => x.SafetyGearTypeId == 3),
            13 => _parametercontext.Set<HydraulicValve>().AsEnumerable(),
            14 => _parametercontext.Set<DriveSafetyBrake>().AsEnumerable(),
            15 => _parametercontext.Set<OverspeedGovernor>().AsEnumerable(),
            16 => [],
            17 => [],
            18 => [],
            19 => [],
            20 => _parametercontext.Set<LiftPositionSystem>().Where(x => x.TypeExaminationCertificateId != 1),
            21 => [],
            22 => _parametercontext.Set<LiftDoorTelescopicApron>().AsEnumerable(),
            _ => [],
        };
        SafetyComponents.AddRange(safetyComponents);
    }

    [ObservableProperty]
    public partial bool SafetyComponentSelectionVisibility { get; set; }

    [ObservableProperty]
    public partial SafetyComponentEntity? SelectedSafetyComponent { get; set; }
    partial void OnSelectedSafetyComponentChanged(SafetyComponentEntity? value)
    {
        if (value is null)
        {
            StepBarStatus = StepStatus.Info;
            StepBarIndex = 1;
            SafetyComponentGridVisibility = false;
        }
        else
        {
            StepBarStatus = StepStatus.Warning;
            StepBarIndex = 2;
            SafetyComponentGridVisibility = true;
            SetDefaultSafetyComponentData();
        }
    }

    [ObservableProperty]
    public partial bool SafetyComponentGridVisibility { get; set; }

    [ObservableProperty]
    public partial int StepBarIndex { get; set; }

    [ObservableProperty]
    public partial StepStatus StepBarStatus { get; set; }


    [RelayCommand]
    public async Task PrimaryButtonClicked(GenerateSafetyComponentRecordDialog sender)
    {
        if (NewObservableDBSafetyComponentRecord is not null)
        {
            NewObservableDBSafetyComponentRecord.SkipDataBaseUpdate = false;
            //await _safetyComponentRecordContext.AddAsync(NewObservableDBSafetyComponentRecord);
            //await _safetyComponentRecordContext.SaveChangesAsync();
            sender.SafetyComponentRecord = NewObservableDBSafetyComponentRecord;
        }
        await Task.CompletedTask;
    }

    private void SetDefaultSafetyComponentData()
    {
        if (SelectedSafetyComponent is not null &&
            NewObservableDBSafetyComponentRecord is not null)
        {
            NewObservableDBSafetyComponentRecord.Name = SelectedSafetyComponent.SAISDescription;
            NewObservableDBSafetyComponentRecord.IdentificationNumber = SelectedSafetyComponent.SAISIdentificationNumber;
            NewObservableDBSafetyComponentRecord.SchindlerCertified = SelectedSafetyComponent.SchindlerCertified;
            NewObservableDBSafetyComponentRecord.SerialNumber = string.Empty;
            NewObservableDBSafetyComponentRecord.BatchNumber = string.Empty;
            //TODO Add Manfacturer
            //NewObservableDBSafetyComponentRecord.SafetyComponentManfacturerId = 1;
            //NewObservableDBSafetyComponentRecord.SafetyComponentManfacturer = ;
        }
    }
}