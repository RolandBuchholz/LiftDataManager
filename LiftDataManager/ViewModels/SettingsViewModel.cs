using Microsoft.UI.Xaml.Media.Animation;

namespace LiftDataManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IJsonNavigationViewService _jsonNavigationViewService;
    public SettingsViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        _jsonNavigationViewService = jsonNavigationViewService;
    }

    [RelayCommand]
    private void GoToSettingPage(object sender)
    {
        var item = sender as SettingsCard;
        if (item?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{item.Tag}");
            if (pageType != null)
            {
                SlideNavigationTransitionInfo entranceNavigation = new()
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                };
                _jsonNavigationViewService.NavigateTo(pageType, item.Header, false, entranceNavigation);
            }
        }
    }
}