using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class LiftPlannerDBDialogViewModel : DataViewModelBase, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    const string workPathDb = @"C:\Work\Administration\DataBase\LiftDataParameter.db";
    private readonly ParameterEditContext _parameterEditContext;
    private readonly ILogger<LiftPlannerDBDialogViewModel> _logger;
    public ObservableCollection<Country>? Countrys { get; set; } = [];

    public LiftPlannerDBDialogViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
        ILogger<LiftPlannerDBDialogViewModel> logger, ParameterEditContext parameterEditContext) :
        base(parameterDataService, dialogService, infoCenterService)
    {
        _logger = logger;
        _parameterEditContext = parameterEditContext;
    }

    public int LiftPlannerId { get; set; }

    [RelayCommand]
    public async Task LiftPlannerDialogLoadedAsync(LiftPlannerDBDialog sender)
    {
        LiftPlannerId = sender.LiftPlannerId;
        if (LiftPlannerId < 1)
        {
            DataBaseAction = "Neuen Fachplaner anlegen";
            DataBaseButtonText = "Fachplaner erstellen und speichern";
            DataBaseActionDescription = "Neuen Fachplaner erstellen und in die Datenbank speichen";
        }
        else
        {
            DataBaseAction = "Fachplaner aktualisieren";
            DataBaseButtonText = "Fachplaner in Datenbank aktualisieren";
            DataBaseActionDescription = "Vorhandenen Fachplaner in der Datenbank aktualisieren";

            var liftPlannerDatabase = _parameterEditContext.Set<LiftPlanner>().Include(i => i.ZipCode)
                                                                              .ThenInclude(t => t.Country)
                                                                              .FirstOrDefault(x => x.Id == LiftPlannerId);
            if (liftPlannerDatabase is not null)
            {
                Company = liftPlannerDatabase.Company;
                FirstName = liftPlannerDatabase.FirstName;
                Name = liftPlannerDatabase.Name;
                Street = liftPlannerDatabase.Street;
                StreetNumber = liftPlannerDatabase.StreetNumber;
                SelectedCountry = liftPlannerDatabase.ZipCode.Country;
                ZipCode = liftPlannerDatabase.ZipCode.ZipCodeNumber.ToString();
                Town = liftPlannerDatabase.ZipCode.Name;
                PhoneNumber = liftPlannerDatabase.PhoneNumber;
                MobileNumber = liftPlannerDatabase.MobileNumber;
                Mailadress = liftPlannerDatabase.EmailAddress;
                ZipCode = liftPlannerDatabase.ZipCode.ZipCodeNumber.ToString();
            }
        }

        var countrys = _parameterEditContext.Set<Country>().ToArray();
        foreach (var country in countrys)
        {
            Countrys?.Add(country);
        }

        NavigatedToBaseActions();
        sender.Closed += LiftPlannerDBDialogClosed;
        await Task.CompletedTask;
    }

    private void LiftPlannerDBDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        sender.Closed -= LiftPlannerDBDialogClosed;
        NavigatedFromBaseActions();
        if (_parameterEditContext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        _parameterEditContext.Database.CloseConnection();
    }

    [ObservableProperty]
    private string dataBaseAction = "Neuen Fachplaner anlegen";

    [ObservableProperty]
    private string dataBaseActionDescription = "Neuen Fachplaner erstellen und in die Datenbank speichen";

    [ObservableProperty]
    private string dataBaseButtonText = "Fachplaner erstellen und speichern";

    private void CheckLiftplannerIsValid()
    {
        CanAddLiftPlannerToDatabase = !string.IsNullOrWhiteSpace(Company) &&
                                      !string.IsNullOrWhiteSpace(Name) &&
                                      !string.IsNullOrWhiteSpace(Street) &&
                                      !string.IsNullOrWhiteSpace(Town) &&
                                      !string.IsNullOrWhiteSpace(Mailadress);
    }

    [ObservableProperty]
    private int zipCodePlaces;

    [ObservableProperty]
    private string? zipCode;
    partial void OnZipCodeChanged(string? value)
    {
        if (value is not null && SelectedCountry is not null)
        {
            CanShowTown = ((value.Length >= 4 && SelectedCountry.ShortMark != "D") || value.Length == 5);
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddLiftPlannerToDatabaseCommand))]
    private bool canAddLiftPlannerToDatabase;

    [ObservableProperty]
    private string? company;
    partial void OnCompanyChanged(string? value)
    {
        CheckLiftplannerIsValid();
    }

    [ObservableProperty]
    private string? name;
    partial void OnNameChanged(string? value)
    {
        CheckLiftplannerIsValid();
    }

    [ObservableProperty]
    private string? firstName;

    [ObservableProperty]
    private string? street;
    partial void OnStreetChanged(string? value)
    {
        CheckLiftplannerIsValid();
    }

    [ObservableProperty]
    private string? streetNumber;

    [ObservableProperty]
    private string? town;
    partial void OnTownChanged(string? value)
    {
        CheckLiftplannerIsValid();
    }

    [ObservableProperty]
    private bool canShowZipCode;
    partial void OnCanShowZipCodeChanged(bool value)
    {
        if (value && SelectedCountry is not null)
        {
            ZipCodePlaces = SelectedCountry.ShortMark != "D" ? 4 : 5;
        }
    }

    [ObservableProperty]
    private bool canShowTown;
    partial void OnCanShowTownChanged(bool value)
    {
        if (!string.IsNullOrWhiteSpace(ZipCode))
        {
            var storedZipCode = _parameterEditContext.Set<ZipCode>().FirstOrDefault(x => x.ZipCodeNumber == Convert.ToInt32(ZipCode));
            if (storedZipCode is not null)
            {
                TownIsInDataBase = true;
                Town = storedZipCode.Name;
            }
            else
            {
                TownIsInDataBase = false;
                Town = string.Empty;
            }
        }
    }

    [ObservableProperty]
    private bool townIsInDataBase;

    [ObservableProperty]
    private string? phoneNumber;

    [ObservableProperty]
    private string? mobileNumber;

    [ObservableProperty]
    private string? mailadress;
    partial void OnMailadressChanged(string? value)
    {
        CheckLiftplannerIsValid();
    }

    [ObservableProperty]
    private Country? selectedCountry;
    partial void OnSelectedCountryChanged(Country? oldValue, Country? newValue)
    {
        if (newValue is null && oldValue is not null)
        {
            SelectedCountry = oldValue;
        }
        else
        {
            CanShowZipCode = false;
            CanShowTown = false;
            ZipCode = string.Empty;
            Town = string.Empty;
            CanShowZipCode = newValue is not null;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddLiftPlannerToDatabase))]
    private async Task AddLiftPlannerToDatabaseAsync(LiftPlannerDBDialog sender)
    {
        if (LiftPlannerId < 1)
        {
            try
            {
                ZipCode dBZipCode;
                var storedZipCode = _parameterEditContext.Set<ZipCode>().FirstOrDefault(x => x.ZipCodeNumber == Convert.ToInt32(ZipCode));
                if (storedZipCode is not null)
                {
                    dBZipCode = storedZipCode;
                }
                else
                {
                    if (SelectedCountry is not null)
                    {
                        dBZipCode = new ZipCode
                        {
                            ZipCodeNumber = Convert.ToInt32(ZipCode),
                            Name = Town!,
                            Country = SelectedCountry
                        };
                    }
                    else
                    {
                        _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
                        return;
                    }
                }
                var newLiftplanner = new LiftPlanner
                {
                    Company = Company!,
                    Name = Name!,
                    FirstName = FirstName,
                    Street = Street!,
                    StreetNumber = StreetNumber,
                    PhoneNumber = PhoneNumber,
                    MobileNumber = MobileNumber,
                    EmailAddress = Mailadress!,
                    ZipCode = dBZipCode,
                };
                var addedLiftpanner = _parameterEditContext.Update(newLiftplanner);
                await _parameterEditContext.SaveChangesAsync();
                _logger.LogInformation(60179, "Liftplanner: {Company} successfully add to database", addedLiftpanner.DebugView.LongView);

                LiftPlannerId = addedLiftpanner.Entity.Id;

                if (newLiftplanner != null)
                {
                    SetLiftPlanner(newLiftplanner);
                }

                //if (LiftPlanners is not null)
                //{
                //    LiftPlanners.Add(addedLiftpanner.Entity.Id, $"{addedLiftpanner.Entity.Company} ({addedLiftpanner.Entity.FirstName} {addedLiftpanner.Entity.Name})");
                //    SelectedLiftPlanner = LiftPlanners[addedLiftpanner.Entity.Id];
                //}
            }
            catch
            {
                _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
            }
            finally 
            { 
                sender.LiftPlannerId = LiftPlannerId;
                await CopyDataBaseToWorkSpace();
                sender.Hide();
            }
        }
        else
        {
            try
            {
                //var liftPlanner = LiftPlanners.FirstOrDefault(x => x.Value == SelectedLiftPlanner);
                //var liftPlannerDatabase = _editableparametercontext.Set<LiftPlanner>().Include(i => i.ZipCode)
                //                                                                      .ThenInclude(t => t.Country)
                //                                                                      .FirstOrDefault(x => x.Id == liftPlanner.Key);



                //if (liftPlannerDatabase is not null)
                //{
                //    liftPlannerDatabase.Company = Company!;
                //    liftPlannerDatabase.FirstName = FirstName;
                //    liftPlannerDatabase.Name = Name!;
                //    liftPlannerDatabase.Street = Street!;
                //    liftPlannerDatabase.StreetNumber = StreetNumber;
                //    liftPlannerDatabase.PhoneNumber = PhoneNumber;
                //    liftPlannerDatabase.MobileNumber = MobileNumber;
                //    liftPlannerDatabase.EmailAddress = Mailadress!;
                //    liftPlannerDatabase.ZipCode.Country = SelectedCountry!;
                //    liftPlannerDatabase.ZipCode.ZipCodeNumber = Convert.ToInt32(ZipCode);
                //    liftPlannerDatabase.ZipCode.Name = Town!;
                //}
                //await _editableparametercontext.SaveChangesAsync();

                //LiftPlanners[liftPlanner.Key] = $"{Company} ({FirstName} {Name})";
                //SelectedLiftPlanner = LiftPlanners[liftPlanner.Key];
            }
            catch
            {
                _logger.LogError(61078, "Failed to update Liftplanner {Company}", Company);
            }
            //GetLiftPlannerFromDatabase(_editableparametercontext);
            //Company = string.Empty;
            //FirstName = string.Empty;
            //Name = string.Empty;
            //Street = string.Empty;
            //StreetNumber = string.Empty;
            //PhoneNumber = string.Empty;
            //MobileNumber = string.Empty;
            //Mailadress = string.Empty;
            //Town = string.Empty;
            //ZipCode = null;
            //addLiftPlannerDialog.Hide();
            finally
            {
                sender.LiftPlannerId = LiftPlannerId;
                sender.Hide();
            }
        }

        

    }

    private void SetLiftPlanner(LiftPlanner liftPlanner)
    {
        //ParameterDictionary["var_AnPersonZ4"].Value = SelectedLiftPlanner;
        ParameterDictionary["var_FP_Adresse"].Value = $"{liftPlanner.ZipCode.Country.ShortMark} - {liftPlanner.ZipCode.ZipCodeNumber} {liftPlanner.ZipCode.Name} {liftPlanner.Street} {liftPlanner.StreetNumber}";
        ParameterDictionary["var_AnPersonPhone"].Value = liftPlanner.PhoneNumber;
        ParameterDictionary["var_AnPersonMobil"].Value = liftPlanner.MobileNumber;
        ParameterDictionary["var_AnPersonMail"].Value = liftPlanner.EmailAddress;
    }

    private async Task CopyDataBaseToWorkSpace()
    {
        var connectionString = _parameterEditContext.Database.GetConnectionString();
        var dbPath = connectionString?[13..connectionString.LastIndexOf('"')];

        if (_parameterEditContext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        _parameterEditContext.Database.CloseConnection();
        await Task.Delay(1500);
        if (!string.IsNullOrWhiteSpace(dbPath))
        {
            File.Copy(dbPath, workPathDb, true);
        }
        await Task.CompletedTask;
    }
}