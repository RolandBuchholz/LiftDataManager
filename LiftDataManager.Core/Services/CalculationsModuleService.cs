using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.ComponentModels;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace LiftDataManager.Core.Services;

public partial class CalculationsModuleService : ICalculationsModule
{
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<CalculationsModuleService> _logger;

    public required Dictionary<int, TableRow<int, double>> Table6 { get; set; }
    public required Dictionary<int, TableRow<int, double>> Table7 { get; set; }
    public required Dictionary<int, TableRow<int, double>> Table8 { get; set; }

    private double kabinenbreite;
    private double kabinentiefe;
    bool zugangA;
    bool zugangB;
    bool zugangC;
    bool zugangD;
    int anzahlKabinentueren;
    double tuerbreite;
    double tuerbreiteB;
    double tuerbreiteC;
    double tuerbreiteD;
    double tuerhoehe;
    double halsL1;
    double halsR1;
    double halsL2;
    double halsR2;
    double halsL3;
    double halsR3;
    double halsL4;
    double halsR4;

    public CalculationsModuleService(ParameterContext parametercontext, ILogger<CalculationsModuleService> logger)
    {
        _parametercontext = parametercontext;
        _logger = logger;

        InitializeTableData();
    }

    private void InitializeTableData()
    {
        try
        {
            var loadTable6 = _parametercontext.Set<LoadTable6>().ToArray();
            var loadTable7 = _parametercontext.Set<LoadTable7>().ToArray();
            var personsTable8 = _parametercontext.Set<PersonsTable8>().ToArray();
            Table6 = SetTableData(loadTable6, "kg", "m²");
            Table7 = SetTableData(loadTable7, "kg", "m²");
            Table8 = SetTableData(personsTable8, "Pers.", "m²");
        }
        catch (Exception)
        {
            _logger.LogError(61151, "InitializeTableData failed");
        }
    }

    public bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem)
    {
        var loadTable6 = GetLoadFromTable(area, "Tabelle6");
        var loadTable7 = GetLoadFromTable(area, "Tabelle7");

        if (load >= loadTable6)
            return true;
        if (cargotyp == "Lastenaufzug" && drivesystem == "Hydraulik")
        {
            if (loadTable7 > 0)
            {
                LogTabledata("Tabelle 7", load);
                return load >= loadTable7;
            }
            else
            {
                LogTabledata("Tabelle 6", load);
                return load >= loadTable6;
            }
        }
        return false;
    }

    public int GetMaxFuse(string? inverter)
    {
        int maxFuse = 0;
        if (!string.IsNullOrWhiteSpace(inverter))
        {
            var inverterSize = inverter.Replace(" ", "")[8..11];
            var inverterType = _parametercontext.Set<LiftInverterType>().FirstOrDefault(x => x.Name.Contains(inverterSize));
            maxFuse = inverterType is not null ? inverterType.MaxFuseSize : 0;
        }
        return maxFuse;
    }

    public CarVentilationResult GetCarVentilationCalculation(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        const int tuerspalt = 4;
        const int luftspaltoeffnung = 10;
        
        SetDefaultParameter(parameterDictionary);

        double aKabine = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_A_Kabine");
        int anzahlKabinentuerfluegel = LiftParameterHelper.GetLiftParameterValue<int>(parameterDictionary, "var_AnzahlTuerfluegel");
        bool lueftungUnten = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_LueftungSchottenUnten");

        double aKabine1Pozent = Math.Round(aKabine * 10000);
        double belueftung1Seite = kabinentiefe * luftspaltoeffnung;
        double belueftung2Seiten = kabinentiefe * luftspaltoeffnung * 2;

        bool ergebnisBelueftungDecke = belueftung2Seiten > aKabine1Pozent;

        int anzahlLuftspaltoeffnungenTB = anzahlKabinentueren * 2;
        int anzahlLuftspaltoeffnungenTH = anzahlKabinentueren * anzahlKabinentuerfluegel;
        double flaecheLuftspaltoeffnungenTB = anzahlLuftspaltoeffnungenTB * tuerspalt * tuerbreite;
        double flaecheLuftspaltoeffnungenTH = anzahlLuftspaltoeffnungenTH * tuerspalt * tuerhoehe;
        double entlueftungTuerspalten50Pozent = (flaecheLuftspaltoeffnungenTB + flaecheLuftspaltoeffnungenTH) * 0.5;

        int anzahlLuftspaltoeffnungenFB = 0;
        int anzahlLuftspaltoeffnungenFT = 0;

        if (lueftungUnten)
        {
            anzahlLuftspaltoeffnungenFB = 2 - Convert.ToInt32(zugangA) - Convert.ToInt32(zugangC);
            anzahlLuftspaltoeffnungenFT = 2 - Convert.ToInt32(zugangB) - Convert.ToInt32(zugangD);
        }

        double flaecheLuftspaltoeffnungenFB = Math.Round(kabinenbreite / 50 * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
        double flaecheLuftspaltoeffnungenFT = Math.Round(kabinentiefe / 50 * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
        double flaecheLuftspaltoeffnungenFBGesamt = anzahlLuftspaltoeffnungenFB * flaecheLuftspaltoeffnungenFB;
        double flaecheLuftspaltoeffnungenFTGesamt = anzahlLuftspaltoeffnungenFT * flaecheLuftspaltoeffnungenFT;

        double flaecheEntLueftungSockelleisten = Math.Round(flaecheLuftspaltoeffnungenFBGesamt + flaecheLuftspaltoeffnungenFTGesamt);
        double flaecheEntLueftungGesamt = Math.Round(flaecheEntLueftungSockelleisten + entlueftungTuerspalten50Pozent);

        bool ergebnisEntlueftung = flaecheEntLueftungGesamt > aKabine1Pozent;

        return new CarVentilationResult()
        {
            Tuerspalt = tuerspalt,
            Luftspaltoeffnung = luftspaltoeffnung,
            AnzahlLuftspaltoeffnungenFT = anzahlLuftspaltoeffnungenFT,
            AnzahlKabinentueren = anzahlKabinentueren,
            AKabine1Pozent = aKabine1Pozent,
            Belueftung1Seite = belueftung1Seite,
            Belueftung2Seiten = belueftung2Seiten,
            ErgebnisBelueftungDecke = ergebnisBelueftungDecke,
            AnzahlLuftspaltoeffnungenTB = anzahlLuftspaltoeffnungenTB,
            AnzahlLuftspaltoeffnungenTH = anzahlLuftspaltoeffnungenTH,
            FlaecheLuftspaltoeffnungenTB = flaecheLuftspaltoeffnungenTB,
            FlaecheLuftspaltoeffnungenTH = flaecheLuftspaltoeffnungenTH,
            EntlueftungTuerspalten50Pozent = entlueftungTuerspalten50Pozent,
            AnzahlLuftspaltoeffnungenFB = anzahlLuftspaltoeffnungenFB,
            FlaecheLuftspaltoeffnungenFB = flaecheLuftspaltoeffnungenFB,
            FlaecheLuftspaltoeffnungenFT = flaecheLuftspaltoeffnungenFT,
            FlaecheLuftspaltoeffnungenFBGesamt = flaecheLuftspaltoeffnungenFBGesamt,
            FlaecheLuftspaltoeffnungenFTGesamt = flaecheLuftspaltoeffnungenFTGesamt,
            FlaecheEntLueftungSockelleisten = flaecheEntLueftungSockelleisten,
            FlaecheEntLueftungGesamt = flaecheEntLueftungGesamt,
            ErgebnisEntlueftung = ergebnisEntlueftung,
        };
    }

    public PayLoadResult GetPayLoadCalculation(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        string aufzugstyp = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Aufzugstyp");

        var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                        .ToList()
                                                        .FirstOrDefault(x => x.Name == aufzugstyp);
        var cargoTyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";

        var driveSystemDB = _parametercontext.Set<LiftType>().Include(i => i.DriveType)
                                                           .ToList()
                                                           .FirstOrDefault(x => x.Name == aufzugstyp);
        var driveSystem = driveSystemDB is not null ? driveSystemDB.DriveType!.Name! : "";

        SetDefaultParameter(parameterDictionary);

        ClearSelectedRows(Table6);
        ClearSelectedRows(Table7);
        ClearSelectedRows(Table8);

        var nutzflaecheKabine = Math.Round(kabinenbreite * kabinentiefe / Math.Pow(10, 6), 2);
        var nutzflaecheZugangA = zugangA ? GetCarDoorArea(parameterDictionary, "A") : 0;
        var nutzflaecheZugangB = zugangB ? GetCarDoorArea(parameterDictionary, "B") : 0;
        var nutzflaecheZugangC = zugangC ? GetCarDoorArea(parameterDictionary, "C") : 0;
        var nutzflaecheZugangD = zugangD ? GetCarDoorArea(parameterDictionary, "D") : 0;

        var nutzflaeche = Math.Round(nutzflaecheKabine + nutzflaecheZugangA + nutzflaecheZugangB + nutzflaecheZugangC + nutzflaecheZugangD, 2);
        var nennlast = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Q");

        var nennlast6 = Math.Round(GetLoadFromTable(nutzflaeche, "Tabelle6"), 1);
        var nennlast7 = Math.Round(GetLoadFromTable(nutzflaeche, "Tabelle7"), 1);

        var payloadAllowed = ValdidateLiftLoad(nennlast, nutzflaeche, cargoTyp, driveSystem);

        var personen75kg = (int)(nennlast / 75);
        var personenFlaeche = GetPersonenCarArea(nutzflaeche);
        var personenBerechnet = (personen75kg > personenFlaeche) ? personenFlaeche : personen75kg;

        return new PayLoadResult()
        {
            CargoTyp = cargoTyp,
            DriveSystem = driveSystem,
            ZugangA = zugangA,
            ZugangB = zugangB,
            ZugangC = zugangC,
            ZugangD = zugangD,
            AnzahlKabinentueren = anzahlKabinentueren,
            NutzflaecheKabine = nutzflaecheKabine,
            NutzflaecheZugangA = nutzflaecheZugangA,
            NutzflaecheZugangB = nutzflaecheZugangB,
            NutzflaecheZugangC = nutzflaecheZugangC,
            NutzflaecheZugangD = nutzflaecheZugangD,
            NutzflaecheGesamt = nutzflaeche,
            NennLastTabelle6 = nennlast6,
            NennLastTabelle7 = nennlast7,
            PayloadAllowed = payloadAllowed,
            Personen75kg = personen75kg,
            PersonenFlaeche = personenFlaeche,
            PersonenBerechnet = personenBerechnet
        };
    }

    public CarWeightResult GetCarWeightCalculation(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        const double gewichtDecke = 42.6;
        const double abgehaengteDeckeGewichtproQm = 24;
        const double deckeBelegtGewichtproQm = 12;
        const double spiegelGewichtproQm = 6 * 2.5;
        const double paneeleSpiegelGewichtproQm = 6 * 2.5; // 1.5 * 2.7 + 0.85 * 10 + 4 * 2.5
        const double aussenVerkleidungGewichtproQm = 12;
        const double gewichtKlemmkasten = 10;
        const double gewichtSchraubenZubehoer = 5;

        SetDefaultParameter(parameterDictionary);

        //ParameterDictionary
        double kabinenKorrekturGewicht = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_F_Korr");
        bool abgehaengteDecke = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_abgDecke");
        bool belegteDecke = ((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Decke")).StartsWith("Sichtseite belegt");
        double kabineundAbgehaengteDeckeHoehe = abgehaengteDecke ? LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KHLicht") + 50 :
                                                                LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KHLicht");
        double kabinenhoeheAussen = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KHA");
        bool belagAufDerDecke = !(((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_BelagAufDemKabinendach")).StartsWith("kein") ||
                               string.IsNullOrEmpty((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_BelagAufDemKabinendach")));
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_Variable_Tuerdaten");
        string oeffnungsrichtung = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Tueroeffnung");
        int anzahlKabinentuerfluegel = LiftParameterHelper.GetLiftParameterValue<int>(parameterDictionary, "var_AnzahlTuerfluegel");
        double kabinentuerGewichtA = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Tuergewicht");
        double kabinentuerGewichtB = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Tuergewicht_B");
        double kabinentuerGewichtC = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Tuergewicht_C");
        double kabinentuerGewichtD = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Tuergewicht_D");

        string bodenTyp = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Bodentyp");
        double bodenblech = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Bodenblech");
        double bodenProfilGewicht = Math.Round(GetGewichtBodenprofil(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_BoPr")), 2);
        double bodengewichtProQM = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_SonderExternBodengewicht");
        double bodenBelagGewichtproQm = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Bodenbelagsgewicht");
        bool schuerzeVerstaerkt = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_Schuerzeverstaerkt");
        double schottenStaerke = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Materialstaerke");

        bool spiegelA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SpiegelA");
        bool spiegelB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SpiegelB");
        bool spiegelC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SpiegelC");
        bool spiegelD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SpiegelD");
        double spiegelHoehe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_HoeheSpiegel");
        double spiegelBreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_BreiteSpiegel");
        double spiegelHoehe2 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_HoeheSpiegel2");
        double spiegelBreite2 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_BreiteSpiegel2");
        double spiegelHoehe3 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_HoeheSpiegel3");
        double spiegelBreite3 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_BreiteSpiegel3");
        bool spiegelPaneel = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SpiegelPaneel");

        bool paneelPosA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_PaneelPosA");
        bool paneelPosB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_PaneelPosB");
        bool paneelPosC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_PaneelPosC");
        bool paneelPosD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_PaneelPosD");

        bool aussenverkleidungA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_AussenverkleidungA");
        bool aussenverkleidungB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_AussenverkleidungB");
        bool aussenverkleidungC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_AussenverkleidungC");
        bool aussenverkleidungD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_AussenverkleidungD");

        bool rammschutzA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_RammschutzA");
        bool rammschutzB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_RammschutzB");
        bool rammschutzC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_RammschutzC");
        bool rammschutzD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_RammschutzD");

        bool handlaufA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_HandlaufA");
        bool handlaufB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_HandlaufB");
        bool handlaufC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_HandlaufC");
        bool handlaufD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_HandlaufD");

        bool sockelleisteA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SockelleisteA");
        bool sockelleisteB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SockelleisteB");
        bool sockelleisteC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SockelleisteC");
        bool sockelleisteD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_SockelleisteD");

        //Database
        double sockelleisteHoehe = GetHoeheSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Sockelleiste"));
        double tableauGewicht = GetGewichtTableau(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_KabTabKabinentableau"));
        double tableauBreite = GetBreiteTableau(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_KabTabKabinentableau"));
        double belagAufDerDeckeGewichtproQm = GetGewichtSonderblech(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_BelagAufDemKabinendach"));
        double paneeleGewichtproQm = GetGewichtPaneele(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Paneelmaterial"));
        double stossleisteGewichtproMeter = Math.Round(GetGewichtRammschutz(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Rammschutz")), 2);

        //Calculations
        double bodengewicht = kabinenbreite * kabinentiefe * bodenblech * 7.85 / Math.Pow(10, 6) +
                                    ((((kabinenbreite / 230) + 1 + ((kabinenbreite > 2000) ? 1 : 0)) * kabinentiefe / 1000 +
                                    (((kabinenbreite > 1250) || (kabinentiefe > 2350)) ? 3 : 2) * kabinenbreite / 1000 +
                                    anzahlKabinentueren * tuerbreite / 1000) * bodenProfilGewicht);
        var kabinenBodengewicht =
            bodenTyp switch
            {
                "standard" or "verstärkt" => bodengewicht,
                "standard mit Wanne" => bodengewicht + (kabinenbreite * kabinentiefe * 11.8 / Math.Pow(10, 6)),
                "sonder" or "extern" => kabinenbreite * kabinentiefe * bodengewichtProQM / Math.Pow(10, 6),
                _ => 0,
            };

        var bodenBelagGewicht = kabinenbreite * kabinentiefe * bodenBelagGewichtproQm / Math.Pow(10, 6);

        var glasLaengeWandA = ((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Eingangswand")).StartsWith("VSG")
                                         ? (kabinenbreite - Convert.ToInt32(zugangA) * tuerbreite) / 1000 : 0;
        var glasLaengeWandB = ((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Seitenwaende")).Contains("Seite B")
                                         ? (kabinentiefe - Convert.ToInt32(zugangB) * tuerbreiteB) / 1000 : 0;
        var glasLaengeWandC = ((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Rueckwand")).StartsWith("VSG")
                                         ? (kabinenbreite - Convert.ToInt32(zugangC) * tuerbreiteC) / 1000 : 0;
        var glasLaengeWandD = (((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Seitenwaende")).Contains("Seite D") ||
                                         ((string)LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Seitenwaende")).Contains("Seite B/D"))
                                         ? (kabinentiefe - Convert.ToInt32(zugangD) * tuerbreiteD) / 1000 : 0;

        var schottenLaenge = (!zugangB ? glasLaengeWandB > 0 ? 0 : kabinentiefe : 0) +
                                     (!zugangC ? glasLaengeWandC > 0 ? 0 : kabinenbreite : 0) +
                                     (!zugangD ? glasLaengeWandD > 0 ? 0 : kabinentiefe : 0);

        var schottengewicht = schottenLaenge > 0 ? schottenStaerke * (kabineundAbgehaengteDeckeHoehe + 161) * 339 * 8 / Math.Pow(10, 6) * (schottenLaenge / 275) : 0;

        var haelsegewicht = schottenStaerke * ((kabineundAbgehaengteDeckeHoehe + 86) *
                                   ((halsL1 + halsR1 + halsL2 + halsR2 + halsL3 + halsR3 + halsL4 + halsR4) + anzahlKabinentueren * 214 +
                                   anzahlKabinentueren * (oeffnungsrichtung.StartsWith("einseitig öffnend") ? anzahlKabinentuerfluegel * 45 : 0))
                                   + anzahlKabinentueren * (tuerbreite + 104) * (kabineundAbgehaengteDeckeHoehe - tuerhoehe + 99)) * 8 / Math.Pow(10, 6);
        var andidroehnGewicht = 2 * 1000 * 150 * 1.36 / Math.Pow(10, 6) * (schottenLaenge / 275);

        var schuerzeGewicht = (kabinenbreite > 0 && kabinenbreite > 0) ?
                                     anzahlKabinentueren * (((schuerzeVerstaerkt ? 5 : 3) * 1.5 * (tuerbreite - 100) *
                                     266 + 1.5 * tuerbreite * 800 + (tuerbreite / 300 + 1) *
                                     1.5 * 380 * 109 + 7 * (schuerzeVerstaerkt ? 5 : 3) * 100 * 81) * 8 / Math.Pow(10, 6)) : 0;

        var deckegewicht = kabinenbreite * kabinentiefe * gewichtDecke / Math.Pow(10, 6);
        var gewichtAbgehaengteDecke = abgehaengteDecke ? kabinenbreite * kabinentiefe * abgehaengteDeckeGewichtproQm / Math.Pow(10, 6) : 0;

        var deckeBelegtGewicht = belegteDecke ? kabinenbreite * kabinentiefe * deckeBelegtGewichtproQm / Math.Pow(10, 6) : 0;


        var belagAufDerDeckeGewicht = belagAufDerDecke ? Math.Round(kabinenbreite * kabinentiefe * belagAufDerDeckeGewichtproQm / Math.Pow(10, 6)) : 0;

        var spiegelQm = !spiegelPaneel ? (spiegelHoehe * spiegelBreite + spiegelHoehe2 * spiegelBreite2 + spiegelHoehe3 * spiegelBreite3) / Math.Pow(10, 6) : 0;
        var spiegelGewicht = spiegelGewichtproQm * spiegelQm;

        var paneeleSpiegelQm = spiegelPaneel ? (spiegelHoehe * spiegelBreite + spiegelHoehe2 * spiegelBreite2 + spiegelHoehe3 * spiegelBreite3) / Math.Pow(10, 6) : 0;
        var paneeleSpiegelGewicht = paneeleSpiegelGewichtproQm * paneeleSpiegelQm;

        var paneeleQm = (paneelPosA || paneelPosB || paneelPosC || paneelPosD) ?
                       (((Convert.ToInt32(paneelPosA) + Convert.ToInt32(paneelPosC) * kabinenbreite / 1000) +
                     ((Convert.ToInt32(paneelPosB) + Convert.ToInt32(paneelPosD)) * kabinentiefe / 1000) - tableauBreite / 1000)) * (kabineundAbgehaengteDeckeHoehe - sockelleisteHoehe) / 1000 - (spiegelQm + paneeleSpiegelQm) : 0;
        var paneeleGewicht = paneeleGewichtproQm * paneeleQm;

        var vSGTyp = (glasLaengeWandA > 1 || glasLaengeWandB > 1 || glasLaengeWandC > 1 || glasLaengeWandD > 1) ? "VSG 12" : "VSG 10";
        var vSGGewichtproQm = (vSGTyp == "VSG 10") ? 25.0 : 30.0;
        var vSGQm = ((glasLaengeWandA > 0.15 ? glasLaengeWandA - 0.15 : 0) +
                                       (glasLaengeWandB > 0.15 ? glasLaengeWandB - 0.15 : 0) +
                                       (glasLaengeWandC > 0.15 ? glasLaengeWandC - 0.15 : 0) +
                                       (glasLaengeWandD > 0.15 ? glasLaengeWandD - 0.15 : 0) -
                                       ((glasLaengeWandA + glasLaengeWandB + glasLaengeWandC + glasLaengeWandD) > ((tableauBreite + 150) / 1000) ? (tableauBreite + 150) / 1000 : 0))
                                       * (kabineundAbgehaengteDeckeHoehe > 0 ? (kabineundAbgehaengteDeckeHoehe - 200) / 1000 : 0);
        var vSGGewicht = vSGQm * vSGGewichtproQm;

        int aussenVerkleidungenmitTueren = Convert.ToInt32(aussenverkleidungA) * Convert.ToInt32(zugangA) +
                                         Convert.ToInt32(aussenverkleidungB) * Convert.ToInt32(zugangB) +
                                         Convert.ToInt32(aussenverkleidungC) * Convert.ToInt32(zugangC) +
                                         Convert.ToInt32(aussenverkleidungD) * Convert.ToInt32(zugangD);

        var aussenVerkleidungQm = (aussenverkleidungA || aussenverkleidungB || aussenverkleidungC || aussenverkleidungD) ?
                                        (((Convert.ToInt32(aussenverkleidungA) + Convert.ToInt32(aussenverkleidungC)) * kabinenbreite / 1000) +
                                        ((Convert.ToInt32(aussenverkleidungB) + Convert.ToInt32(aussenverkleidungD)) * kabinentiefe / 1000)) * kabinenhoeheAussen / 1000 -
                                        aussenVerkleidungenmitTueren * (tuerbreite / 1000 * tuerhoehe / 1000) : 0;
        var aussenVerkleidungGewicht = Math.Round(aussenVerkleidungQm * aussenVerkleidungGewichtproQm, 0);

        var stossleisteLaenge = ((Convert.ToInt32(rammschutzA) + Convert.ToInt32(rammschutzC)) * kabinenbreite / 1000) +
                                        ((Convert.ToInt32(rammschutzB) + Convert.ToInt32(rammschutzD)) * kabinentiefe / 1000);
        var stossleisteGewicht = stossleisteGewichtproMeter * stossleisteLaenge;

        var handlaufGewichtproMeter = GetGewichtHandlauf(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Handlauf"));
        var handlaufLaenge = ((Convert.ToInt32(handlaufA) + Convert.ToInt32(handlaufC)) * kabinenbreite / 1000) +
                                        ((Convert.ToInt32(handlaufB) + Convert.ToInt32(handlaufD)) * kabinentiefe / 1000);
        var handlaufGewicht = handlaufGewichtproMeter * handlaufLaenge;

        var sockelleisteGewichtproMeter = GetGewichtSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Sockelleiste"));
        var sockelleisteLaenge = ((Convert.ToInt32(sockelleisteA) + Convert.ToInt32(sockelleisteC)) * kabinenbreite / 1000) +
                                        ((Convert.ToInt32(sockelleisteB) + Convert.ToInt32(sockelleisteD)) * kabinentiefe / 1000);
        var sockelleisteGewicht = sockelleisteLaenge * sockelleisteGewichtproMeter;

        //<!--  KabinenZubehör  -->
        var schutzgelaenderAnzahlPfosten = Math.Ceiling(kabinentiefe / 400 * 2 + kabinenbreite / 400 + 2);
        var schutzgelaenderGewicht = (kabinenbreite > 0 && kabinenbreite > 0) ?
                                  ((2 * 3 * 1.5 * (kabinentiefe > 0 ? (kabinentiefe - 250) : 0) * 137 +
                                  (anzahlKabinentueren > 1 ? 0 : 1) * 3 * 1.5 * kabinenbreite * 137) * 8 / Math.Pow(10, 6) +
                                   (1.5 * 670 * 120 + 5 * 50 * 85) * 8 / Math.Pow(10, 6) * schutzgelaenderAnzahlPfosten) : 0;
        var klemmkastenGewicht = (kabinenbreite > 0 && kabinenbreite > 0) ? gewichtKlemmkasten : 0;
        var schraubenZubehoerGewicht = (kabinenbreite > 0 && kabinenbreite > 0) ? gewichtSchraubenZubehoer : 0;

        //<!--  KabinenGewichtDetail  -->
        var kabinenGewichtGesamt = kabinenBodengewicht + bodenBelagGewicht + schottengewicht + andidroehnGewicht + haelsegewicht + schuerzeGewicht + deckegewicht + gewichtAbgehaengteDecke +
                                              deckeBelegtGewicht + belagAufDerDeckeGewicht + spiegelGewicht + paneeleGewicht + paneeleSpiegelGewicht + vSGGewicht + aussenVerkleidungGewicht +
                                              stossleisteGewicht + handlaufGewicht + sockelleisteGewicht + schutzgelaenderGewicht + klemmkastenGewicht + schraubenZubehoerGewicht + tableauGewicht;

        var kabinenTuerGewicht = variableTuerdaten ? (zugangA ? kabinentuerGewichtA : 0) +
                                                                (zugangB ? kabinentuerGewichtB : 0) +
                                                                (zugangC ? kabinentuerGewichtC : 0) +
                                                                (zugangD ? kabinentuerGewichtD : 0)
                                                               : kabinentuerGewichtA * anzahlKabinentueren;

        var fangrahmenGewicht = GetCarFrameWeight(parameterDictionary);

        var fahrkorbGewicht = Math.Round(kabinenGewichtGesamt + kabinenKorrekturGewicht + kabinenTuerGewicht + fangrahmenGewicht);

        return new CarWeightResult()
        {
            AnzahlKabinentueren = anzahlKabinentueren,
            AnzahlKabinentuerfluegel = anzahlKabinentuerfluegel,
            SchuerzeVerstaerkt = schuerzeVerstaerkt,
            ZugangA = zugangA,
            ZugangB = zugangB,
            ZugangC = zugangC,
            ZugangD = zugangD,
            BodenBelagGewichtproQm = bodenBelagGewichtproQm,
            AbgehaengteDeckeGewichtproQm = abgehaengteDeckeGewichtproQm,
            DeckeBelegtGewichtproQm = deckeBelegtGewichtproQm,
            SpiegelGewichtproQm = spiegelGewichtproQm,
            PaneeleGewichtproQm = paneeleGewichtproQm,
            PaneeleSpiegelGewichtproQm = paneeleSpiegelGewichtproQm,
            BelagAufDerDeckeGewichtproQm = belagAufDerDeckeGewichtproQm,
            VSGGewichtproQm = vSGGewichtproQm,
            AussenVerkleidungGewichtproQm = aussenVerkleidungGewichtproQm,
            HandlaufGewichtproMeter = handlaufGewichtproMeter,
            StossleisteGewichtproMeter = stossleisteGewichtproMeter,
            SockelleisteGewichtproMeter = sockelleisteGewichtproMeter,
            GewichtKlemmkasten = gewichtKlemmkasten,
            GewichtSchraubenZubehoer = gewichtSchraubenZubehoer,
            Deckegewicht = deckegewicht,
            GewichtAbgehaengteDecke = gewichtAbgehaengteDecke,
            Bodenblech = bodenblech,
            BodenProfilGewicht = bodenProfilGewicht,
            KabinenBodengewicht = kabinenBodengewicht,
            BodenBelagGewicht = bodenBelagGewicht,
            Schottengewicht = schottengewicht,
            Haelsegewicht = haelsegewicht,
            AndidroehnGewicht = andidroehnGewicht,
            SchuerzeGewicht = schuerzeGewicht,
            DeckeBelegtGewicht = deckeBelegtGewicht,
            BelagAufDerDeckeGewicht = belagAufDerDeckeGewicht,
            SpiegelQm = spiegelQm,
            SpiegelGewicht = spiegelGewicht,
            PaneeleQm = paneeleQm,
            PaneeleGewicht = paneeleGewicht,
            PaneeleSpiegelQm = paneeleSpiegelQm,
            PaneeleSpiegelGewicht = paneeleSpiegelGewicht,
            VSGTyp = vSGTyp,
            VSGQm = vSGQm,
            VSGGewicht = vSGGewicht,
            AussenVerkleidungQm = aussenVerkleidungQm,
            AussenVerkleidungGewicht = aussenVerkleidungGewicht,
            StossleisteLaenge = stossleisteLaenge,
            StossleisteGewicht = stossleisteGewicht,
            HandlaufLaenge = handlaufLaenge,
            HandlaufGewicht = handlaufGewicht,
            SockelleisteLaenge = sockelleisteLaenge,
            SockelleisteGewicht = sockelleisteGewicht,
            SchutzgelaenderAnzahlPfosten = schutzgelaenderAnzahlPfosten,
            SchutzgelaenderGewicht = schutzgelaenderGewicht,
            TableauBreite = tableauBreite,
            TableauGewicht = tableauGewicht,
            KlemmkastenGewicht = klemmkastenGewicht,
            SchraubenZubehoerGewicht = schraubenZubehoerGewicht,
            KabinenGewichtGesamt = kabinenGewichtGesamt,
            KabinenTuerGewicht = kabinenTuerGewicht,
            FangrahmenGewicht = fangrahmenGewicht,
            FahrkorbGewicht = fahrkorbGewicht
        };
    }

    public SafetyGearResult GetSafetyGearCalculation(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        var carRailSurface = string.Empty;
        var lubrication = string.Empty;
        var allowedRailHeads = string.Empty;
        var railHeadAllowed = false;
        var railHead = 0.0;
        var minLoad = 0;
        var maxLoad = 0;

        if (!string.IsNullOrWhiteSpace(parameterDictionary!["var_Fuehrungsart"].Value))
        {
            lubrication = parameterDictionary!["var_Fuehrungsart"].Value switch
            {
                "Gleitführung" => "geölt",
                "Rollenführung" => "trocken",
                _ => string.Empty,
            };
        }

        if (!string.IsNullOrWhiteSpace(parameterDictionary!["var_FuehrungsschieneFahrkorb"].Value))
        {
            var carRail = _parametercontext.Set<GuideRails>().FirstOrDefault(x => x.Name.Contains(parameterDictionary!["var_FuehrungsschieneFahrkorb"].Value!));
            if (carRail is not null)
            {
                carRailSurface = carRail.Machined ? "bearbeitet" : "gezogen";
                railHead = carRail.RailHead;
            }
        }

        if (!string.IsNullOrWhiteSpace(parameterDictionary!["var_TypFV"].Value))
        {
            if (carRailSurface != string.Empty && lubrication != string.Empty)
            {
                var currentSafetyGear = _parametercontext.Set<SafetyGearModelType>().FirstOrDefault(x => x.Name.Contains(parameterDictionary!["var_TypFV"].Value!));
                if (currentSafetyGear is not null)
                {
                    var listOfRailHeads = GetAllowedRailHeads(currentSafetyGear.AllowableWidth);
                    railHeadAllowed = listOfRailHeads.Count == 2 ? listOfRailHeads.First() <= railHead && listOfRailHeads.Last() >= railHead : listOfRailHeads.Contains(railHead);
                    allowedRailHeads = listOfRailHeads.Count == 2 ? $"{listOfRailHeads.First()} - {listOfRailHeads.Last()}" : string.Join(" | ", listOfRailHeads);
                    if (railHeadAllowed)
                    {
                        if (lubrication == "geölt" && carRailSurface == "gezogen")
                        {
                            minLoad = currentSafetyGear.MinLoadOiledColddrawn;
                            maxLoad = currentSafetyGear.MaxLoadOiledColddrawn;
                        }
                        else if (lubrication == "trocken" && carRailSurface == "gezogen")
                        {
                            minLoad = currentSafetyGear.MinLoadDryColddrawn;
                            maxLoad = currentSafetyGear.MaxLoadDryColddrawn;
                        }
                        else if (lubrication == "geölt" && carRailSurface == "bearbeitet")
                        {
                            minLoad = currentSafetyGear.MinLoadOiledMachined;
                            maxLoad = currentSafetyGear.MaxLoadOiledMachined;
                        }
                        else if (lubrication == "trocken" && carRailSurface == "bearbeitet")
                        {
                            minLoad = currentSafetyGear.MinLoadDryMachined;
                            maxLoad = currentSafetyGear.MaxLoadDryMachined;
                        }
                    }
                }
            }
        }

        return new SafetyGearResult()
        {
            CarRailSurface = carRailSurface,
            Lubrication = lubrication,
            AllowedRailHeads = allowedRailHeads,
            RailHeadAllowed = railHeadAllowed,
            MinLoad = minLoad,
            MaxLoad = maxLoad
        };
    }

    private void SetDefaultParameter(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        kabinenbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KBI");
        kabinentiefe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KTI");
        zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_A");
        zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_B");
        zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_C");
        zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_D");
        anzahlKabinentueren = NumberOfCardoors(zugangA, zugangB, zugangC, zugangD);
        tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB");
        tuerbreiteB = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_B");
        tuerbreiteC = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_C");
        tuerbreiteD = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_D");
        tuerhoehe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TH");
        halsL1 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_L1");
        halsR1 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_R1");
        halsL2 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_L2");
        halsR2 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_R2");
        halsL3 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_L3");
        halsR3 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_R3");
        halsL4 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_L4");
        halsR4 = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_R4");
    }

    public static int NumberOfCardoors(bool zugangA, bool zugangB, bool zugangC, bool zugangD)
    {
        return Convert.ToInt32(zugangA) + Convert.ToInt32(zugangB) + Convert.ToInt32(zugangC) + Convert.ToInt32(zugangD);
    }

    private double GetCarDoorArea(ObservableDictionary<string, Parameter>? parameterDictionary, string zugang)
    {
        double tuerbreiteZugang;
        double tuerEinbau;
        string? tuerbezeichnung;
        switch (zugang)
        {
            case "A":
                tuerbreiteZugang = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbau");
                tuerbezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Tuerbezeichnung");
                break;
            case "B":
                tuerbreiteZugang = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_B");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauB");
                tuerbezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Tuerbezeichnung_B");
                break;
            case "C":
                tuerbreiteZugang = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_C");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauC");
                tuerbezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Tuerbezeichnung_C");
                break;
            case "D":
                tuerbreiteZugang = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_D");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauD");
                tuerbezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Tuerbezeichnung_D");
                break;
            default:
                return 0.0;
        }

        if (tuerEinbau <= 0)
            return 0;

        if (tuerbreiteZugang <= 0)
            return 0;

        if (string.IsNullOrWhiteSpace(tuerbezeichnung))
            return 0;

        var kabinenTuer = new CarDoorDesignParameter();

        var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor)
                                                                  .FirstOrDefault(x => x.Name == tuerbezeichnung);
        if (liftDoorGroup?.CarDoor is not null)
        {
            var carDoor = liftDoorGroup.CarDoor;
            kabinenTuer.Hersteller = carDoor.Manufacturer!;
            kabinenTuer.Typ = carDoor.Name;
            kabinenTuer.Baumusterpruefbescheinigung = carDoor.Name;
            kabinenTuer.Tuerbreite = tuerbreiteZugang;
            kabinenTuer.AnzahlTuerFluegel = carDoor.DoorPanelCount;
            kabinenTuer.TuerFluegelBreite = carDoor.DoorPanelWidth;
            kabinenTuer.TuerFluegelAbstand = carDoor.DoorPanelSpace;
        }

        if (kabinenTuer is null)
            return 0;

        if ((tuerEinbau - kabinenTuer.TuerFluegelBreite) <= 100)
        {
            switch (zugang)
            {
                case "A":
                    if (kabinenbreite > kabinenTuer.Tuerbreite)
                        return 0;
                    break;
                case "B":
                    if (kabinentiefe > kabinenTuer.Tuerbreite)
                        return 0;
                    break;
                case "C":
                    if (kabinenbreite > kabinenTuer.Tuerbreite)
                        return 0;
                    break;
                case "D":
                    if (kabinentiefe > kabinenTuer.Tuerbreite)
                        return 0;
                    break;
                default:
                    break;
            }
        }

        return kabinenTuer.AnzahlTuerFluegel switch
        {
            2 => Math.Round((kabinenTuer.Tuerbreite / 2 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel * kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),

            3 => Math.Round((kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            2 * kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel * kabinenTuer.TuerFluegelBreite + 2 * kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),

            4 => Math.Round((kabinenTuer.Tuerbreite / 2 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel / 2 * kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),

            6 => Math.Round((kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            2 * kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel / 2 * kabinenTuer.TuerFluegelBreite + 2 * kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),
            _ => 0,
        };
    }

    public double GetLoadFromTable(double area, string tableName)
    {
        var table = tableName switch
        {
            "Tabelle6" => Table6,
            "Tabelle7" => Table7,
            _ => Table6,
        };

        TableRow<int, double>? nutzlast = null;
        if (table == null)
            return 0;
        if (area <= 0)
            return 0;
        if (tableName == "Tabelle6" && area > 5.0)
            return Math.Round(2500 + (area - 5.0) / 0.16 * 100, 0);
        if (tableName == "Tabelle6" && area < 0.37)
            return 0;
        if (tableName == "Tabelle7" && area > 5.04)
            return Math.Round(1600 + (area - 5.04) / 0.40 * 100, 0);
        if (tableName == "Tabelle7" && area < 1.68)
            return 0;
        if (table.Any(x => x.Value.SecondValue == area))
        {
            nutzlast = table.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            nutzlast.IsSelected = true;
            return nutzlast.FirstValue;
        };
        var lowTableEntry = table.Where(x => x.Value.SecondValue < area).Last();
        lowTableEntry.Value.IsSelected = true;
        var highTableEntry = table.Where(x => x.Value.SecondValue > area).First();
        highTableEntry.Value.IsSelected = true;
        return Math.Round(lowTableEntry.Value.FirstValue + (highTableEntry.Value.FirstValue - lowTableEntry.Value.FirstValue) /
                (highTableEntry.Value.SecondValue - lowTableEntry.Value.SecondValue) * (area - lowTableEntry.Value.SecondValue), 0);
    }

    public int GetPersonenCarArea(double area)
    {
        TableRow<int, double>? personenAnzahl = null;
        if (Table8 == null)
            return 0;
        if (area < 0.28)
            return 0;
        if (area > 3.13)
            return Convert.ToInt32(20 + (area - 3.13) / 0.115);
        if (Table8.Any(x => x.Value.SecondValue == area))
        {
            personenAnzahl = Table8.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            personenAnzahl.IsSelected = true;
            return personenAnzahl.FirstValue;
        };
        personenAnzahl = Table8.Where(x => x.Value.SecondValue < area).Last().Value;
        LogTabledata("Tabelle 8", personenAnzahl.FirstValue);
        personenAnzahl.IsSelected = true;
        return personenAnzahl.FirstValue;
    }

    public double GetCarFrameWeight(ObservableDictionary<string, Parameter>? parameterDictionary)
    {

        if (!string.IsNullOrWhiteSpace(parameterDictionary!["var_Rahmengewicht"].Value))
        {
            return LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Rahmengewicht");
        }
        else if (!string.IsNullOrWhiteSpace(parameterDictionary!["var_Bausatz"].Value))
        {
            var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == parameterDictionary!["var_Bausatz"].Value);
            if (carFrameType is null)
                return 0;
            return carFrameType.CarFrameWeight;
        }
        else
        {
            return 0;
        }
    }

    public CarFrameType? GetCarFrameTyp(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        return _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == parameterDictionary!["var_Bausatz"].Value);
    }

    public void SetPayLoadResult(ObservableDictionary<string, Parameter> parameterDictionary, int personenBerechnet, double nutzflaecheGesamt)
    {
        if (parameterDictionary.TryGetValue("var_Personen", out Parameter personen))
        {
            personen.Value = Convert.ToString(personenBerechnet);
        }

        if (nutzflaecheGesamt > 0)
        {
            if (parameterDictionary.TryGetValue("var_A_Kabine", out Parameter aKabine))
            {
                aKabine.Value = Convert.ToString(nutzflaecheGesamt);
            }
        }
    }

    private static Dictionary<int, TableRow<int, double>> SetTableData(object[]? tabledata, string firstUnit, string secondUnit)
    {
        var dic = new Dictionary<int, TableRow<int, double>>();
        if (tabledata is null)
            return dic;
        switch (tabledata.GetType().Name)
        {
            case "LoadTable6[]":
                foreach (var item in (LoadTable6[])tabledata)
                {
                    dic.Add(item.Load, new TableRow<int, double>
                    {
                        FirstValue = item.Load,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            case "LoadTable7[]":
                foreach (var item in (LoadTable7[])tabledata)
                {
                    dic.Add(item.Load, new TableRow<int, double>
                    {
                        FirstValue = item.Load,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            case "PersonsTable8[]":
                foreach (var item in (PersonsTable8[])tabledata)
                {
                    dic.Add(item.Persons, new TableRow<int, double>
                    {
                        FirstValue = item.Persons,
                        SecondValue = item.Area,
                        FirstUnit = firstUnit,
                        SecondUnit = secondUnit
                    });
                }
                break;
            default:
                break;
        }
        return dic;
    }

    private static void ClearSelectedRows(Dictionary<int, TableRow<int, double>> table)
    {
        foreach (var row in table)
        {
            if (row.Value.IsSelected)
                row.Value.IsSelected = false;
        }
    }

    private static List<double> GetAllowedRailHeads(string? railHeads)
    {
        var listOfRailHeads = new List<double>();

        if (railHeads is not null)
        {
            var listOfRailHeadsStrings = railHeads!.Contains('-') ? railHeads.Split('-') : railHeads.Split(';');

            foreach (var railHead in listOfRailHeadsStrings)
            {
                listOfRailHeads.Add(Convert.ToDouble(railHead, CultureInfo.CurrentCulture));
            }
        }
        return listOfRailHeads;
    }

    //Database

    private double? GetGewichtSonderblech(string bodenblech)
    {
        if (string.IsNullOrEmpty(bodenblech))
            return 0;
        var blech = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenblech && x.SpecialSheet == true);
        if (blech is null)
            return 0;
        return blech.WeightPerSquareMeter;
    }

    private double? GetGewichtBodenprofil(string bodenprofil)
    {
        if (string.IsNullOrEmpty(bodenprofil))
            return 0;
        var profil = _parametercontext.Set<CarFloorProfile>().FirstOrDefault(x => x.Name == bodenprofil);
        if (profil is null)
            return 0;
        return profil.WeightPerMeter;
    }

    private double? GetGewichtPaneele(string paneele)
    {
        if (string.IsNullOrEmpty(paneele))
            return 0;
        var coverPanel = _parametercontext.Set<CarCoverPanel>().FirstOrDefault(x => x.Name == paneele);
        if (coverPanel is null)
            return 0;
        return coverPanel.WeightPerSquareMeter;
    }

    private double? GetGewichtRammschutz(string rammschutz)
    {
        if (string.IsNullOrEmpty(rammschutz))
            return 0;
        var rammingProtection = _parametercontext.Set<RammingProtection>().FirstOrDefault(x => x.Name == rammschutz);
        if (rammingProtection is null)
            return 0;
        return rammingProtection.WeightPerMeter;
    }

    private double? GetGewichtHandlauf(string handlauf)
    {
        if (string.IsNullOrEmpty(handlauf))
            return 0;
        var handrail = _parametercontext.Set<Handrail>().FirstOrDefault(x => x.Name == handlauf);
        if (handrail is null)
            return 0;
        return handrail.WeightPerMeter;
    }

    private double? GetGewichtSockelleiste(string sockelleiste)
    {
        if (string.IsNullOrEmpty(sockelleiste))
            return 0;
        var skirtingBoard = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == sockelleiste);
        if (skirtingBoard is null)
            return 0;
        return skirtingBoard.WeightPerMeter;
    }

    private double? GetHoeheSockelleiste(string sockelleiste)
    {
        if (string.IsNullOrEmpty(sockelleiste))
            return 0;
        var skirtingBoard = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == sockelleiste);
        if (skirtingBoard is null)
            return 0;
        return skirtingBoard.Height;
    }

    private double? GetGewichtTableau(string tableau)
    {
        if (string.IsNullOrEmpty(tableau))
            return 0;
        var carPanel = _parametercontext.Set<CarPanel>().FirstOrDefault(x => x.Name == tableau);
        if (carPanel is null)
            return 0;
        return carPanel.Weight;
    }

    private double? GetBreiteTableau(string tableau)
    {
        if (string.IsNullOrEmpty(tableau))
            return 0;
        var carPanel = _parametercontext.Set<CarPanel>().FirstOrDefault(x => x.Name == tableau);
        if (carPanel is null)
            return 0;
        return carPanel.Width;
    }

    [LoggerMessage(60121, LogLevel.Debug,
    "table {tableName} with {load}loaded")]
    partial void LogTabledata(string tableName, double load);
}