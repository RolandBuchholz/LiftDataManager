using Cogs.Collections;
using LiftDataManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace LiftDataManager.Controls;
public sealed partial class FooterControl : UserControl
{
    public ObservableCollection<ParameterStateInfo> ErrorsList { get; set; }


    public FooterControl()
    {
        InitializeComponent();
        Loaded += OnLoadFooterControl;
        Unloaded += OnUnLoadFooterControl;
        ErrorsList ??= new();
    }

    public ParameterStateInfo? SelectedError { get; set; }

    [RelayCommand]
    private async Task ErrorDialogAsync()
    {
        await DetailErrorDialog?.ShowAsync();
    }

    [RelayCommand]
    private void NavigateToError()
    {
        if (SelectedError is not null)
        {
            DetailErrorDialog.Hide();
            var nav = App.GetService<INavigationService>();
            nav.NavigateTo("LiftDataManager.ViewModels.DatenansichtDetailViewModel", SelectedError.Name);
        }
    }

    private void OnLoadFooterControl(object sender, RoutedEventArgs e)
    {
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

    private readonly SolidColorBrush checkIncolorBrush = new(Colors.Red);
    private readonly SolidColorBrush checkOutcolorBrush = new(Colors.Green);

    private void UpdateInfobar()
    {
        ErrorsList.Clear();
        if (ErrorsDictionary is not null && ErrorsDictionary.Any())
        {
            try
            {
                var foundParameterErrors = new List<ParameterStateInfo>();

                foreach (var parameterErrors in ErrorsDictionary.Values)
                {
                    foreach (var error in parameterErrors)
                    {
                        if (HideInfoErrors && error.Severity == ParameterStateInfo.ErrorLevel.Informational)
                            break;
                        foundParameterErrors.Add(error);
                    }
                }

                var sortedErrors = foundParameterErrors.OrderByDescending(error => error.Severity);

                foreach (var error in sortedErrors)
                {
                    ErrorsList.Add(error);
                }

                ErrorCount = ErrorsList.Count((e) => e.Severity == ParameterStateInfo.ErrorLevel.Error);
                WarningCount = ErrorsList.Count((e) => e.Severity == ParameterStateInfo.ErrorLevel.Warning);
                InfoCount = ErrorsList.Count((e) => e.Severity == ParameterStateInfo.ErrorLevel.Informational);
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
        { return InfoBarSeverity.Error; }
        if (WarningCount > 0)
        { return InfoBarSeverity.Warning; }
        if (InfoCount > 0)
        { return InfoBarSeverity.Informational; }
        return InfoBarState = InfoBarSeverity.Success;
    }

    public ObservableDictionary<string, List<ParameterStateInfo>> ErrorsDictionary
    {
        get => (ObservableDictionary<string, List<ParameterStateInfo>>)GetValue(ErrorsDictionaryProperty);
        set => SetValue(ErrorsDictionaryProperty, value);
    }

    public static readonly DependencyProperty ErrorsDictionaryProperty =
        DependencyProperty.Register("ErrorsDictionary", typeof(ObservableDictionary<string, List<ParameterStateInfo>>), typeof(FooterControl), new PropertyMetadata(null));


    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register("ErrorMessage", typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public string FileInfo
    {
        get => (string)GetValue(FileInfoProperty);
        set => SetValue(FileInfoProperty, value);
    }

    public static readonly DependencyProperty FileInfoProperty =
        DependencyProperty.Register("FileInfo", typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public SolidColorBrush FileInfoForeground
    {
        get => (SolidColorBrush)GetValue(FileInfoForegroundProperty);
        set => SetValue(FileInfoForegroundProperty, value);
    }

    public static readonly DependencyProperty FileInfoForegroundProperty =
        DependencyProperty.Register("FileInfoForeground", typeof(SolidColorBrush), typeof(FooterControl), new PropertyMetadata(null));

    public bool CheckOut
    {
        get => (bool)GetValue(CheckOutProperty);
        set
        {
            SetValue(CheckOutProperty, value);
            FileInfo = value ? "CheckOut - Datei kann gespeichert werden" : "CheckIn - Datei schreibgeschützt";
            FileInfoForeground = value ? checkOutcolorBrush : checkIncolorBrush;
        }
    }

    public static readonly DependencyProperty CheckOutProperty =
        DependencyProperty.Register("CheckOut", typeof(bool), typeof(FooterControl), new PropertyMetadata(false));

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
        DependencyProperty.Register("HasErrors", typeof(bool), typeof(FooterControl), new PropertyMetadata(false));

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
        DependencyProperty.Register("HideInfoErrors", typeof(bool), typeof(FooterControl), new PropertyMetadata(false));

    public string XmlPath
    {
        get => ((string)GetValue(XmlPathProperty)).Split(@"\").Last();
        set => SetValue(XmlPathProperty, value);
    }

    public static readonly DependencyProperty XmlPathProperty =
        DependencyProperty.Register("XmlPath", typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

    public int ParameterFound
    {
        get => (int)GetValue(ParameterFoundProperty);
        set => SetValue(ParameterFoundProperty, value);
    }

    public static readonly DependencyProperty ParameterFoundProperty =
        DependencyProperty.Register("ParameterFound", typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int ErrorCount
    {
        get => (int)GetValue(ErrorCountProperty);
        set => SetValue(ErrorCountProperty, value);
    }

    public static readonly DependencyProperty ErrorCountProperty =
        DependencyProperty.Register("ErrorCount", typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int WarningCount
    {
        get => (int)GetValue(WarningCountProperty);
        set => SetValue(WarningCountProperty, value);
    }

    public static readonly DependencyProperty WarningCountProperty =
        DependencyProperty.Register("WarningCount", typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public int InfoCount
    {
        get => (int)GetValue(InfoCountProperty);
        set => SetValue(InfoCountProperty, value);
    }

    public static readonly DependencyProperty InfoCountProperty =
        DependencyProperty.Register("InfoCount", typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    public InfoBarSeverity InfoBarState
    {
        get => (InfoBarSeverity)GetValue(InfoBarStateProperty);
        set => SetValue(InfoBarStateProperty, value);
    }

    public static readonly DependencyProperty InfoBarStateProperty =
        DependencyProperty.Register("InfoBarState", typeof(InfoBarSeverity), typeof(FooterControl), new PropertyMetadata(InfoBarSeverity.Informational));
}
