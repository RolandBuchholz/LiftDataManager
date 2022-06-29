using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class AntriebSteuerungNotrufPage : Page
{
    public AntriebSteuerungNotrufViewModel ViewModel
    {
        get;
    }

    public AntriebSteuerungNotrufPage()
    {
        ViewModel = App.GetService<AntriebSteuerungNotrufViewModel>();
        InitializeComponent();
    }
}
