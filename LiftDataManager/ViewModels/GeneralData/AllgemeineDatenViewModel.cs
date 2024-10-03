using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class AllgemeineDatenViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public Dictionary<int, string> LiftPlanners { get; set; } = [];
    public ObservableCollection<string?> FilteredLiftPlanners { get; set; } = [];
    private readonly ILogger<AllgemeineDatenViewModel> _logger;
    private readonly ParameterContext _parametercontext;

    public AllgemeineDatenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ILogger<AllgemeineDatenViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _logger = logger;
    }

    [ObservableProperty]
    private string? autoSuggestBoxText;


    [ObservableProperty]
    private string? selectedLiftPlanner;
    partial void OnSelectedLiftPlannerChanged(string? value)
    {
        CanGetLiftPlannerFromDatabase = !string.IsNullOrWhiteSpace(value);
        DataBaseAction = string.IsNullOrWhiteSpace(value) ? "Neuen Fachplaner anlegen" : "Fachplaner aktualisieren";
        DataBaseActionDescription = string.IsNullOrWhiteSpace(value) ? "Neuen Fachplaner erstellen und in die Datenbank speichen" : "Vorhandenen Fachplaner in der Datenbank aktualisieren";
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GetLiftPlannerFromDatabaseCommand))]
    private bool canGetLiftPlannerFromDatabase;

    [ObservableProperty]
    private string dataBaseAction = "Neuen Fachplaner anlegen";

    [ObservableProperty]
    private string dataBaseActionDescription = "Neuen Fachplaner erstellen und in die Datenbank speichen";

    private void SetLiftplanners()
    {
        LiftPlanners?.Clear();
        var liftPlanners = _parametercontext.Set<LiftPlanner>();
        foreach (var planner in liftPlanners)
        {
            LiftPlanners?.Add(planner.Id, $"{planner.Company} ({planner.FirstName} {planner.Name})");
        }
    }

    [RelayCommand]
    private void FilterLiftPlanners(object sender)
    {
        if (sender is null)
        {
            return;
        }
        if (sender is not AutoSuggestBox)
        {
            return;
        }
        var splitText = ((AutoSuggestBox)sender).Text.ToLower().Split(" ");
        FilteredLiftPlanners.Clear();
        foreach (var liftplanner in LiftPlanners)
        {
            var found = splitText.All((key) =>
            {
                return liftplanner.Value.ToLower().Contains(key);
            });
            if (found)
            {
                FilteredLiftPlanners?.Add(liftplanner.Value);
            }
        }
        if (FilteredLiftPlanners?.Count == 0)
        {
            FilteredLiftPlanners.Add("Kein Fachplaner gefunden");
        }
    }

    [RelayCommand]
    private void SelectLiftPlanners(string selectedItem)
    {
        SelectedLiftPlanner = selectedItem;
    }

    [RelayCommand(CanExecute = nameof(CanGetLiftPlannerFromDatabase))]
    private void GetLiftPlannerFromDatabase()
    {
        var liftPlanner = LiftPlanners.FirstOrDefault(x => x.Value == SelectedLiftPlanner);

        var liftPlannerDatabase = _parametercontext.Set<LiftPlanner>().Include(i => i.ZipCode)
                                                              .ThenInclude(t => t.Country)
                                                              .FirstOrDefault(x => x.Id == liftPlanner.Key);
        ParameterDictionary["var_AnPersonZ4"].Value = SelectedLiftPlanner;
        ParameterDictionary["var_FP_Adresse"].Value = $"{liftPlannerDatabase?.ZipCode.Country.ShortMark} - {liftPlannerDatabase?.ZipCode.ZipCodeNumber} {liftPlannerDatabase?.ZipCode.Name} {liftPlannerDatabase?.Street} {liftPlannerDatabase?.StreetNumber}";
        ParameterDictionary["var_AnPersonPhone"].Value = liftPlannerDatabase?.PhoneNumber;
        ParameterDictionary["var_AnPersonMobil"].Value = liftPlannerDatabase?.MobileNumber;
        ParameterDictionary["var_AnPersonMail"].Value = liftPlannerDatabase?.EmailAddress;
        AutoSuggestBoxText = string.Empty;
        SelectedLiftPlanner = string.Empty;
    }

    [RelayCommand]
    private void ResetLiftPlanner()
    {
        ParameterDictionary["var_AnPersonZ4"].Value = string.Empty;
        ParameterDictionary["var_FP_Adresse"].Value = string.Empty;
        ParameterDictionary["var_AnPersonPhone"].Value = string.Empty;
        ParameterDictionary["var_AnPersonMobil"].Value = string.Empty;
        ParameterDictionary["var_AnPersonMail"].Value = string.Empty;
        AutoSuggestBoxText = string.Empty;
        SelectedLiftPlanner = string.Empty;
    }

    [RelayCommand]
    private async Task AddLiftPlannerDialogAsync(ContentDialog addLiftPlannerDialog)
    {
        var liftPlannerId = LiftPlanners.FirstOrDefault(x => x.Value == SelectedLiftPlanner).Key;
        var result = await _dialogService.LiftPlannerDBDialogAsync(liftPlannerId);
        if (_parametercontext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        SetLiftplanners();
        if (result > 0)
        {
            if (LiftPlanners.TryGetValue(result, out var liftPlanner))
            {
                SelectedLiftPlanner = liftPlanner;
                GetLiftPlannerFromDatabase();
            };
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        SetLiftplanners();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}