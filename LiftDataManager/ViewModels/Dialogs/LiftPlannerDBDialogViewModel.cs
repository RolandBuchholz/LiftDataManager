using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using LiftDataManager.Core.DataAccessLayer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;

namespace LiftDataManager.ViewModels;

public partial class LiftPlannerDBDialogViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ParameterEditContext _parameterEditContext;
    private readonly ILogger<LiftPlannerDBDialogViewModel> _logger;
    public ObservableCollection<Country>? Countrys { get; set; } = [];
    public LiftPlannerDBDialogViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ILogger<LiftPlannerDBDialogViewModel> logger, ParameterEditContext parameterEditContext) :
     base(parameterDataService, dialogService, infoCenterService)
    {
        _logger = logger;
        _parameterEditContext = parameterEditContext;
    }

    //var countrys = _parametercontext.Set<Country>().ToArray();
    //    foreach (var country in countrys)
    //    {
    //        Countrys?.Add(country);
    //    }


    //if (!string.IsNullOrWhiteSpace(SelectedLiftPlanner))
    //{
    //    if (_editableparametercontext is null)
    //    {
    //        DbContextOptionsBuilder editableOptions = new();
    //        editableOptions.UseSqlite(App.GetConnectionString(false));
    //        _editableparametercontext = new ParameterContext(editableOptions.Options);
    //    }

    //    var liftPlanner = LiftPlanners.FirstOrDefault(x => x.Value == SelectedLiftPlanner);
    //    var liftPlannerDatabase = _editableparametercontext.Set<LiftPlanner>().Include(i => i.ZipCode)
    //                                                                  .ThenInclude(t => t.Country)
    //                                                                  .FirstOrDefault(x => x.Id == liftPlanner.Key);
    //    if (liftPlannerDatabase is not null)
    //    {
    //        Company = liftPlannerDatabase.Company;
    //        FirstName = liftPlannerDatabase.FirstName;
    //        Name = liftPlannerDatabase.Name;
    //        Street = liftPlannerDatabase.Street;
    //        StreetNumber = liftPlannerDatabase.StreetNumber;
    //        SelectedCountry = liftPlannerDatabase.ZipCode.Country;
    //        ZipCode = liftPlannerDatabase.ZipCode.ZipCodeNumber.ToString();
    //        Town = liftPlannerDatabase.ZipCode.Name;
    //        PhoneNumber = liftPlannerDatabase.PhoneNumber;
    //        MobileNumber = liftPlannerDatabase.MobileNumber;
    //        Mailadress = liftPlannerDatabase.EmailAddress;
    //        ZipCode = liftPlannerDatabase.ZipCode.ZipCodeNumber.ToString();
    //    }
    //}
    //await addLiftPlannerDialog.ShowAsyncQueueDraggable();

    //DataBaseAction = string.IsNullOrWhiteSpace(value) ? "Neuen Fachplaner anlegen" : "Fachplaner aktualisieren";
    //DataBaseButtonText = string.IsNullOrWhiteSpace(value) ? "Fachplaner erstellen und speichern" : "Fachplaner in Datenbank aktualisieren";
    //DataBaseActionDescription = string.IsNullOrWhiteSpace(value) ? "Neuen Fachplaner erstellen und in die Datenbank speichen" : "Vorhandenen Fachplaner in der Datenbank aktualisieren";

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
    private async Task AddLiftPlannerToDatabaseAsync(ContentDialog addLiftPlannerDialog)
    {
        //if (string.IsNullOrWhiteSpace(SelectedLiftPlanner))
        //{
        //    try
        //    {
        //        ZipCode dBZipCode;
        //        var storedZipCode = _parameterEditContext.Set<ZipCode>().FirstOrDefault(x => x.ZipCodeNumber == Convert.ToInt32(ZipCode));
        //        if (storedZipCode is not null)
        //        {
        //            dBZipCode = storedZipCode;
        //        }
        //        else
        //        {
        //            if (SelectedCountry is not null)
        //            {
        //                dBZipCode = new ZipCode
        //                {
        //                    ZipCodeNumber = Convert.ToInt32(ZipCode),
        //                    Name = Town!,
        //                    Country = SelectedCountry
        //                };
        //            }
        //            else
        //            {
        //                _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
        //                return;
        //            }
        //        }
        //        var newLiftplanner = new LiftPlanner
        //        {
        //            Company = Company!,
        //            Name = Name!,
        //            FirstName = FirstName,
        //            Street = Street!,
        //            StreetNumber = StreetNumber,
        //            PhoneNumber = PhoneNumber,
        //            MobileNumber = MobileNumber,
        //            EmailAddress = Mailadress!,
        //            ZipCode = dBZipCode,
        //        };
        //        var addedLiftpanner = _parameterEditContext.Update(newLiftplanner);
        //        await _parameterEditContext.SaveChangesAsync();

        //        if (LiftPlanners is not null)
        //        {
        //            LiftPlanners.Add(addedLiftpanner.Entity.Id, $"{addedLiftpanner.Entity.Company} ({addedLiftpanner.Entity.FirstName} {addedLiftpanner.Entity.Name})");
        //            SelectedLiftPlanner = LiftPlanners[addedLiftpanner.Entity.Id];
        //            _logger.LogInformation(60179, "Liftplanner: {Company} successfully add to database", addedLiftpanner.DebugView.LongView);
        //        }
        //    }
        //    catch
        //    {
        //        _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
        //    }

        //    GetLiftPlannerFromDatabase(_parameterEditContext);
        //    Company = string.Empty;
        //    FirstName = string.Empty;
        //    Name = string.Empty;
        //    Street = string.Empty;
        //    StreetNumber = string.Empty;
        //    PhoneNumber = string.Empty;
        //    MobileNumber = string.Empty;
        //    Mailadress = string.Empty;
        //    Town = string.Empty;
        //    ZipCode = null;
        //    addLiftPlannerDialog.Hide();
        //}
        //else
        //{
        //    try
        //    {
        //        var liftPlanner = LiftPlanners.FirstOrDefault(x => x.Value == SelectedLiftPlanner);
        //        var liftPlannerDatabase = _editableparametercontext.Set<LiftPlanner>().Include(i => i.ZipCode)
        //                                                                              .ThenInclude(t => t.Country)
        //                                                                              .FirstOrDefault(x => x.Id == liftPlanner.Key);



        //        if (liftPlannerDatabase is not null)
        //        {
        //            liftPlannerDatabase.Company = Company!;
        //            liftPlannerDatabase.FirstName = FirstName;
        //            liftPlannerDatabase.Name = Name!;
        //            liftPlannerDatabase.Street = Street!;
        //            liftPlannerDatabase.StreetNumber = StreetNumber;
        //            liftPlannerDatabase.PhoneNumber = PhoneNumber;
        //            liftPlannerDatabase.MobileNumber = MobileNumber;
        //            liftPlannerDatabase.EmailAddress = Mailadress!;
        //            liftPlannerDatabase.ZipCode.Country = SelectedCountry!;
        //            liftPlannerDatabase.ZipCode.ZipCodeNumber = Convert.ToInt32(ZipCode);
        //            liftPlannerDatabase.ZipCode.Name = Town!;
        //        }
        //        await _editableparametercontext.SaveChangesAsync();

        //        LiftPlanners[liftPlanner.Key] = $"{Company} ({FirstName} {Name})";
        //        SelectedLiftPlanner = LiftPlanners[liftPlanner.Key];
        //    }
        //    catch
        //    {
        //        _logger.LogError(61078, "Failed to update Liftplanner {Company}", Company);
        //    }
        //    GetLiftPlannerFromDatabase(_editableparametercontext);
        //    Company = string.Empty;
        //    FirstName = string.Empty;
        //    Name = string.Empty;
        //    Street = string.Empty;
        //    StreetNumber = string.Empty;
        //    PhoneNumber = string.Empty;
        //    MobileNumber = string.Empty;
        //    Mailadress = string.Empty;
        //    Town = string.Empty;
        //    ZipCode = null;
        //    addLiftPlannerDialog.Hide();
        //}
    }

    public void OnNavigatedTo(object parameter)
    {
        //NavigatedToBaseActions();
    }

    public void OnNavigatedFrom()
    {
        //NavigatedFromBaseActions();
        if (_parameterEditContext.Database.GetDbConnection() is SqliteConnection conn)
        {
            SqliteConnection.ClearPool(conn);
        }
        _parameterEditContext.Database.CloseConnection();
    }
}