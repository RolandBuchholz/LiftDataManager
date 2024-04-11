using Microsoft.UI.Xaml.Input;
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
        SetBorderHeight();
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
                    ErrorLevel.Informational => "\ue946",
                    ErrorLevel.Warning => "\ue7ba",
                    ErrorLevel.Error => "\ue730",
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

            var goToHighLightedParameter = new AppBarButton() { Icon = new SymbolIcon(Symbol.ShowResults), Label = "All High Lighted" };
            goToHighLightedParameter.Click += NavigateToHighlightParameters_Click;
            var setMainEntranceToolTip = new ToolTip { Content = "Show all HighLighted Parameter" };
            ToolTipService.SetToolTip(goToHighLightedParameter, setMainEntranceToolTip);

            var goToParameterDetails = new AppBarButton() { Icon = new SymbolIcon(Symbol.PreviewLink), Label = "Parameterdetails" };
            goToParameterDetails.Click += NavigateToParameterDetails_Click;
            var setParameterDetailToolTip = new ToolTip { Content = "Show Parameterdetails" };
            ToolTipService.SetToolTip(goToParameterDetails, setParameterDetailToolTip);

            entranceFlyout.PrimaryCommands.Add(highLightParameter);
            entranceFlyout.PrimaryCommands.Add(goToHighLightedParameter);
            entranceFlyout.PrimaryCommands.Add(goToParameterDetails);
        }
    }

    public bool ReadOnly
    {
        get => (bool)GetValue(ReadOnlyProperty);
        set => SetValue(ReadOnlyProperty, value);
    }

    public static readonly DependencyProperty ReadOnlyProperty =
        DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

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

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

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

    public double BorderHeight
    {
        get { return (double)GetValue(BorderHeightProperty); }
        set { SetValue(BorderHeightProperty, value); }
    }

    public static readonly DependencyProperty BorderHeightProperty =
        DependencyProperty.Register(nameof(BorderHeight), typeof(double), typeof(ParameterComboBox), new PropertyMetadata(default));

    public string HighlightAction
    {
        get => GetHighlightAction();
        set { SetValue(HighlightActionProperty, value); }
    }

    public static readonly DependencyProperty HighlightActionProperty =
        DependencyProperty.Register(nameof(HighlightAction), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    private void HighlightParameter_Click(object sender, RoutedEventArgs e)
    {
        if (LiftParameter != null)
        {
            LiftParameter.IsKey = !LiftParameter.IsKey;
            HighlightAction = GetHighlightAction();
        }
    }

    private void SetBorderHeight()
    {
        var controlHeight = cdp_Liftparameter.ActualHeight;
        if (controlHeight > 24d)
        {
            BorderHeight = controlHeight - 24d;
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

    public void MoveFocus(Object sender, KeyRoutedEventArgs e)
    {
        var ptx = sender as ParameterTextBox;
        if (ptx is not null)
        {
            FindNextElementOptions fneo = new() { SearchRoot = ptx.XamlRoot.Content };
            _ = FocusManager.TryMoveFocus(FocusNavigationDirection.Next, fneo);
        }
    }
}