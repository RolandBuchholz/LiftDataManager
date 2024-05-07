using Humanizer;
using Microsoft.UI.Text;

namespace LiftDataManager.Services;

public class DialogService : IDialogService
{
    private static FrameworkElement MainRoot => App.MainRoot!;

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <returns>Task.</returns>
    public async Task MessageDialogAsync(string title, string message)
    {

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="buttonText">The button text.</param>
    /// <returns>Task.</returns>
    public async Task MessageDialogAsync(string title, string message, string buttonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = buttonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="buttonText">The button text.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> MessageConfirmationDialogAsync(string title, string message, string buttonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = buttonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        var result = await dialog.ShowAsyncQueueDraggable();

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
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(string title)
    {
        return await ConfirmationDialogAsync(title, "OK", string.Empty, "Cancel");
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText)
    {
        return (await ConfirmationDialogAsync(title, yesButtonText, noButtonText, string.Empty)).Value;
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <param name="cancelButtonText">The cancelbutton text.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText, string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            CloseButtonText = cancelButtonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        var result = await dialog.ShowAsyncQueueDraggable();

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
    /// <param name="message">The content.</param>
    /// <param name="yesButtonText">The yesbutton text.</param>
    /// <param name="noButtonText">The nobutton text.</param>
    /// <param name="cancelButtonText">The cancelbutton text.</param>
    /// <returns>Task.</returns>
    public async Task<bool?> ConfirmationDialogAsync(string title, string message, string yesButtonText, string noButtonText, string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            CloseButtonText = cancelButtonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        var result = await dialog.ShowAsyncQueueDraggable();

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

    public async Task<bool?> WarningDialogAsync(string title, string message, string yesButtonText, string noButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        var result = await dialog.ShowAsyncQueueDraggable();

        if (result == ContentDialogResult.None)
        {
            return null;
        }

        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Opens a modal message Inputdialog.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="textBoxName">Header.</param>
    /// <returns>Task.</returns>

    public async Task<string?> InputDialogAsync(string title, string message, string textBoxName)
    {
        var textresult = new TextBox
        {
            Text = string.Empty,
            Header = textBoxName,
            MinHeight = 62,
            Margin = new Thickness(0, 10, 0, 10),
            FontSize = 12,
            AcceptsReturn = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var mainPanel = new StackPanel();
        mainPanel.Children.Add(new TextBlock
        {
            Margin = new Thickness(0, 10, 0, 0),
            Text = message,
            TextWrapping = TextWrapping.Wrap,
        });
        mainPanel.Children.Add(textresult);

        var dialog = new ContentDialog
        {
            Title = title,
            Content = mainPanel,
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Abbrechen",
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        var result = await dialog.ShowAsyncQueueDraggable();

        if (result == ContentDialogResult.None || result == ContentDialogResult.Secondary)
        {
            return null;
        }

        return textresult.Text;
    }

    /// <summary>
    /// Opens a modal message dialog.
    /// </summary>
    /// <param name="downloadResult">The title.</param>
    /// <returns>Task.</returns>
    public async Task LiftDataManagerdownloadInfoAsync(DownloadInfo downloadResult)
    {
        var title = "LiftDataManager InfoDialog";

        var closeButtonText = "Ok";

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
            IsExpanded = true,
            Content = detailPanel
        };

        var mainPanel = new StackPanel();
        mainPanel.Children.Add(new TextBlock
        {
            Margin = new Thickness(0, 10, 0, 0),
            Text = downloadResult.ExitState.Humanize(),
            TextWrapping = TextWrapping.Wrap,
        });
        mainPanel.Children.Add(exp);

        var dialog = new ContentDialog
        {
            Title = title,
            Content = mainPanel,
            CloseButtonText = closeButtonText,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
    }
}