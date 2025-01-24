using Cogs.Collections;
using Humanizer;
using Microsoft.UI.Text;

namespace LiftDataManager.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="IDialogService"/> <see langword="interface"/> using contentDialog service
/// </summary>
public class DialogService : IDialogService
{
    private static FrameworkElement MainRoot => App.MainRoot!;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<bool?> ConfirmationDialogAsync(string title)
    {
        return await ConfirmationDialogAsync(title, "OK", string.Empty, "Cancel");
    }

    /// <inheritdoc/>
    public async Task<bool?> ConfirmationDialogAsync(string title, string yesButtonText, string noButtonText)
    {
        return (await ConfirmationDialogAsync(title, yesButtonText, noButtonText, string.Empty)).Value;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<int> LiftPlannerDBDialogAsync(int liftPlannerId)
    {
        var dialog = new LiftPlannerDBDialog()
        {
            XamlRoot = MainRoot.XamlRoot,
            LiftPlannerId = liftPlannerId,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
        return dialog.LiftPlannerId;
    }

    /// <inheritdoc/>
    public async Task<bool> PasswordDialogAsync(string? title, string? condition, string? description)
    {
        var dialog = new PasswortDialog()
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
            Title = title,
            Condition = condition,
            Description = description
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        if (result == ContentDialogResult.None || result == ContentDialogResult.Secondary)
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ZALiftDialogAsync(string? fullPathXml)
    {
        var dialog = new ZALiftDialog()
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
            FullPathXml = fullPathXml
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        if (result == ContentDialogResult.None || result == ContentDialogResult.Secondary)
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> CFPEditDialogAsync(string? fullPathXml, string? carFrameTyp)
    {
        var dialog = new CFPEditDialog()
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
            FullPathXml = fullPathXml,
            CarFrameTyp = carFrameTyp
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        if (result == ContentDialogResult.None || result == ContentDialogResult.Secondary)
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public async Task ParameterChangedDialogAsync(List<InfoCenterEntry> parameterChangedList)
    {
        var dialog = new ParameterChangedDialog()
        {
            ParameterChangedList = parameterChangedList,
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
    }

    /// <inheritdoc/>
    public async Task<(ContentDialogResult, bool)> AppClosingDialogAsync(bool ignoreSaveWarning)
    {
        var dialog = new AppClosingDialog()
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
            IgnoreSaveWarning = ignoreSaveWarning
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        return (result, dialog.IgnoreSaveWarning);
    }

    /// <inheritdoc/>
    public async Task<(string?, IEnumerable<TransferData>?)> ImportLiftDataDialogAsync(string fullPathXml, string spezifikationName, SpezifikationTyp spezifikationTyp, bool vaultDisabled)
    {
        var dialog = new ImportLiftDataDialog(fullPathXml, spezifikationName, spezifikationTyp, vaultDisabled)
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        if (result == ContentDialogResult.None || result == ContentDialogResult.Secondary)
        {
            return (null, null);
        }
        return (dialog.ImportSpezifikationName, dialog.ImportPamameter);
    }

    /// <inheritdoc/>
    public async Task<CheckOutDialogResult> CheckOutDialogAsync(string spezifikationName, bool forceCheckOut = false)
    {
        var dialog = new CheckOutDialog(spezifikationName, forceCheckOut)
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme,
        };
        var result = await dialog.ShowAsyncQueueDraggable();
        if (result == ContentDialogResult.None || 
            result == ContentDialogResult.Secondary ||
            result == ContentDialogResult.Primary)
        {
            return dialog.CheckOutDialogResult;
        }
        return CheckOutDialogResult.CheckOutFailed;
    }

    /// <inheritdoc/>
    public async Task ValidationDialogAsync(int paramterCount, ObservableDictionary<string, List<ParameterStateInfo>>  parameterErrorDictionary)
    {
        var dialog = new ValidationDialog(paramterCount, parameterErrorDictionary)
        {
            XamlRoot = MainRoot.XamlRoot,
            RequestedTheme = MainRoot.ActualTheme
        };
        await dialog.ShowAsyncQueueDraggable();
    }
}