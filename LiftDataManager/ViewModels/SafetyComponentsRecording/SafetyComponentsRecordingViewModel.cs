using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
namespace LiftDataManager.ViewModels;

public partial class SafetyComponentsRecordingViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    public ObservableCollection<SafetyComponentRecord> SafetyComponentsRecordings { get; set; } = [];


    public SafetyComponentsRecordingViewModel(IParameterDataService parameterDataService, SafetyComponentRecordContext safetyComponentRecordContext, IDialogService dialogService, IInfoCenterService infoCenterService,
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

    private async Task GetSafetyComponentsRecordingsFromDatabaseAsync()
    {
        var safetyComponentsRecordings = _safetyComponentRecordContext.SafetyComponentRecords?.Include(i =>i.SafetyComponentManfacturer).Include(i => i.LiftCommission);
        if (safetyComponentsRecordings is not null)
        {
            SafetyComponentsRecordings.AddRange(safetyComponentsRecordings);
        }
        await Task.CompletedTask;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await GetSafetyComponentsRecordingsFromDatabaseAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
        if (string.Equals(SelectedPivotItem?.Tag, "CurrentSafetyComponentsPage") ||
            string.Equals(SelectedPivotItem?.Tag, "SafetyComponentsEquipmentsPage"))
        {
            return;
        }
        if (_safetyComponentRecordContext.Database.GetDbConnection() is SqliteConnection editConn)
        {
            SqliteConnection.ClearPool(editConn);
        }
        _safetyComponentRecordContext.Database.CloseConnection();
    }
}