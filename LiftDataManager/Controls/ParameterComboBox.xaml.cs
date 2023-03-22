using System.ComponentModel;

namespace LiftDataManager.Controls;

public sealed partial class ParameterComboBox : UserControl
{
    public ParameterComboBox()
    {
        InitializeComponent();
        Loaded += OnLoadParameterComboBoxControl;
        Unloaded += OnUnLoadParameterComboBoxControl;

    }

    private void OnLoadParameterComboBoxControl(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged += OnErrorsChanged;
        }
    }

    private void OnUnLoadParameterComboBoxControl(object sender, RoutedEventArgs e)
    {
        if(LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged -= OnErrorsChanged;
        }
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            SetParameterState(sender as Parameter);
        }
    }

    private readonly SolidColorBrush errorcolorBrush = new(Colors.IndianRed);
    private readonly SolidColorBrush warningcolorBrush = new(Colors.Orange);
    private readonly SolidColorBrush infocolorBrush = new(Colors.Gray);

    public Parameter? LiftParameter
    {
        get
        {
            var liftparameter = GetValue(LiftParameterProperty) as Parameter;
            SetParameterState(liftparameter);
            return liftparameter;
        }
        set => SetValue(LiftParameterProperty, value);
    }

    public static readonly DependencyProperty LiftParameterProperty = DependencyProperty.Register("LiftParameter", typeof(Parameter), typeof(ParameterComboBox) , new PropertyMetadata(null));

    private void SetParameterState(Parameter? liftParameter)
    {
        ParameterInfoForeground = infocolorBrush;
        ErrorGlyph = string.Empty;
        ErrorType = string.Empty;

        if (liftParameter is null || !liftParameter.HasErrors)
            return;

        if (liftParameter.parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? errorList))
        {
            if (errorList is null)
                return;
            if (!errorList.Any())
                return;

            var error = errorList.OrderByDescending(p => p.Severity).FirstOrDefault();

            if (error is not null)
            {
                switch (error.Severity)
                {
                    case ParameterStateInfo.ErrorLevel.Informational:
                        ParameterInfoForeground = infocolorBrush;
                        ErrorGlyph = "\ue946";
                        break;
                    case ParameterStateInfo.ErrorLevel.Warning:
                        ParameterInfoForeground = warningcolorBrush;
                        ErrorGlyph = "\ue7ba";
                        break;
                    case ParameterStateInfo.ErrorLevel.Error:
                        ParameterInfoForeground = errorcolorBrush;
                        ErrorGlyph = "\ue730";
                        break;
                    default:
                        ParameterInfoForeground = infocolorBrush;
                        ErrorGlyph = string.Empty;
                        break;
                }
                ErrorType = error.Severity.ToString();
            }
        }
    }

    public bool ShowDefaultHeader
    {
        get => (bool)GetValue(ShowDefaultHeaderProperty);
        set => SetValue(ShowDefaultHeaderProperty, value);
    }

    public static readonly DependencyProperty ShowDefaultHeaderProperty =
        DependencyProperty.Register("ShowDefaultHeader", typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public string Header
    {
        get => ShowDefaultHeader ? LiftParameter?.DisplayName! : (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value); 
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public SolidColorBrush ParameterInfoForeground
    {
        get => (SolidColorBrush)GetValue(ParameterInfoForegroundProperty);
        set => SetValue(ParameterInfoForegroundProperty, value);
    }

    public static readonly DependencyProperty ParameterInfoForegroundProperty =
        DependencyProperty.Register("ParameterInfoForeground", typeof(SolidColorBrush), typeof(ParameterComboBox), new PropertyMetadata(null));

    public string ErrorGlyph
    {
        get { return (string)GetValue(ErrorGlyphProperty); }
        set { SetValue(ErrorGlyphProperty, value); }
    }

    public static readonly DependencyProperty ErrorGlyphProperty =
        DependencyProperty.Register("ErrorGlyph", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string ErrorType
    {
        get { return (string)GetValue(ErrorTypeProperty); }
        set { SetValue(ErrorTypeProperty, value); }
    }

    public static readonly DependencyProperty ErrorTypeProperty =
        DependencyProperty.Register("ErrorType", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public int BorderHeight
    {
        get { return (int)GetValue(BorderHeightProperty); }
        set { SetValue(BorderHeightProperty, value); }
    }

    public static readonly DependencyProperty BorderHeightProperty =
        DependencyProperty.Register("BorderHeight", typeof(int), typeof(ParameterComboBox), new PropertyMetadata(33));
}