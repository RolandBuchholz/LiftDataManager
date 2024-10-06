using Humanizer;
using Microsoft.UI.Text;

namespace LiftDataManager.Contracts.Services;

public interface IDialogService
{
    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <returns>Task</returns>
    Task MessageDialogAsync(string title, string message);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="buttonText">The button text.</param>
    /// <returns>Task</returns>
    Task MessageDialogAsync(string title, string message, string buttonText);

    /// <summary>
    /// Opens a modal message confirmation dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="buttonText">The button text.</param>
    /// <returns>Task</returns>
    Task<bool?> MessageConfirmationDialogAsync(string title, string message, string buttonText);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <returns>Task</returns>
    Task<bool?> ConfirmationDialogAsync(string title);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <returns>Task</returns>
    Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <param name="cancelButtonText">The cancelbutton text.</param>
    /// <returns>Task</returns>
    Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText, string cancelButtonText);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The content.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <param name="cancelButtonText">The cancelbutton text.</param>
    /// <returns>Task</returns>
    Task<bool?> ConfirmationDialogAsync(string title, string message, string yesButtonText, string noButtonText, string cancelButtonText);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <returns>Task.</returns>
    Task<bool?> WarningDialogAsync(string title, string message, string yesButtonText, string noButtonText);

    /// <summary>
    /// Opens a modal message Inputdialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="textBoxName">Header.</param>
    /// <returns>Task</returns>
    Task<string?> InputDialogAsync(string title, string message, string textBoxName);

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="downloadResult">The title.</param>
    /// <returns>Task</returns>
    Task LiftDataManagerdownloadInfoAsync(DownloadInfo downloadResult);

    /// <summary>
    /// Opens a modal message LiftPlannerDB.
    /// </summary>
    /// <param name="liftPlanner">The title.</param>
    /// <returns>Task</returns>
    Task<int> LiftPlannerDBDialogAsync(int liftPlanner);

    /// <summary>
    /// Opens a modal passwordDialog.
    /// </summary>
    /// <param name="title">title</param>
    /// <param name="condition">condition</param>
    /// <param name="description">description</param>
    /// <returns>Task</returns>
    Task<bool> PasswordDialogAsync(string? title, string? condition, string? description);

    /// <summary>
    /// Opens a modal ZiehlAbeggProcessingDialog.
    /// </summary>
    /// <param name="fullPathXml">FullPathXml AutodeskTransfer.xml</param>
    /// <returns>Task</returns>
    Task<bool> ZALiftDialogAsync(string? fullPathXml);

    /// <summary>
    /// Opens a modal CarFrameProcessingDialog.
    /// </summary>
    /// <param name="fullPathXml">FullPathXml AutodeskTransfer.xml</param>
    /// <param name="carFrameTyp">carFrameTyp</param>
    /// <returns>Task</returns>
    Task<bool> CFPEditDialogAsync(string? fullPathXml, string? carFrameTyp);

    /// <summary>
    /// Opens a modal ParameterChangeDialog.
    /// </summary>
    /// <param name="parameterChangedList">List with changed parameter(InfoCenterEntry)</param>
    /// <returns>Task</returns>
    Task ParameterChangedDialogAsync(List<InfoCenterEntry> parameterChangedList);

    /// <summary>
    /// Opens a modal AppClosingDialog.
    /// </summary>
    /// <returns>Task</returns>
    Task<(ContentDialogResult, bool)> AppClosingDialogAsync(bool ignoreSaveWarning);

    /// <summary>
    /// Opens a modal Liftdata import dialog.
    /// </summary>
    /// <param name="fullPathXml">FullPathXml AutodeskTransfer.xml</param>
    /// <param name="spezifikationName">name of the current open spezifikation</param>
    /// <param name="spezifikationTyp">type of the current open spezifikation</param>
    /// <returns>Task<(string?, IEnumerable<TransferData>?)></returns>
    Task<(string?, IEnumerable<TransferData>?)> ImportLiftDataDialogAsync(string fullPathXml, string spezifikationName, SpezifikationTyp spezifikationTyp);

    /// <summary>
    /// Opens a modal CheckOutDialog.
    /// </summary>
    /// <param name="spezifikationName">name of the current open spezifikation</param>
    /// <param name="forceCheckOut">force CheckOut</param>
    /// <returns>Task<CheckOutDialogResult></returns>
    Task<CheckOutDialogResult> CheckOutDialogAsync(string spezifikationName, bool forceCheckOut = false);
}