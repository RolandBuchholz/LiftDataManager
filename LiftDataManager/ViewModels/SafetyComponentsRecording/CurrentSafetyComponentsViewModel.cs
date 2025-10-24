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
                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
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

    private async Task SetDefaultDataAsync() 
    {
        if (string.IsNullOrWhiteSpace(SpezifikationsNumber))
        {
            return;
        }
        var saisNumber = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_SAISEquipment");
        if (CurrentLiftCommission is null)
        {
            CurrentLiftCommission = new LiftCommission()
            {
                Name = SpezifikationsNumber,
                LiftInstallerID = 1491,
                SAISEquipment = saisNumber,
                Country = "DE",
                SafetyComponentRecords = []
            };
            await _safetyComponentRecordContext.LiftCommissions!.AddAsync(CurrentLiftCommission);
            await _safetyComponentRecordContext.SaveChangesAsync();
        }
   
        if (string.IsNullOrWhiteSpace(saisNumber) &&
            !string.IsNullOrWhiteSpace(CurrentLiftCommission.SAISEquipment))
        {
            ParameterDictionary["var_SAISEquipment"].Value = CurrentLiftCommission.SAISEquipment;
        }
        await Task.CompletedTask;
    }

    public async void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        await GetCurrentSafetyComponentsFromDatabaseAsync();
        await SetDefaultDataAsync();
        await Task.CompletedTask;
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}