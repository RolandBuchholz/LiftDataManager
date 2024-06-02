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

    private void ListView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
       if (ViewModel.CanAddCustomRailBracketDistance)
       {
            ViewModel.AddCustomRailBracketDistanceCommand.Execute(null);
       }  
    }
}
