using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class CurrentSafetyComponentsViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly SafetyComponentRecordContext _safetyComponentRecordContext;
    public ObservableCollection<SafetyComponentRecord> ListOfSafetyComponents { get; set; } = [];

    public CurrentSafetyComponentsViewModel(IParameterDataService parameterDataService, SafetyComponentRecordContext safetyComponentRecordContext, IDialogService dialogService, IInfoCenterService infoCenterService,
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
                var pageTitle = "Aktueller Auftrag";
                LiftParameterNavigationHelper.NavigatePivotItem(pageType, pageTitle);
            }
        }
    }

    [ObservableProperty]
    public partial LiftCommission? CurrentLiftCommission { get; set; }
    partial void OnCurrentLiftCommissionChanged(LiftCommission? value) 
    { 
    
    }

    private async Task GetCurrentSafetyComponentsFromDatabaseAsync()
    {
        CurrentLiftCommission = _safetyComponentRecordContext.LiftCommissions?.Where(x => x.Name == SpezifikationsNumber)
                                                                              .Include(i => i.SafetyComponentRecords!)
                                                                              .ThenInclude(t => t.SafetyComponentManfacturer)
                                                                              .FirstOrDefault();
        if (CurrentLiftCommission is not null && CurrentLiftCommission.SafetyComponentRecords is not null)
        {
            ListOfSafetyComponents.AddRange(CurrentLiftCommission.SafetyComponentRecords);
        }
        await Task.CompletedTask;
    }

    public async void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        await GetCurrentSafetyComponentsFromDatabaseAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}