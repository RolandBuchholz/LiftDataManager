using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class ErrorPage : Page
{
    public ErrorViewModel ViewModel
    {
        get;
    }

    public ErrorPage()
    {
        ViewModel = App.GetService<ErrorViewModel>();
        InitializeComponent();
    }
}
