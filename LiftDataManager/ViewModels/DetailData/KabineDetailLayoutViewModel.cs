using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailLayoutViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public KabineDetailLayoutViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService) :
     base(parameterDataService, dialogService, infoCenterService, settingService)
    {
    }

    [RelayCommand]
    private static void GoToKabine()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabinePage));
    }

    [ObservableProperty]
    private PivotItem? selectedPivotItem;
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {

                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}