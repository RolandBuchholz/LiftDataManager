using CommunityToolkit.Mvvm.Collections;

namespace LiftDataManager.Views;

public sealed partial class TabellenansichtPage : Page
{
    public TabellenansichtViewModel ViewModel
    {
        get;
    }

    public TabellenansichtPage()
    {
        ViewModel = App.GetService<TabellenansichtViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
