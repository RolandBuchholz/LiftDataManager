using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using LiftDataManager.core.Helpers;
using LiftDataManager.Models;
using static LiftDataManager.Models.TechnicalLiftDocumentation;
using System.Text.Json;
using Humanizer;

namespace LiftDataManager.ViewModels;

public partial class EinreichunterlagenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;

    public EinreichunterlagenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _parametercontext = parametercontext;
        LiftDocumentation = new();
    }

    public TechnicalLiftDocumentation LiftDocumentation { get; set; }

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

    public string DateTimeNow => DateTime.Now.ToShortDateString();

    public string Manufacturer => """
                                    Berchtenbreiter GmbH
                                    Maschinenbau - Aufzugtechnik
                                    Mähderweg 1a
                                    86637 Rieblingen
                                    """;

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
        }
        UpdateProtectedSpaceTyp();
    }

    private void UpdateLiftDocumentation()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        ParameterDictionary!["var_Einreichunterlagen"].Value = JsonSerializer.Serialize(LiftDocumentation, options);
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
