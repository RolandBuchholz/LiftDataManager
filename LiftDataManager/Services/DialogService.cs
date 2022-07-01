using System;
using System.Threading.Tasks;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Core.Models;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Services;

public class DialogService : IDialogService
{
    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <returns>Task.</returns>
    public async Task MessageDialogAsync(FrameworkElement element, string title, string message)
    {

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        await dialog.ShowAsync();
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="buttonText">The button text.</param>
    /// <returns>Task.</returns>
    public async Task MessageDialogAsync(FrameworkElement element, string title, string message, string buttonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = buttonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        await dialog.ShowAsync();
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(FrameworkElement element, string title)
    {
        return await ConfirmationDialogAsync(element, title, "OK", string.Empty, "Cancel");
    }
    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <returns>Task.</returns>
    public async Task<bool> ConfirmationDialogAsync(FrameworkElement element, string title, string yesButtonText, string noButtonText)
    {
        return (await ConfirmationDialogAsync(element, title, yesButtonText, noButtonText, string.Empty)).Value;
    }
    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <param name="cancelButtonText">The cancelbutton text.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(FrameworkElement element, string title, string yesButtonText, string noButtonText, string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            CloseButtonText = cancelButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return null;
        }

        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <returns>Task.</returns>

    public async Task<bool> WarningDialogAsync(FrameworkElement element, string title, string message, string yesButtonText, string noButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        var result = await dialog.ShowAsync();

        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="downloadResult">The title.</param>
    /// <returns>Task.</returns>
    public async Task LiftDataManagerdownloadInfoAsync(FrameworkElement element, DownloadInfo downloadResult)
    {
        string title = "LiftDataManager InfoDialog";

        string closeButtonText = "Ok";

        var infoHeader = new TextBlock
        {
            Text = $"DownloadInfos:",
            Margin = new Thickness(0, 0, 0, 10),
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var detailinfoleft = new TextBlock
        {
            Text = "FileName:\n" +
                   "FullFileName:\n" +
                   "CheckOutState:\n" +
                   "IsCheckOut:\n" +
                   "CheckOutPC:\n" +
                   "EditedBy:\n" +
                   "ErrorState:\n",
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var detailinforight = new TextBlock
        {
            Text = $"{downloadResult.FileName}\n" +
                   $"{downloadResult.FullFileName}\n" +
                   $"{downloadResult.CheckOutState}\n" +
                   $"{downloadResult.IsCheckOut}\n" +
                   $"{downloadResult.CheckOutPC}\n" +
                   $"{downloadResult.EditedBy}\n" +
                   $"{downloadResult.ErrorState}\n",
            Margin = new Thickness(25, 0, 0, 0),
            FontWeight = FontWeights.Medium,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var infoPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        infoPanel.Children.Add(detailinfoleft);
        infoPanel.Children.Add(detailinforight);

        var detailPanel = new StackPanel();
        detailPanel.Children.Add(infoHeader);
        detailPanel.Children.Add(infoPanel);

        var exp = new Expander
        {
            Header = "Detailinformationen",
            Margin = new Thickness(0, 20, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            Content = detailPanel
        };

        var mainPanel = new StackPanel();
        mainPanel.Children.Add(new TextBlock
        {
            Margin = new Thickness(0, 10, 0, 0),
            Text = DownloadInfoEnumToString(downloadResult),
            TextWrapping = TextWrapping.Wrap,
        });
        mainPanel.Children.Add(exp);

        var dialog = new ContentDialog
        {
            Title = title,
            Content = mainPanel,
            CloseButtonText = closeButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        await dialog.ShowAsync();
    }

    private string DownloadInfoEnumToString(DownloadInfo downloadResult)
    {
        string enumText;
        switch (downloadResult.ExitState)
        {
            case DownloadInfo.ExitCodeEnum.NoError:
                enumText = "kein Fehler";
                break;
            case DownloadInfo.ExitCodeEnum.UpDownLoadError:
                enumText = "Datei - Upload /Download/Reservierung enfernen fehlgeschlagen";
                break;
            case DownloadInfo.ExitCodeEnum.LoginError:
                enumText = "Login Fehler im Vault";
                break;
            case DownloadInfo.ExitCodeEnum.UpdatePropertiesError:
                enumText = "Eigenschaftsabgleich mit Vault fehlgeschlagen";
                break;
            case DownloadInfo.ExitCodeEnum.PowerShellStartError:
                enumText = "Interner Fehler in der PowerShellStart Programm";
                break;
            case DownloadInfo.ExitCodeEnum.MultipleAutoDeskTransferXml:
                enumText = "AutoDeskTransferXml mehrfach im Arbeitsbereich vorhanden";
                break;
            case DownloadInfo.ExitCodeEnum.InvalideOrderNumber:
                enumText = "Invalide Auftrags bzw. Angebotsnummer";
                break;
            case DownloadInfo.ExitCodeEnum.MissingVaultFile:
                enumText = "Datei im Vault oder Arbeitsbereich nicht gefunden";
                break;
            case DownloadInfo.ExitCodeEnum.MissingAdskLicensingSDK_5:
                enumText = "Fehlende AdskLicensingSDK_5.dll im Powershell Ordner";
                break;
            case DownloadInfo.ExitCodeEnum.MissingVaultClient_DataStandard:
                enumText = "Vault Client 2022 oder DataStandard wurde nicht gefunden";
                break;
            case DownloadInfo.ExitCodeEnum.CheckedOutByOtherUser:
                enumText = "AutoDeskTransferXml durch anderen Benutzer ausgechecked";
                break;
            case DownloadInfo.ExitCodeEnum.CheckedOutLinkedFilesByOtherUser:
                enumText = "AutoDeskTransferXml verbundene Dateien durch anderen Benutzer ausgechecked";
                break;
            default:
                enumText = "keine ExitCode vorhanden";
                break;
        }
        return enumText;
    }
}
