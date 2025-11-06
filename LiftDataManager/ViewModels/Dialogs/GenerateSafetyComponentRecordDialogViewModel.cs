using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.Models;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class GenerateSafetyComponentRecordDialogViewModel : ObservableObject
{
    private readonly ParameterContext _parametercontext;
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    public int LiftCommissionId { get; set; }
    public List<SafetyComponentTyp> SafetyComponentTyps { get; set; }
    public List<SafetyComponentEntity> SafetyComponents { get; set; }

    private readonly ILogger<GenerateSafetyComponentRecordDialogViewModel> _logger;

    public GenerateSafetyComponentRecordDialogViewModel(ParameterContext parametercontext, SafetyComponentRecordContext safetyComponentRecordContext, ILogger<GenerateSafetyComponentRecordDialogViewModel> logger)
    {
        _parametercontext = parametercontext;
        _safetyComponentRecordContext = safetyComponentRecordContext;
        SafetyComponentTyps = [.. _parametercontext.Set<SafetyComponentTyp>()];
        SafetyComponents = [];
        SafetyComponents = [.. _parametercontext.Set<OverspeedGovernor>()];
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
    public partial SafetyComponentTyp? SelectedSafetyComponentTyp {  get; set; }
    partial void OnSelectedSafetyComponentTypChanged(SafetyComponentTyp? value) 
    {
        SafetyComponentSelectionVisibility = value?.Id != 1;
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