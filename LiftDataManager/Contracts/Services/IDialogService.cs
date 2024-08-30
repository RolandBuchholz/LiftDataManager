namespace LiftDataManager.Contracts.Services;

public interface IDialogService
{
    Task MessageDialogAsync(string title, string message);

    Task MessageDialogAsync(string title, string message, string buttonText);

    Task<bool?> MessageConfirmationDialogAsync(string title, string message, string buttonText);

    Task<bool?> ConfirmationDialogAsync(string title);

    Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText);

    Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText, string cancelButtonText);

    Task<bool?> ConfirmationDialogAsync(string title, string message, string yesButtonText, string noButtonText, string cancelButtonText);

    Task<bool?> WarningDialogAsync(string title, string message, string yesButtonText, string noButtonText);

    Task<string?> InputDialogAsync(string title, string message, string textBoxName);

    Task LiftDataManagerdownloadInfoAsync(DownloadInfo downloadResult);

    Task<string?> LiftPlannerDBDialogAsync(string liftPlanner);

    Task<bool> PasswordDialogAsync(string? title, string? condition, string? description);

    Task<bool> ZALiftDialogAsync(string? fullPathXml);

    Task<bool> CFPEditDialogAsync(string? fullPathXml, string? carFrameTyp);

    Task ParameterChangedDialogAsync(List<InfoCenterEntry> parameterChangedList);

    Task<(ContentDialogResult, bool)> AppClosingDialogAsync(bool ignoreSaveWarning);
}
