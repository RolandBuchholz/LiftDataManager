using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class CheckOutDialogViewModel : ObservableObject
{
    private readonly ILogger<CheckOutDialogViewModel> _logger;
    public CheckOutDialogViewModel(ILogger<CheckOutDialogViewModel> logger)
    {
        _logger = logger;
    }

    [RelayCommand]
    public async Task CFPEditDialogLoadedAsync(CFPEditDialog sender)
    {
        await Task.CompletedTask;
    }
}