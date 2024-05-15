using Microsoft.UI.Xaml.Media.Animation;
using static WinUICommunity.LanguageDictionary;

namespace LiftDataManager.Helpers;
public class LiftParameterNavigationHelper
{
    public static void NavigateToHighlightParameters()
    {
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(typeof(ListenansichtPage), "ShowHighlightParameter");
    }

    public static void NavigateToParameterDetails(string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(typeof(DatenansichtDetailPage), parameterName);
    }

    public static void NavigateToPage(Type page)
    {
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(page);
    }

    public static void NavigateToPage(Type page, object parameter)
    {
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(page, parameter);
    }

    public static void NavigatePivotItem(Type page)
    {
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(page, null, false, new DrillInNavigationTransitionInfo());
    }

    public static void NavigatePivotItem(Type page, object parameter)
    {
        var navigationService = App.GetService<IJsonNavigationViewService>();
        navigationService.NavigateTo(page, parameter, false, new DrillInNavigationTransitionInfo());
    }
}
