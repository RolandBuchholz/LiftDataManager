using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class SonstigesPage : Page
{
    public SonstigesViewModel ViewModel
    {
        get;
    }

    public SonstigesPage()
    {
        ViewModel = App.GetService<SonstigesViewModel>();
        InitializeComponent();
    }
}
