namespace LiftDataManager.Views;

public sealed partial class EinreichunterlagenPage : Page
{
    public EinreichunterlagenViewModel ViewModel
    {
        get;
    }

    public EinreichunterlagenPage()
    {
        ViewModel = App.GetService<EinreichunterlagenViewModel>();
        InitializeComponent();
    }
}
