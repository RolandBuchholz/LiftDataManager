using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class QuickLinksPage : Page
{
    public QuickLinksViewModel ViewModel
    {
        get;
    }

    public QuickLinksPage()
    {
        ViewModel = App.GetService<QuickLinksViewModel>();
        InitializeComponent();
    }
}
