using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class AllgemeineDatenPage : Page
{
    public AllgemeineDatenViewModel ViewModel
    {
        get;
    }

    public AllgemeineDatenPage()
    {
        ViewModel = App.GetService<AllgemeineDatenViewModel>();
        InitializeComponent();
    }
}
