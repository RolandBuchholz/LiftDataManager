namespace LiftDataManager.Views;

public sealed partial class BausatzDetailRailBracketPage : Page
{
    public BausatzDetailRailBracketViewModel ViewModel
    {
        get;
    }

    public BausatzDetailRailBracketPage()
    {
        ViewModel = App.GetService<BausatzDetailRailBracketViewModel>();
        InitializeComponent();
    }
}