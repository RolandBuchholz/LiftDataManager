﻿using Microsoft.UI.Xaml.Media.Animation;

namespace LiftDataManager.Helpers;
public class LiftParameterNavigationHelper
{
    public static void NavigateToHighlightParameters()
    {
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(typeof(ListenansichtPage), "ShowHighlightParameter");
    }

    public static void NavigateToParameterDetails(string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(typeof(DatenansichtDetailPage), parameterName);
    }

    public static void NavigateToPage(Type page)
    {
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(page);
    }

    public static void NavigateToPage(Type page, object parameter)
    {
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(page, parameter);
    }

    public static void NavigatePivotItem(Type page)
    {
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(page, null, false, new DrillInNavigationTransitionInfo());
    }

    public static void NavigatePivotItem(Type page, object parameter)
    {
        var navigationService = App.GetService<IJsonNavigationService>();
        navigationService.NavigateTo(page, parameter, false, new DrillInNavigationTransitionInfo());
    }
}
