using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class SafetyComponentsEquipmentsViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    public ObservableCollection<LiftCommission> Equipments { get; set; } = [];

    public SafetyComponentsEquipmentsViewModel(IParameterDataService parameterDataService, SafetyComponentRecordContext safetyComponentRecordContext, IDialogService dialogService, IInfoCenterService infoCenterService,
                              ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _safetyComponentRecordContext = safetyComponentRecordContext;
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

    private async Task GetEquipmentsFromDatabaseAsync()
    {
        var liftCommissions = _safetyComponentRecordContext.LiftCommissions?.Include(i => i.SafetyComponentRecords);
        if (liftCommissions is not null)
        {
            Equipments.AddRange(liftCommissions);
        }
        await Task.CompletedTask;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await GetEquipmentsFromDatabaseAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {

    }
}