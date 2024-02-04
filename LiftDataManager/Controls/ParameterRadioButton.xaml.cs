using System.ComponentModel;

namespace LiftDataManager.Controls;

public sealed partial class ParameterRadioButton : UserControl
{
    public ParameterRadioButton()
    {
        InitializeComponent();
        Loaded += OnLoadParameterRadioButton;
        Unloaded += OnUnLoadParameterRadioButton;

    }

    private void OnLoadParameterRadioButton(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged += OnErrorsChanged;
        }
    }

    private void OnUnLoadParameterRadioButton(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
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

    public static readonly DependencyProperty LiftParameterProperty = DependencyProperty.Register("LiftParameter", typeof(Parameter), typeof(ParameterComboBox), new PropertyMetadata(null));

    private void SetParameterState(Parameter? liftParameter)
    {

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
                ErrorGlyph = error.Severity switch
                {
                    ErrorLevel.Informational => "\ue946",
                    ErrorLevel.Warning => "\ue7ba",
                    ErrorLevel.Error => "\ue730",
                    _ => string.Empty,
                };
                ErrorType = error.Severity.ToString();
            }
        }
    }

    public bool ReadOnly
    {
        get => (bool)GetValue(ReadOnlyProperty);
        set => SetValue(ReadOnlyProperty, value);
    }

    public static readonly DependencyProperty ReadOnlyProperty =
        DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public bool ShowDefaultRadioButtonContent
    {
        get => (bool)GetValue(ShowDefaultRadioButtonContentProperty);
        set => SetValue(ShowDefaultRadioButtonContentProperty, value);
    }

    public static readonly DependencyProperty ShowDefaultRadioButtonContentProperty =
        DependencyProperty.Register("ShowDefaultRadioButtonContent", typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public string RadioButtonContent
    {
        get => ShowDefaultRadioButtonContent ? LiftParameter?.DisplayName! : (string)GetValue(RadioButtonContentProperty);
        set => SetValue(RadioButtonContentProperty, value);
    }

    public static readonly DependencyProperty RadioButtonContentProperty =
        DependencyProperty.Register("RadioButtonContent", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register("PlaceholderText", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string RadioButtonGroupName
    {
        get => (string)GetValue(RadioButtonGroupNameProperty);
        set => SetValue(RadioButtonGroupNameProperty, value);
    }

    public static readonly DependencyProperty RadioButtonGroupNameProperty =
        DependencyProperty.Register("RadioButtonGroupName", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

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

    public string HighlightAction
    {
        get => GetHighlightAction();
        set { SetValue(HighlightActionProperty, value); }
    }

    public static readonly DependencyProperty HighlightActionProperty =
        DependencyProperty.Register("HighlightAction", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    private void HighlightParameter_Click(object sender, RoutedEventArgs e)
    {
        if (LiftParameter != null)
        {
            LiftParameter.IsKey = !LiftParameter.IsKey;
            HighlightAction = GetHighlightAction();
        }
    }

    private string GetHighlightAction() => LiftParameter!.IsKey ? "Remove highlight" : "Highlight Parameter";

    private void NavigateToHighlightParameters_Click(object sender, RoutedEventArgs e)
    {
        var nav = App.GetService<INavigationService>();
        nav.NavigateTo("LiftDataManager.ViewModels.ListenansichtViewModel", "ShowHighlightParameter");
    }

    private void NavigateToParameterDetails_Click(object sender, RoutedEventArgs e)
    {
        if (LiftParameter?.Name is not null)
        {
            var nav = App.GetService<INavigationService>();
            nav.NavigateTo("LiftDataManager.ViewModels.DatenansichtDetailViewModel", LiftParameter.Name);
        }
    }
}