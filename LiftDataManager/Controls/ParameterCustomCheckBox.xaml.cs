namespace LiftDataManager.Controls;

public sealed partial class ParameterCustomCheckBox : UserControl
{
    public ParameterCustomCheckBox()
    {
        InitializeComponent();
    }

    public Parameter? LiftParameter
    {
        get => GetValue(LiftParameterProperty) as Parameter;
        set => SetValue(LiftParameterProperty, value);
    }

    public static readonly DependencyProperty LiftParameterProperty =
        DependencyProperty.Register(nameof(LiftParameter), typeof(Parameter), typeof(ParameterComboBox), new PropertyMetadata(null));

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

            var goToHighLightedParameter = new AppBarButton() { Icon = new SymbolIcon(Symbol.ShowResults), Label = "Show all HighLighted Parameter" };
            goToHighLightedParameter.Click += NavigateToHighlightParameters_Click;
            var setMainEntranceToolTip = new ToolTip { Content = "All HighLighted Parameter" };
            ToolTipService.SetToolTip(goToHighLightedParameter, setMainEntranceToolTip);

            var goToParameterDetails = new AppBarButton() { Icon = new SymbolIcon(Symbol.PreviewLink), Label = "Show Parameterdetails" };
            goToParameterDetails.Click += NavigateToParameterDetails_Click;
            var setParameterDetailToolTip = new ToolTip { Content = "Show Parameterdetails" };
            ToolTipService.SetToolTip(goToParameterDetails, setParameterDetailToolTip);

            entranceFlyout.PrimaryCommands.Add(highLightParameter);
            entranceFlyout.PrimaryCommands.Add(goToHighLightedParameter);
            entranceFlyout.PrimaryCommands.Add(goToParameterDetails);
        }
    }

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
}