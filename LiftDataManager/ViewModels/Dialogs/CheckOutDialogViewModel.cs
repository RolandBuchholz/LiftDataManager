using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class CheckOutDialogViewModel : ObservableObject
{
    private readonly IVaultDataService _vaultDataService;
    private readonly IParameterDataService _parameterDataService;
    private readonly IInfoCenterService _infoCenterService;
    private readonly ILogger<CheckOutDialogViewModel> _logger;

    public CheckOutDialogResult CheckOutDialogResult { get; set; }
    public string? SpezifikationName { get; set; }
    public bool ForceCheckOut { get; set; }
    public CheckOutDialogViewModel(IVaultDataService vaultDataService, IParameterDataService parameterDataService, IInfoCenterService infoCenterService, ILogger<CheckOutDialogViewModel> logger)
    {
        _vaultDataService = vaultDataService;
        _parameterDataService = parameterDataService;
        _infoCenterService = infoCenterService;
        _logger = logger;
    }

    [ObservableProperty]
    public partial bool ShowReadOnlyWarning { get; set; } = true;

    [ObservableProperty]
    public partial bool CheckOutInprogress { get; set; }

    [ObservableProperty]
    public partial string? CheckOutInfoMessage { get; set; }

    [ObservableProperty]
    public partial InfoBarSeverity CheckOutInfoSeverity { get; set; }


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
            _logger.LogInformation(60139, "downloadResult: {checkOutState}", downloadResult.CheckOutState);
            switch (downloadResult.CheckOutState)
            {
                case "CheckedOutByCurrentUser":
                    CheckOutInfoSeverity = InfoBarSeverity.Success;
                    CheckOutInfoMessage = $"Datei erfolgreich abgerufen.";
                    await Task.Delay(1000);
                    break;
                case "CheckedOutByOtherUser":
                    CheckOutInfoSeverity = InfoBarSeverity.Warning;
                    CheckOutInfoMessage = $"Datei wurde von {downloadResult.EditedBy} barabeitet.";
                    await Task.Delay(2000);
                    return false;
                case "NotCheckedOut" or "Unknown" or null:
                    CheckOutInfoSeverity = InfoBarSeverity.Error;
                    CheckOutInfoMessage = $"Fehler: {downloadResult.CheckOutState}";
                    await Task.Delay(2000);
                    return false;
            }

            if (ForceCheckOut && !string.IsNullOrWhiteSpace(downloadResult.FullFileName))
            {
                var data = await _parameterDataService.LoadParameterAsync(downloadResult.FullFileName);
                var newInfoCenterEntrys = await _parameterDataService.UpdateParameterDictionary(downloadResult.FullFileName, data, false);
                await _infoCenterService.AddListofInfoCenterEntrysAsync(newInfoCenterEntrys);
                _logger.LogInformation(60136, "Data loaded from {FullPathXml}", downloadResult.FullFileName);
                await _infoCenterService.AddInfoCenterMessageAsync($"Daten aus {downloadResult.FullFileName} geladen");
            }
            return downloadResult.IsCheckOut;
        }
        _logger.LogWarning(60139, "CheckOut failed");
        return false;
    }
}