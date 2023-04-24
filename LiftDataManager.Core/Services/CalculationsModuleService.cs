using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.ComponentModels;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.Core.Services;

public partial class CalculationsModuleService : ICalculationsModule
{
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<CalculationsModuleService> _logger;

    public required Dictionary<int, TableRow<int, double>> Table6 { get; set; }
    public required Dictionary<int, TableRow<int, double>> Table7 { get; set; }
    public required Dictionary<int, TableRow<int, double>> Table8 { get; set; }

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

        if (load >= loadTable6) return true;
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

    public CarVentilationResult GetCarVentilationCalculation (ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        const int tuerspalt = 4;
        const int luftspaltoeffnung = 10;
        const int anzahlLuftspaltoeffnungenFT = 2;

        double aKabine = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_A_Kabine");
        double kabinenbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KBI");
        double kabinentiefe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KTI");
        double tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB");
        double tuerhoehe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TH");

        int anzahlKabinentuerfluegel = LiftParameterHelper.GetLiftParameterValue<int>(parameterDictionary, "var_AnzahlTuerfluegel");

        int anzahlKabinentueren = NumberOfCardoors(parameterDictionary);

        double aKabine1Pozent = Math.Round(aKabine * 10000);
        double belueftung1Seite = kabinentiefe* luftspaltoeffnung;
        double belueftung2Seiten = kabinentiefe* luftspaltoeffnung *2;

        bool ergebnisBelueftungDecke = belueftung2Seiten > aKabine1Pozent;

        int anzahlLuftspaltoeffnungenTB = anzahlKabinentueren * 2;
        int anzahlLuftspaltoeffnungenTH = anzahlKabinentueren * anzahlKabinentuerfluegel;
        double flaecheLuftspaltoeffnungenTB = anzahlLuftspaltoeffnungenTB * tuerspalt * tuerbreite;
        double flaecheLuftspaltoeffnungenTH = anzahlLuftspaltoeffnungenTH * tuerspalt * tuerhoehe;
        double entlueftungTuerspalten50Pozent = (flaecheLuftspaltoeffnungenTB + flaecheLuftspaltoeffnungenTH) * 0.5;

        int anzahlLuftspaltoeffnungenFB = (anzahlKabinentueren > 1) ? 0 : 1;

        double flaecheLuftspaltoeffnungenFB = Math.Round(kabinenbreite / 50 * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
        double flaecheLuftspaltoeffnungenFT = Math.Round(kabinentiefe / 50 * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
        double flaecheLuftspaltoeffnungenFBGesamt = anzahlLuftspaltoeffnungenFB * flaecheLuftspaltoeffnungenFB;
        double flaecheLuftspaltoeffnungenFTGesamt = anzahlLuftspaltoeffnungenFT * flaecheLuftspaltoeffnungenFT;

        double flaecheEntLueftungSockelleisten = Math.Round(flaecheLuftspaltoeffnungenFB + flaecheLuftspaltoeffnungenFT);
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
            AnzahlLuftspaltoeffnungenTB=anzahlLuftspaltoeffnungenTB,
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
        var aufzugstyp = LiftParameterHelper.GetLiftParameterValue<string>(parameterDictionary, "var_Aufzugstyp");
 
        var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                        .ToList()
                                                        .FirstOrDefault(x => x.Name == aufzugstyp);
        var cargoTyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";

        var driveSystemDB = _parametercontext.Set<LiftType>().Include(i => i.DriveType)
                                                           .ToList()
                                                           .FirstOrDefault(x => x.Name == aufzugstyp);
        var driveSystem = driveSystemDB is not null ? driveSystemDB.DriveType!.Name! : "";

        var zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_A");
        var zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_B");
        var zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_C");
        var zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_D");
        var anzahlKabinentueren = NumberOfCardoors(zugangA, zugangB, zugangC, zugangD);

        var kabinenbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KBI");
        var kabinentiefe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KTI");

        var nutzflaecheKabine = Math.Round(kabinenbreite * kabinentiefe / Math.Pow(10, 6), 2);
        var nutzflaecheZugangA = zugangA ? GetCarDoorArea(parameterDictionary, "A") : 0;
        var nutzflaecheZugangB = zugangB ? GetCarDoorArea(parameterDictionary, "B") : 0;
        var nutzflaecheZugangC = zugangC ? GetCarDoorArea(parameterDictionary, "C") : 0;
        var nutzflaecheZugangD = zugangD ? GetCarDoorArea(parameterDictionary, "D") : 0;
        
        var nutzflaeche = Math.Round(nutzflaecheKabine + nutzflaecheZugangA + nutzflaecheZugangB + nutzflaecheZugangC + nutzflaecheZugangD, 2);
        var nennlast = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_Q");
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
            PayloadAllowed = payloadAllowed,
            Personen75kg = personen75kg,
            PersonenFlaeche = personenFlaeche,
            PersonenBerechnet = personenBerechnet
        };
    }

    public CarWeightResult GetCarWeightCalculation(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        return new CarWeightResult()
        {

        };
    }

    public int NumberOfCardoors(ObservableDictionary<string, Parameter>? parameterDictionary)
    {
        bool zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_A");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(parameterDictionary, "var_ZUGANSSTELLEN_D");

        return Convert.ToInt32(zugangA) + Convert.ToInt32(zugangB) + Convert.ToInt32(zugangC) + Convert.ToInt32(zugangD);
    }

    public int NumberOfCardoors(bool zugangA, bool zugangB, bool zugangC, bool zugangD)
    {
        return Convert.ToInt32(zugangA) + Convert.ToInt32(zugangB) + Convert.ToInt32(zugangC) + Convert.ToInt32(zugangD);
    }

    private double GetCarDoorArea(ObservableDictionary<string, Parameter>? parameterDictionary, string zugang)
    {
        double tuerbreite;
        double tuerEinbau;
        switch (zugang)
        {
            case "A":
                tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbau");
                break;
            case "B":
                tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_B");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauB");
                break;
            case "C":
                tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_C");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauC");
                break;
            case "D":
                tuerbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TB_D");
                tuerEinbau = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_TuerEinbauD");
                break;
            default:
                return 0.0;
        }

        if (tuerEinbau <= 0)
            return 0;

        // ToDo Türdaten aus Datenbankladen Beispieldaten einer TTK 25
        // Logic zur Auswahl fehlt noch 
        // CarDoor Model am sinnvollsten schon bei der AuswahlTüren erstellen und in einem ComponentModelsdictionary verwalten

        var kabinenTuer = new CarDoorDesignParameter()
        {
            Name = "Meiller TTK 25",
            Hersteller = "Meiller",
            Typ = "TTK25",
            Schwellenbreite = 93,
            Tuerbreite = tuerbreite,
            MinimalerEinbau = 135,
            AnzahlTuerFluegel = 2,
            TuerFluegelBreite = 36,
            TuerFluegelAbstand = 6
        };

        if (kabinenTuer is null)
            return 0;

        var kabinenbreite = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KBI");
        var kabinentiefe = LiftParameterHelper.GetLiftParameterValue<double>(parameterDictionary, "var_KTI");

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
        if (table == null)  return 0; 
        if (area <= 0)  return 0; 
        if (tableName == "Tabelle6" && area > 5.0) return Math.Round(2500 + (area - 5.0) / 0.16 * 100,0);
        if (tableName == "Tabelle6" && area < 0.37) return 0;
        if (tableName == "Tabelle7" && area > 5.04) return Math.Round(1600 + (area - 5.04) / 0.40 * 100, 0);
        if (tableName == "Tabelle7" && area < 1.68) return 0;
        if (table.Any(x => x.Value.SecondValue == area))
        {
            nutzlast = table.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return nutzlast.FirstValue;
        };
        var lowTableEntry = table.Where(x => x.Value.SecondValue < area).Last();
        var highTableEntry = table.Where(x => x.Value.SecondValue > area).First();
        return Math.Round(lowTableEntry.Value.FirstValue + (highTableEntry.Value.FirstValue - lowTableEntry.Value.FirstValue) /
                (highTableEntry.Value.SecondValue - lowTableEntry.Value.SecondValue) * (area - lowTableEntry.Value.SecondValue),0); 
    }

    public int GetPersonenCarArea(double area)
    {
        TableRow<int, double>? personenAnzahl = null;
        if (Table8 == null) return 0; 
        if (area < 0.28) return 0;
        if (area > 3.13) return Convert.ToInt32(20 + (area - 3.13) / 0.115);
        if (Table8.Any(x => x.Value.SecondValue == area))
        {
            personenAnzahl = Table8.FirstOrDefault(x => x.Value.SecondValue == area).Value;
            return personenAnzahl.FirstValue;
        };
        personenAnzahl = Table8.Where(x => x.Value.SecondValue < area).Last().Value;
        LogTabledata("Tabelle 8", personenAnzahl.FirstValue);
        return personenAnzahl.FirstValue;
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
        if (tabledata is null) return dic;
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

    [LoggerMessage(60121, LogLevel.Debug,
    "table {tableName} with {load}loaded")]
    partial void LogTabledata(string tableName, double load);
}