using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class BausatzPage : Page
{
    public BausatzViewModel ViewModel
    {
        get;
    }

    public BausatzPage()
    {
        ViewModel = App.GetService<BausatzViewModel>();
        InitializeComponent();
    }
}
