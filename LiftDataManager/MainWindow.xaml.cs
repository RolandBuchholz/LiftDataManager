namespace LiftDataManager;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();
        Content = null;
        ExtendsContentIntoTitleBar = true;
        this.SetTitleBarBackgroundColors(Microsoft.UI.Colors.Transparent);
    }
}
