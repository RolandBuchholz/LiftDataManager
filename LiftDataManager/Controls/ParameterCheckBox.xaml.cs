using System.ComponentModel;

namespace LiftDataManager.Controls;

public sealed partial class ParameterCheckBox : UserControl
{
    public ParameterCheckBox()
    {
        InitializeComponent();
        Loaded += OnLoadParameterTextBox;
        Unloaded += OnUnLoadParameterTextBox;

    }

    private void OnLoadParameterTextBox(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged += OnErrorsChanged;
        }
    }

    private void OnUnLoadParameterTextBox(object sender, RoutedEventArgs e)
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
                    ParameterStateInfo.ErrorLevel.Informational => "\ue946",
                    ParameterStateInfo.ErrorLevel.Warning => "\ue7ba",
                    ParameterStateInfo.ErrorLevel.Error => "\ue730",
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

    public bool ShowDefaultCheckBoxContent
    {
        get => (bool)GetValue(ShowDefaultCheckBoxContentProperty);
        set => SetValue(ShowDefaultCheckBoxContentProperty, value);
    }

    public static readonly DependencyProperty ShowDefaultCheckBoxContentProperty =
        DependencyProperty.Register("ShowDefaultCheckBoxContent", typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public string CheckBoxContent
    {
        get => ShowDefaultCheckBoxContent ? LiftParameter?.DisplayName! : (string)GetValue(CheckBoxContentProperty);
        set => SetValue(CheckBoxContentProperty, value);
    }

    public static readonly DependencyProperty CheckBoxContentProperty =
        DependencyProperty.Register("CheckBoxContent", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register("PlaceholderText", typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

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