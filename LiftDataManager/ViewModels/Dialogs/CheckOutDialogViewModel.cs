using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class CheckOutDialogViewModel : ObservableObject
{
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<CheckOutDialogViewModel> _logger;

    public CheckOutDialogResult CheckOutDialogResult { get; set; }
    public string? SpezifikationName { get; set; }
    public bool ForceCheckOut { get; set; }
    public CheckOutDialogViewModel(IVaultDataService vaultDataService, ILogger<CheckOutDialogViewModel> logger)
    {
        _vaultDataService = vaultDataService;
        _logger = logger;
    }

    [ObservableProperty]
    private bool showReadOnlyWarning = true;

    [ObservableProperty]
    private bool checkOutInprogress;

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
        sender.Closed += VaultCheckOutDialogClosed;
        await Task.CompletedTask;
    }

    private void VaultCheckOutDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender is CheckOutDialog checkOutDialog)
        {
            checkOutDialog.CheckOutDialogResult = CheckOutDialogResult;
        }
        CheckOutInprogress = false;
    }

    [RelayCommand]
    public async Task PrimaryButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        CheckOutInprogress = true;
        var result = await CheckOut();
        CheckOutDialogResult = result ? CheckOutDialogResult.SuccessfulIncreaseRevision : CheckOutDialogResult.CheckOutFailed;
        deferral.Complete();
    }
    [RelayCommand]
    public async Task SecondaryButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        CheckOutInprogress = true;
        var result = await CheckOut();
        CheckOutDialogResult = result ? CheckOutDialogResult.SuccessfulNoRevisionChange : CheckOutDialogResult.CheckOutFailed;
        deferral.Complete();
    }
    [RelayCommand]
    public async Task CloseButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        CheckOutDialogResult = CheckOutDialogResult.ReadOnly;
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task DialogClosedAsync(CheckOutDialog sender)
    {
        sender.CheckOutDialogResult = CheckOutDialogResult;
        await Task.CompletedTask;
    }
    private async Task<bool> CheckOut()
    {
        await Task.Delay(50);
        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!);
        if (downloadResult is not null)
        {
            _logger.LogInformation(60139, "downloadResult: {downloadResult.CheckOutState}", downloadResult.CheckOutState);
            return downloadResult.IsCheckOut;
        }
        _logger.LogWarning(60139, "CheckOut failed");
        return false;
    }
}