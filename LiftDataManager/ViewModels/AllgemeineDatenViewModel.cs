using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class AllgemeineDatenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public Dictionary<int,string> LiftPlanners { get; set; } = new();
    public ObservableCollection<string?> FilteredLiftPlanners { get; set; } = new();
    public ObservableCollection<Country>? Countrys { get; set; } = new();
    private readonly ILogger<AllgemeineDatenViewModel> _logger;

    private readonly ParameterContext _editableparametercontext;

    public AllgemeineDatenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ILogger<AllgemeineDatenViewModel> logger) :
         base(parameterDataService, dialogService, navigationService)
    {
        DbContextOptionsBuilder editableOptions = new();
        editableOptions.UseSqlite(App.GetConnectionString(false));
        _editableparametercontext = new ParameterContext(editableOptions.Options);
        _logger = logger;
    }

    [ObservableProperty]
    private string? autoSuggestBoxText;

    [ObservableProperty]
    private int zipCodePlaces;

    [ObservableProperty]
    private string? zipCode;
    partial void OnZipCodeChanged(string? value)
    {
        if (value is not null && selectedCountry is not null)
        {
            CanShowTown = ((value.Length >= 4 && selectedCountry.ShortMark != "D") || value.Length == 5); 
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
        if (value && selectedCountry is not null)
        {
            ZipCodePlaces = selectedCountry.ShortMark != "D" ? 4 : 5;
        }
    }

    [ObservableProperty]
    private bool canShowTown;
    partial void OnCanShowTownChanged(bool value)
    {
        if (ZipCode is not null)
        {
            var storedZipCode = _editableparametercontext.Set<ZipCode>().FirstOrDefault(x => x.ZipCodeNumber == Convert.ToInt32(ZipCode));
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
    private string? selectedLiftPlanner;
    partial void OnSelectedLiftPlannerChanged(string? value)
    {
        CanGetLiftPlannerFromDatabase = !string.IsNullOrWhiteSpace(value);
    }

    [ObservableProperty]
    private Country? selectedCountry;
    partial void OnSelectedCountryChanged(Country? value)
    {
        CanShowZipCode = false;
        CanShowTown = false;
        ZipCode = string.Empty;
        Town = string.Empty;
        CanShowZipCode = value is not null;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GetLiftPlannerFromDatabaseCommand))]
    private bool canGetLiftPlannerFromDatabase;

    private void SetLiftplanners()
    {
        var liftPlanners = _editableparametercontext.Set<LiftPlanner>().ToArray();
        foreach (var planner in liftPlanners)
        {
            LiftPlanners?.Add(planner.Id ,$"{planner.Company} ({planner.FirstName} {planner.Name})");
        }
        var countrys = _editableparametercontext.Set<Country>().ToArray();
        foreach (var country in countrys)
        {
            Countrys?.Add(country);
        }
    }

    [RelayCommand]
    private void FilterLiftPlanners(object sender)
    {
        if (sender is null)
            return;
        if (sender is not AutoSuggestBox)
            return;
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
        if (FilteredLiftPlanners.Count == 0)
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

        var liftPlannerDatabase = _editableparametercontext.Set<LiftPlanner>().Include(i => i.ZipCode)
                                                                      .ThenInclude(t => t.Country)
                                                                      .FirstOrDefault(x => x.Id == liftPlanner.Key);

        ParamterDictionary!["var_AnPersonZ4"].Value = SelectedLiftPlanner;
        ParamterDictionary!["var_FP_Adresse"].Value = $"{liftPlannerDatabase?.ZipCode.Country.ShortMark} - {liftPlannerDatabase?.ZipCode.ZipCodeNumber} {liftPlannerDatabase?.ZipCode.Name} {liftPlannerDatabase?.Street} {liftPlannerDatabase?.StreetNumber}";
        ParamterDictionary!["var_AnPersonPhone"].Value = liftPlannerDatabase?.PhoneNumber;
        ParamterDictionary!["var_AnPersonMobil"].Value = liftPlannerDatabase?.MobileNumber;
        ParamterDictionary!["var_AnPersonMail"].Value = liftPlannerDatabase?.EmailAddress;
        AutoSuggestBoxText = string.Empty;
        SelectedLiftPlanner = string.Empty;
    }

    [RelayCommand]
    private async Task AddLiftPlannerDialogAsync(ContentDialog addLiftPlannerDialog)
    {
        await addLiftPlannerDialog.ShowAsync();
    }

    private void CheckLiftplannerIsValid()
    {
        CanAddLiftPlannerToDatabase = !string.IsNullOrWhiteSpace(Company) &&
                                      !string.IsNullOrWhiteSpace(Name) &&
                                      !string.IsNullOrWhiteSpace(Street) &&
                                      !string.IsNullOrWhiteSpace(Town) &&
                                      !string.IsNullOrWhiteSpace(Mailadress);
    }

    [RelayCommand(CanExecute = nameof(CanAddLiftPlannerToDatabase))]
    private async Task AddLiftPlannerToDatabaseAsync()
    {
        try
        {
            ZipCode dBZipCode;

            var storedZipCode = _editableparametercontext.Set<ZipCode>().FirstOrDefault(x => x.ZipCodeNumber == Convert.ToInt32(ZipCode));
            if (storedZipCode is not null)
            {
                dBZipCode = storedZipCode;
            }
            else
            {
                dBZipCode = new ZipCode
                {
                    ZipCodeNumber = Convert.ToInt32(ZipCode),
                    Name = Town!,
                    Country = selectedCountry!
                };
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

            var addedLiftpanner = _editableparametercontext.Add(newLiftplanner);
            await _editableparametercontext.SaveChangesAsync();
            _logger.LogInformation(60179, "Liftplanner: {Company} successfully add to database", addedLiftpanner.DebugView.LongView);
        }
        catch
        {
            _logger.LogError(61078, "Failed to add new Liftplanner {Company} to database", Company);
        }

        //SetLiftplanners();
        // TO DO Close Window
        // Refresh DB Context
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();
        SetLiftplanners();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}