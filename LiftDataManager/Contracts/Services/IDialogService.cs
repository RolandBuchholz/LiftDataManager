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

    Task LiftDataManagerdownloadInfoAsync(DownloadInfo downloadResult);
}
