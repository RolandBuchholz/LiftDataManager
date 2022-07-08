namespace LiftDataManager.Contracts.Services;

public interface IDialogService
{
    Task MessageDialogAsync(FrameworkElement element, string title, string message);
    Task MessageDialogAsync(FrameworkElement element, string title, string message, string buttonText);
    Task LiftDataManagerdownloadInfoAsync(FrameworkElement element, DownloadInfo downloadResult);
    Task<bool?> ConfirmationDialogAsync(FrameworkElement element, string title);
    Task<bool> ConfirmationDialogAsync(FrameworkElement element, string title, string yesButtonText, string noButtonText);
    Task<bool?> ConfirmationDialogAsync(FrameworkElement element, string title, string yesButtonText, string noButtonText, string cancelButtonText);
    Task<bool> WarningDialogAsync(FrameworkElement element, string title, string message, string yesButtonText, string noButtonText);
}
