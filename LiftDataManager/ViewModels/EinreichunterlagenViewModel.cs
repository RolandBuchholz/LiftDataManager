using CommunityToolkit.Mvvm.Messaging.Messages;
using Humanizer;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using LiftDataManager.Models;
using System.Text.Json;
using static LiftDataManager.Models.TechnicalLiftDocumentation;

namespace LiftDataManager.ViewModels;

public partial class EinreichunterlagenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public EinreichunterlagenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                                       ICalculationsModule calculationsModuleService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        LiftDocumentation = new();
    }

    public TechnicalLiftDocumentation LiftDocumentation { get; set; }
    public string DriveTyp => _calculationsModuleService.GetDriveTyp(ParameterDictionary?["var_Getriebe"].Value, LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AufhaengungsartRope"));
    public string DriveControl => _calculationsModuleService.GetDriveControl(ParameterDictionary?["var_Aggregat"].Value);
    public string DrivePosition => _calculationsModuleService.GetDrivePosition(ParameterDictionary?["var_Maschinenraum"].Value);
    public int CarDoorCount => _calculationsModuleService.GetNumberOfCardoors(ParameterDictionary);

    [ObservableProperty]
    private int manufactureYear;
    partial void OnManufactureYearChanged(int value)
    {
        LiftDocumentation.ManufactureYear = value;
        UpdateLiftDocumentation();
    }

    [ObservableProperty]
    private int yearOfConstruction;
    partial void OnYearOfConstructionChanged(int value)
    {
        LiftDocumentation.YearOfConstruction = value;
        UpdateLiftDocumentation();
    }

    [ObservableProperty]
    private string? monthOfConstruction;
    partial void OnMonthOfConstructionChanged(string? value)
    {
        Month month;
        if (Enum.TryParse(value, out month))
        {
            LiftDocumentation.MonthOfConstruction = month;
            UpdateLiftDocumentation();
        }
    }

    [ObservableProperty]
    private string? protectedSpaceTypPit;
    partial void OnProtectedSpaceTypPitChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0")
            return;
        ProtectedSpaceTyp protectedSpace;
        if (Enum.TryParse(value?[..4], out protectedSpace))
        {
            LiftDocumentation.ProtectedSpaceTypPit = protectedSpace;
            UpdateLiftDocumentation();
            UpdateProtectedSpaceTyp();
        }
    }

    [ObservableProperty]
    private string? protectedSpaceTypHead;
    partial void OnProtectedSpaceTypHeadChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0")
            return;
        ProtectedSpaceTyp protectedSpace;
        if (Enum.TryParse(value?[..4], out protectedSpace))
        {
            LiftDocumentation.ProtectedSpaceTypHead = protectedSpace;
            UpdateLiftDocumentation();
            UpdateProtectedSpaceTyp();
        }
    }

    [ObservableProperty]
    private string protectedSpaceTypPitImage = "/Images/NoImage.png";

    [ObservableProperty]
    private string protectedSpaceTypPitDescription = "Kein Schutzraum gewählt";

    [ObservableProperty]
    private string protectedSpaceTypHeadImage = "/Images/NoImage.png";

    [ObservableProperty]
    private string protectedSpaceTypHeadDescription = "Kein Schutzraum gewählt";

    [ObservableProperty]
    private double safetySpacePit;
    partial void OnSafetySpacePitChanged(double value)
    {
        LiftDocumentation.SafetySpacePit = value;
        UpdateLiftDocumentation();
    }

    [ObservableProperty]
    private double safetySpaceHead;
    partial void OnSafetySpaceHeadChanged(double value)
    {
        LiftDocumentation.SafetySpaceHead = value;
        UpdateLiftDocumentation();
    }

    public string DateTimeNow => DateTime.Now.ToShortDateString();

    public string Manufacturer => """
                                    Berchtenbreiter GmbH
                                    Maschinenbau - Aufzugtechnik
                                    Mähderweg 1a
                                    86637 Rieblingen
                                    """;

    public string ViewingOpening => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungKT") || LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungST") ? 
                                    "Schauöffnungen (aus 10 mm dickem VSG - Glas) in den Fahr/Schachttüren vorhanden." : 
                                    "Schauöffnungen in den Fahr/Schachttüren - nicht vorhanden.";
    public string LiftType
    {
        get
        {
            string aufzugstyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Aufzugstyp");

            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == aufzugstyp);
            return cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
        }
    }

    private void SetLiftDocumentation()
    {
        var liftDocumentation = ParameterDictionary?["var_Einreichunterlagen"].Value;
        if (!string.IsNullOrWhiteSpace(liftDocumentation))
        {
            var liftdoku = JsonSerializer.Deserialize<TechnicalLiftDocumentation>(liftDocumentation);
            if (liftdoku is not null)
            {
                LiftDocumentation = liftdoku;
            }
            ManufactureYear = LiftDocumentation.ManufactureYear;
            YearOfConstruction = LiftDocumentation.YearOfConstruction;
            MonthOfConstruction = LiftDocumentation.MonthOfConstruction.ToString();
            ProtectedSpaceTypPit = LiftDocumentation.ProtectedSpaceTypPit.Humanize();
            ProtectedSpaceTypHead = LiftDocumentation.ProtectedSpaceTypHead.Humanize();
            SafetySpacePit = LiftDocumentation.SafetySpacePit;
            SafetySpaceHead = LiftDocumentation.SafetySpaceHead;
        }
        UpdateProtectedSpaceTyp();
    }

    private void UpdateLiftDocumentation()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        ParameterDictionary!["var_Einreichunterlagen"].Value = JsonSerializer.Serialize(LiftDocumentation, options).Replace("\r\n", "\n");
    }

    private void UpdateProtectedSpaceTyp()
    {
        ProtectedSpaceTypPitImage = GetProtectedSpaceTypImage(protectedSpaceTypPit);
        ProtectedSpaceTypPitDescription = GetProtectedSpaceTypDescription(protectedSpaceTypPit);
        ProtectedSpaceTypHeadImage = GetProtectedSpaceTypImage(protectedSpaceTypHead);
        ProtectedSpaceTypHeadDescription = GetProtectedSpaceTypDescription(protectedSpaceTypHead);
    }

    private string GetProtectedSpaceTypImage(string? protectedSpace)
    {
        return protectedSpace switch
        {
            "Typ1 (Aufrecht)" => "/Images/TechnicalDocumentation/protectionRoomTyp1.png",
            "Typ2 (Hockend)" => "/Images/TechnicalDocumentation/protectionRoomTyp2.png",
            "Typ3 (Liegend)" => "/Images/TechnicalDocumentation/protectionRoomTyp3.png",
            _ => "/Images/NoImage.png",
        };
    }

    private string GetProtectedSpaceTypDescription(string? protectedSpace)
    {
        return protectedSpace switch
        {
            "Typ1 (Aufrecht)" => "Aufrecht 0,40 m x 0,50 m x 2,00 m",
            "Typ2 (Hockend)" => "Hockend 0,50 m x 0,70 m x 1,00 m ",
            "Typ3 (Liegend)" => "Liegend 0,70 m x 1,00 m x 0,50 m",
            _ => "Kein Schutzraum gewählt",
        };
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
            _ = SetModelStateAsync();
        SetLiftDocumentation();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
