using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using Microsoft.Data.Sqlite;
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

    [RelayCommand]
    public async Task AddEquipmentAsync()
    {
        var equipmentnumber = await _dialogService.NumberInputDialogAsync("Equipmentdatenbank", "Equipmentnummer eingeben", "Equipmentnummer", 6, 7);
        if (equipmentnumber == default)
        {
            return;
        }

        var liftCommission = _safetyComponentRecordContext.LiftCommissions?.Where(x => x.Name == equipmentnumber.ToString()).FirstOrDefault();
        if (liftCommission != null)
        {
            await EditEquipmentAsync(liftCommission);
        }
        else
        {
            var newLiftCommission = new LiftCommission()
            {
                Name = equipmentnumber.ToString(),
                LiftInstallerID = 1491,
                Country = "DE"
            };
            await _safetyComponentRecordContext.LiftCommissions!.AddAsync(newLiftCommission);
            await _safetyComponentRecordContext.SaveChangesAsync();
            await EditEquipmentAsync(newLiftCommission);
        }
    }

    [RelayCommand]
    public async Task RemoveEquipmentAsync()
    {
        var equipmentnumber = await _dialogService.NumberInputDialogAsync("Equipment aus Datenbank entfernen", "Equipmentnummer eingeben", "Equipmentnummer", 6, 7);
        if (equipmentnumber == default)
        {
            return;
        }

        var liftCommission = _safetyComponentRecordContext.LiftCommissions?.Where(x => x.Name == equipmentnumber.ToString()).FirstOrDefault();
        if (liftCommission != null)
        {
            var result = await _dialogService.WarningDialogAsync("Equipment aus Datenbank entfernen", $"{equipmentnumber} endgültig löschen!", "Löschen", "Abbrechen");
            if (result.Value)
            {
                _safetyComponentRecordContext.LiftCommissions!.Remove(liftCommission);
                Equipments.Remove(liftCommission);
                await _safetyComponentRecordContext.SaveChangesAsync();
            }
            else
            {
                return;
            }
        }
        else
        {
            await _dialogService.MessageDialogAsync("Equipment aus Datenbank entfernen", $"Kein Auftrag mit der Auftragsnummer: {equipmentnumber} gefunden.", "Abbrechen");
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task EditEquipmentAsync(object sender)
    {
        if (sender is LiftCommission liftCommission)
        {
            LiftParameterNavigationHelper.NavigatePivotItem(typeof(CurrentSafetyComponentsPage), liftCommission);
        }
        await Task.CompletedTask;
    }

    public static Visibility LiftCommissionIsComplete(IEnumerable<SafetyComponentRecord> listOfSafetyComponentRecords)
    {
        if (!listOfSafetyComponentRecords.Any())
        {
            return Visibility.Collapsed;
        }
        if (listOfSafetyComponentRecords.Any(x => !x.CompleteRecord))
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
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
        NavigatedFromBaseActions();
        if (Equals(SelectedPivotItem?.Tag, "CurrentSafetyComponentsPage") ||
            Equals(SelectedPivotItem?.Tag, "SafetyComponentsRecordingPage"))
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