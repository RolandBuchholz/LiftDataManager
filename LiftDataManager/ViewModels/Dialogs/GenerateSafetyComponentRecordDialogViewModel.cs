namespace LiftDataManager.ViewModels.Dialogs;

public partial class GenerateSafetyComponentRecordDialogViewModel : ObservableObject
{
    private readonly ParameterContext _parametercontext;
    public int LiftCommissionId { get; set; }
    private readonly ILogger<GenerateSafetyComponentRecordDialogViewModel> _logger;

    public GenerateSafetyComponentRecordDialogViewModel(ParameterContext parametercontext, ILogger<GenerateSafetyComponentRecordDialogViewModel> logger)
    {
        _parametercontext = parametercontext;
        _logger = logger;
    }

    [RelayCommand]
    public async Task GenerateSafetyComponentRecordDialogLoadedAsync(GenerateSafetyComponentRecordDialog sender)
    {
        LiftCommissionId = sender.LiftCommissionId;
        await Task.CompletedTask;
    }
}