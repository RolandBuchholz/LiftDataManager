using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailCeilingViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public KabineDetailCeilingViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                                        ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
     base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    [RelayCommand]
    private static void GoToKabine()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabinePage));
    }

    [ObservableProperty]
    public partial PivotItem? SelectedPivotItem { get; set; }
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