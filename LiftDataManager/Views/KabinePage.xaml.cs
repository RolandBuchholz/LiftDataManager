using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class KabinePage : Page
{
    public KabineViewModel ViewModel
    {
        get;
    }

    public KabinePage()
    {
        ViewModel = App.GetService<KabineViewModel>();
        InitializeComponent();
    }
}
