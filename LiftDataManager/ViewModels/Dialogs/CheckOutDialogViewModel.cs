using LiftDataManager.Core.Services;
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
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task PrimaryButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        await CheckOut(true);
        deferral.Complete();
    }
    [RelayCommand]
    public async Task SecondaryButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        await CheckOut(false);
        deferral.Complete();
    }
    [RelayCommand]
    public async Task CloseButtonClickedAsync(ContentDialogButtonClickEventArgs args)
    {
        await Task.CompletedTask;
    }

    private async Task CheckOut(bool increaseRevision)
    {
        await Task.Delay(50);
        var downloadResult = await _vaultDataService.GetFileAsync(SpezifikationName!);
        if (downloadResult is not null)
        {
            Debug.WriteLine(downloadResult.IsCheckOut);
        }
        await Task.Delay(1000);
    }
}