namespace LiftDataManager.Controls;

public sealed partial class HeaderControl : UserControl
{
    public HeaderControl()
    {
        InitializeComponent();
    }

    public bool ReadOnlyMode
    {
        get => (bool)GetValue(ReadOnlyModeProperty);
        set => SetValue(ReadOnlyModeProperty, value);
    }

    public static readonly DependencyProperty ReadOnlyModeProperty =
        DependencyProperty.Register(nameof(ReadOnlyMode), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));
}
