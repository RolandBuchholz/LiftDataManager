namespace LiftDataManager.Controls;

public sealed partial class ParameterComboBox : UserControl
{
    public ParameterComboBox()
    {
        InitializeComponent();
        //Loaded += OnLoadParameterComboBoxControl;
        //Unloaded += OnUnLoadParameterComboBoxControl;
    }

    //private void OnUnLoadParameterComboBoxControl(object sender, RoutedEventArgs e) { }
    //private void OnLoadParameterComboBoxControl(object sender, RoutedEventArgs e) { }

    public Parameter? LiftParameter
    {
        get
        {
            return GetValue(LiftParameterProperty) as Parameter;
        }
        set => SetValue(LiftParameterProperty, value);
    }
    public static readonly DependencyProperty LiftParameterProperty = DependencyProperty.Register("LiftParameter", typeof(Parameter), typeof(ParameterComboBox) , new PropertyMetadata(null));

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
}
