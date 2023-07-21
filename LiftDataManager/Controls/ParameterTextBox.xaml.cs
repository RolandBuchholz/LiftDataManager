using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel;

namespace LiftDataManager.Controls;

public sealed partial class ParameterTextBox : UserControl
{
    public ParameterTextBox()
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

    private void TextCommandBarFlyout_Opening(object sender, object e)
    {
        var entranceFlyout = sender as TextCommandBarFlyout;

        if (entranceFlyout is not null && entranceFlyout.Target is TextBox)
        {
            var highLightParameter = new AppBarButton() { Icon = new SymbolIcon(Symbol.Highlight), Label = "Highlihght Parameter" };
            highLightParameter.Click += HighlightParameter_Click;
            var highLightParameterToolTip = new ToolTip
            {
                Content = GetHighlightAction()
            };
            ToolTipService.SetToolTip(highLightParameter, highLightParameterToolTip);

            var separator = new AppBarSeparator();

            var goToHighLightedParameter = new AppBarButton() { Icon = new SymbolIcon(Symbol.ShowResults), Label = "GoTo HighLighted Parameter" };
            goToHighLightedParameter.Click += NavigateToHighlightParameters_Click;
            var setMainEntranceToolTip = new ToolTip { Content = "All HighLighted Parameter" };

            ToolTipService.SetToolTip(goToHighLightedParameter, setMainEntranceToolTip);
            entranceFlyout.PrimaryCommands.Add(highLightParameter);
            entranceFlyout.PrimaryCommands.Add(separator);
            entranceFlyout.PrimaryCommands.Add(goToHighLightedParameter);
        }
    }

    public bool ReadOnly
    {
        get => (bool)GetValue(ReadOnlyProperty);
        set => SetValue(ReadOnlyProperty, value);
    }

    public static readonly DependencyProperty ReadOnlyProperty =
        DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

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

    public int BorderHeight
    {
        get { return (int)GetValue(BorderHeightProperty); }
        set { SetValue(BorderHeightProperty, value); }
    }

    public static readonly DependencyProperty BorderHeightProperty =
        DependencyProperty.Register("BorderHeight", typeof(int), typeof(ParameterComboBox), new PropertyMetadata(33));

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
}