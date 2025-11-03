namespace LiftDataManager.ViewModels.Dialogs;

public partial class ValidationDialogViewModel : ObservableObject
{
    private readonly ILogger<ValidationDialogViewModel> _logger;
    public ValidationDialogViewModel(ILogger<ValidationDialogViewModel> logger)
    {
        _logger = logger;
    }

    [ObservableProperty]
    public partial int ParameterCount { get; set; }
    [ObservableProperty]
    public partial int ErrorCount { get; set; }
    [ObservableProperty]
    public partial int WarningCount { get; set; }
    [ObservableProperty]
    public partial int InfoCount { get; set; }

    [RelayCommand]
    public async Task ValidationResultDialogLoadedAsync(ValidationDialog sender)
    {
        ParameterCount = sender.ParamterCount;
        var errorsDictionary = sender.ParameterErrorDictionary;
        if (errorsDictionary is not null && errorsDictionary.Count != 0)
        {
            try
            {
                var foundParameterErrors = new List<ParameterStateInfo>();

                foreach (var parameterErrors in errorsDictionary.Values)
                {
                    foreach (var error in parameterErrors)
                    {
                        foundParameterErrors.Add(error);
                    }
                }
                ErrorCount = foundParameterErrors.Count((e) => e.Severity == ErrorLevel.Error);
                WarningCount = foundParameterErrors.Count((e) => e.Severity == ErrorLevel.Warning);
                InfoCount = foundParameterErrors.Count((e) => e.Severity == ErrorLevel.Informational);
            }
            catch
            {
                _logger.LogError(61092, "loading errors failed");
            }
        }
        await Task.CompletedTask;
    }
}