namespace LiftDataManager.Controls;

public sealed partial class FooterControl : UserControl
{
    public FooterControl()
    {
        InitializeComponent();
    }

    private readonly SolidColorBrush checkIncolorBrush = new(Colors.Red);
    private readonly SolidColorBrush checkOutcolorBrush = new(Colors.Green);

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
}
