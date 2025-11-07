using LiftDataManager.Core.DataAccessLayer.Models;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
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
        await Task.CompletedTask;
    }

    [ObservableProperty]
    public partial SafetyComponentTyp? SelectedSafetyComponentTyp { get; set; }
    partial void OnSelectedSafetyComponentTypChanged(SafetyComponentTyp? value)
    {
        SafetyComponents.Clear();
        if (value is null)
        {
            SafetyComponentSelectionVisibility = false;
            return;
        }
        SafetyComponentSelectionVisibility = value.Id != 1;

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
            16=> [],
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
        SafetyComponentGridVisibility = value is not null;
    }

    [ObservableProperty]
    public partial bool SafetyComponentGridVisibility { get; set; }
}