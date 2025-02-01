using Cogs.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.Appointments.DataProvider;

namespace LiftDataManager.Controls;
public sealed partial class FooterControl : UserControl
{
    private static readonly SolidColorBrush checkIncolorBrush = new(Colors.Red);
    private static readonly SolidColorBrush checkOutcolorBrush = new(Colors.Green);
    private static readonly SolidColorBrush lokalModecolorBrush = new(Colors.CornflowerBlue);
    public ObservableCollection<ParameterStateInfo> ErrorsList { get; set; }

    public FooterControl()
    {
        InitializeComponent();
        Loaded += OnLoadFooterControl;
        Unloaded += OnUnLoadFooterControl;
        ErrorsList ??= [];
    }

    public ParameterStateInfo? SelectedError { get; set; }

    [RelayCommand]
    private async Task ErrorDialogAsync()
    {
        await DetailErrorDialog.ShowAsyncQueueDraggable();
    }

    [RelayCommand]
    private void NavigateToError()
    {
        if (SelectedError is not null)
        {
            DetailErrorDialog.Hide();
            var navigationService = App.GetService<IJsonNavigationViewService>();
            navigationService.NavigateTo(typeof(DatenansichtDetailPage), SelectedError.Name);
        }
    }

    private void OnLoadFooterControl(object sender, RoutedEventArgs e)
    {
        if (VaultDisabled)
        {
            FileInfo = "LocalMode - Dateien werden im Windowsdateisystem abgelegt";
            FileInfoForeground = lokalModecolorBrush;
        }
        else
        {
            FileInfo = CheckOut ? "CheckOut - Datei kann gespeichert werden" : "CheckIn - Datei schreibgeschützt";
            FileInfoForeground = CheckOut ? checkOutcolorBrush : checkIncolorBrush;
        }
        ErrorsDictionary.PropertyChanged += ErrorsDictionaryPropertyChanged;
    }

    private void OnUnLoadFooterControl(object sender, RoutedEventArgs e)
    {
        ErrorsDictionary.PropertyChanged -= ErrorsDictionaryPropertyChanged;
    }

    private void ErrorsDictionaryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        UpdateInfobar();
    }

    private void UpdateInfobar()
    {
        ErrorsList.Clear();
        if (ErrorsDictionary is not null && ErrorsDictionary.Count != 0)
        {
            try
            {
                var foundParameterErrors = new List<ParameterStateInfo>();

                foreach (var parameterErrors in ErrorsDictionary.Values)
                {
                    foreach (var error in parameterErrors)
                    {
                        if (HideInfoErrors && error.Severity == ErrorLevel.Informational)
                            break;
                        foundParameterErrors.Add(error);
                    }
                }

                var sortedErrors = foundParameterErrors.OrderByDescending(error => error.Severity);

                foreach (var error in sortedErrors)
                {
                    ErrorsList.Add(error);
                }

                ErrorCount = ErrorsList.Count((e) => e.Severity == ErrorLevel.Error);
                WarningCount = ErrorsList.Count((e) => e.Severity == ErrorLevel.Warning);
                InfoCount = ErrorsList.Count((e) => e.Severity == ErrorLevel.Informational);
                ErrorMessage = (ErrorCount + WarningCount + InfoCount > 0)
                                ? ErrorsList.First()!.ErrorMessage!
                                : string.Empty;
            }
            catch
            {
                ErrorMessage = "Fehlermeldungen konnten nicht geladen werden.";
            }

            InfoBarState = GetInfoBarState();
        }
        else
        {
            ErrorMessage = string.Empty;
        }
    }

    private InfoBarSeverity GetInfoBarState()
    {
        if (ErrorCount > 0)
        {
            return InfoBarSeverity.Error; 
        }
        if (WarningCount > 0)
        { 
            return InfoBarSeverity.Warning; 
        }
        if (InfoCount > 0)
        { 
            return InfoBarSeverity.Informational; 
        }
        return InfoBarState = InfoBarSeverity.Success;
    }

    public ObservableDictionary<string, List<ParameterStateInfo>> ErrorsDictionary
    {
        get => (ObservableDictionary<string, List<ParameterStateInfo>>)GetValue(ErrorsDictionaryProperty);
        set => SetValue(ErrorsDictionaryProperty, value);
    }

    public static readonly DependencyProperty ErrorsDictionaryProperty =
        DependencyProperty.Register(nameof(ErrorsDictionary), typeof(ObservableDictionary<string, List<ParameterStateInfo>>), typeof(FooterControl), new PropertyMetadata(null));

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public string FileInfo
    {
        get => (string)GetValue(FileInfoProperty);
        set => SetValue(FileInfoProperty, value);
    }

    public static readonly DependencyProperty FileInfoProperty =
        DependencyProperty.Register(nameof(FileInfo), typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public SolidColorBrush FileInfoForeground
    {
        get => (SolidColorBrush)GetValue(FileInfoForegroundProperty);
        set => SetValue(FileInfoForegroundProperty, value);
    }

    public static readonly DependencyProperty FileInfoForegroundProperty =
        DependencyProperty.Register(nameof(FileInfoForeground), typeof(SolidColorBrush), typeof(FooterControl), new PropertyMetadata(null));

    public bool CheckOut
    {
        get => (bool)GetValue(CheckOutProperty);
        set
        {
            SetValue(CheckOutProperty, value);
        }
    }

    public static readonly DependencyProperty CheckOutProperty =
        DependencyProperty.Register(nameof(CheckOut), typeof(bool), typeof(FooterControl), new PropertyMetadata(false, CheckOutPropertyChangedCallback));

    private static void CheckOutPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)d.GetValue(VaultDisabledProperty))
        {
            d.SetValue(FileInfoProperty, "LocalMode - Dateien werden im Windowsdateisystem abgelegt");
            d.SetValue(FileInfoForegroundProperty, lokalModecolorBrush);
        }
        else
        {
            d.SetValue(FileInfoProperty, (bool)e.NewValue ? "CheckOut - Datei kann gespeichert werden" : "CheckIn - Datei schreibgeschützt");
            d.SetValue(FileInfoForegroundProperty, (bool)e.NewValue ? checkOutcolorBrush : checkIncolorBrush);
        }
    }
    public bool VaultDisabled
    {
        get => (bool)GetValue(VaultDisabledProperty);
        set
        {
            SetValue(VaultDisabledProperty, value);
        }
    }

    public static readonly DependencyProperty VaultDisabledProperty =
        DependencyProperty.Register(nameof(VaultDisabled), typeof(bool), typeof(FooterControl), new PropertyMetadata(false));
    public bool HasErrors
    {
        get => (bool)GetValue(HasErrorsProperty);
        set
        {
            UpdateInfobar();
            SetValue(HasErrorsProperty, value);
        }
    }

    public static readonly DependencyProperty HasErrorsProperty =
        DependencyProperty.Register(nameof(HasErrors), typeof(bool), typeof(FooterControl), new PropertyMetadata(false));

    public bool HideInfoErrors
    {
        get => (bool)GetValue(HideInfoErrorsProperty);
        set
        {
            SetValue(HideInfoErrorsProperty, value);
            UpdateInfobar();
        }
    }

    public static readonly DependencyProperty HideInfoErrorsProperty =
        DependencyProperty.Register(nameof(HideInfoErrors), typeof(bool), typeof(FooterControl), new PropertyMetadata(false));

    public string XmlPath
    {
        get => ((string)GetValue(XmlPathProperty)).Split(@"\").Last();
        set => SetValue(XmlPathProperty, value);
    }

    public static readonly DependencyProperty XmlPathProperty =
        DependencyProperty.Register(nameof(XmlPath), typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public int ParameterFound
    {
        get => (int)GetValue(ParameterFoundProperty);
        set => SetValue(ParameterFoundProperty, value);
    }

    public static readonly DependencyProperty ParameterFoundProperty =
        DependencyProperty.Register(nameof(ParameterFound), typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int ErrorCount
    {
        get => (int)GetValue(ErrorCountProperty);
        set => SetValue(ErrorCountProperty, value);
    }

    public static readonly DependencyProperty ErrorCountProperty =
        DependencyProperty.Register(nameof(ErrorCount), typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int WarningCount
    {
        get => (int)GetValue(WarningCountProperty);
        set => SetValue(WarningCountProperty, value);
    }

    public static readonly DependencyProperty WarningCountProperty =
        DependencyProperty.Register(nameof(WarningCount), typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int InfoCount
    {
        get => (int)GetValue(InfoCountProperty);
        set => SetValue(InfoCountProperty, value);
    }

    public static readonly DependencyProperty InfoCountProperty =
        DependencyProperty.Register(nameof(InfoCount), typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public InfoBarSeverity InfoBarState
    {
        get => (InfoBarSeverity)GetValue(InfoBarStateProperty);
        set => SetValue(InfoBarStateProperty, value);
    }

    public static readonly DependencyProperty InfoBarStateProperty =
        DependencyProperty.Register(nameof(InfoBarState), typeof(InfoBarSeverity), typeof(FooterControl), new PropertyMetadata(InfoBarSeverity.Informational));
}
