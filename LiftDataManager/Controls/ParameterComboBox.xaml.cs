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

    public bool IsControlActive
    {
        get => (bool)GetValue(IsControlActiveProperty);
        set => SetValue(IsControlActiveProperty, value);
    }

    public static readonly DependencyProperty IsControlActiveProperty =
        DependencyProperty.Register(nameof(IsControlActive), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(true));

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

    public static readonly DependencyProperty LiftParameterProperty = DependencyProperty.Register(nameof(LiftParameter), typeof(Parameter), typeof(ParameterComboBox), new PropertyMetadata(null));

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

    public bool ShowDefaultHeader
    {
        get => (bool)GetValue(ShowDefaultHeaderProperty);
        set => SetValue(ShowDefaultHeaderProperty, value);
    }

    public static readonly DependencyProperty ShowDefaultHeaderProperty =
        DependencyProperty.Register(nameof(ShowDefaultHeader), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public string Header
    {
        get => ShowDefaultHeader ? LiftParameter?.DisplayName! : (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string ErrorGlyph
    {
        get { return (string)GetValue(ErrorGlyphProperty); }
        set { SetValue(ErrorGlyphProperty, value); }
    }

    public static readonly DependencyProperty ErrorGlyphProperty =
        DependencyProperty.Register(nameof(ErrorGlyph), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string ErrorType
    {
        get { return (string)GetValue(ErrorTypeProperty); }
        set { SetValue(ErrorTypeProperty, value); }
    }

    public static readonly DependencyProperty ErrorTypeProperty =
        DependencyProperty.Register(nameof(ErrorType), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public int BorderHeight
    {
        get { return (int)GetValue(BorderHeightProperty); }
        set { SetValue(BorderHeightProperty, value); }
    }

    public static readonly DependencyProperty BorderHeightProperty =
        DependencyProperty.Register(nameof(BorderHeight), typeof(int), typeof(ParameterComboBox), new PropertyMetadata(33));

    public string HighlightAction
    {
        get => GetHighlightAction();
        set { SetValue(HighlightActionProperty, value); }
    }

    public static readonly DependencyProperty HighlightActionProperty =
        DependencyProperty.Register(nameof(HighlightAction), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    private void HighlightParameter_Click(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
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

    private void SetLiftParameterValue(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (!IsControlActive && LiftParameter is not null)
        {
           if (!string.IsNullOrWhiteSpace(LiftParameter.Value))
           {
                LiftParameter.Value = string.Empty;
                LiftParameter.DropDownListValue = null;
           }
        }
    }
}