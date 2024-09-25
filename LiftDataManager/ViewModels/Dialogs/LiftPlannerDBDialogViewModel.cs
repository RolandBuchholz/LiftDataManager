using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class LiftPlannerDBDialogViewModel : ObservableObject
{
    private readonly ParameterEditContext _parameterEditContext;
    private readonly ILogger<LiftPlannerDBDialogViewModel> _logger;
    public ObservableCollection<Country>? Countrys { get; set; } = [];

    public LiftPlannerDBDialogViewModel(ILogger<LiftPlannerDBDialogViewModel> logger, ParameterEditContext parameterEditContext)
    {
        _logger = logger;
        _parameterEditContext = parameterEditContext;
    }

    public int LiftPlannerId { get; set; }

    public bool IsCompanyValid { get; set; }
    public bool IsNameValid { get; set; }
    public bool IsStreetValid { get; set; }
    public bool IsMailadressValid { get; set; }

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
            IsCompanyValid = !string.IsNullOrWhiteSpace(Company);
            IsNameValid = !string.IsNullOrWhiteSpace(Name);
            IsStreetValid = !string.IsNullOrWhiteSpace(Street);
            IsMailadressValid = Mailadress.IsEmail();
            CheckLiftplannerIsValid();
        }

        var countrys = _parameterEditContext.Set<Country>().ToArray();
        foreach (var country in countrys)
        {
            Countrys?.Add(country);
        }
        sender.Closed += LiftPlannerDBDialogClosed;
        await Task.CompletedTask;
    }

    private void LiftPlannerDBDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        sender.Closed -= LiftPlannerDBDialogClosed;
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
        CanAddLiftPlannerToDatabase = IsCompanyValid &&
                                      IsNameValid &&            
                                      IsStreetValid &&
                                      IsMailadressValid &&
                                      !string.IsNullOrWhiteSpace(Town);
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
            }
            catch
            {
                _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
            }
            finally 
            { 
                sender.LiftPlannerId = LiftPlannerId;
                var copyResult =  await ProcessHelpers.CopyDataBaseToWorkSpace(_parameterEditContext);
                if (copyResult)
                {
                    _logger.LogInformation(60177, "Copy database successful to lokal workspace");
                }
                else
                {
                    _logger.LogWarning(61075, "Copy database failed to lokal workspace");
                }
                sender.Hide();
            }
        }
        else
        {
            try
            {
                var liftPlannerDatabase = _parameterEditContext.Set<LiftPlanner>().Include(i => i.ZipCode)
                                                                                      .ThenInclude(t => t.Country)
                                                                                      .FirstOrDefault(x => x.Id == LiftPlannerId);
                if (liftPlannerDatabase is not null)
                {
                    liftPlannerDatabase.Company = Company!;
                    liftPlannerDatabase.FirstName = FirstName;
                    liftPlannerDatabase.Name = Name!;
                    liftPlannerDatabase.Street = Street!;
                    liftPlannerDatabase.StreetNumber = StreetNumber;
                    liftPlannerDatabase.PhoneNumber = PhoneNumber;
                    liftPlannerDatabase.MobileNumber = MobileNumber;
                    liftPlannerDatabase.EmailAddress = Mailadress!;
                    liftPlannerDatabase.ZipCode.Country = SelectedCountry!;
                    liftPlannerDatabase.ZipCode.ZipCodeNumber = Convert.ToInt32(ZipCode);
                    liftPlannerDatabase.ZipCode.Name = Town!;
                }
                await _parameterEditContext.SaveChangesAsync();
            }
            catch
            {
                _logger.LogError(61078, "Failed to update Liftplanner {Company}", Company);
            }
            finally
            {
                var copyResult = await ProcessHelpers.CopyDataBaseToWorkSpace(_parameterEditContext);
                if (copyResult)
                {
                    _logger.LogInformation(60177, "Copy database successful to lokal workspace");
                }
                else
                {
                    _logger.LogWarning(61075, "Copy database failed to lokal workspace");
                }
                sender.Hide();
            }
        }
    }
}