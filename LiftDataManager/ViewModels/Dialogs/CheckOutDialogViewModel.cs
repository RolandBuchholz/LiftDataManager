using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class CheckOutDialogViewModel : ObservableObject
{
    private readonly ILogger<CheckOutDialogViewModel> _logger;
    public CheckOutDialogResult CheckOutDialogResult { get; set; }
    public string? SpezifikationName { get; set; }
    public bool ForceCheckOut { get; set; }
    public CheckOutDialogViewModel(ILogger<CheckOutDialogViewModel> logger)
    {
        _logger = logger;
    }

    [ObservableProperty]
    private bool showReadOnlyWarning = true;

    [RelayCommand]
    public async Task VaultCheckOutDialogLoadedAsync(CheckOutDialog sender)
    {
        SpezifikationName = sender.SpezifikationName;
        if (string.IsNullOrWhiteSpace(SpezifikationName))
        {
            CheckOutDialogResult = CheckOutDialogResult.CheckOutFailed;
            sender.Hide();
            return;
        }
        ForceCheckOut = sender.ForceCheckOut;

        if (ForceCheckOut)
        {
            ShowReadOnlyWarning = false;
            sender.CloseButtonText = string.Empty;
        }
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task PrimaryButtonClickedAsync(object sender)
    {
        await Task.Delay(3000);
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task SecondaryButtonClickedAsync(object sender)
    {
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task CloseButtonClickedAsync(object sender)
    {
        await Task.CompletedTask;
    }

}