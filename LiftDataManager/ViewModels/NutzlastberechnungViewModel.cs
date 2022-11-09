using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
namespace LiftDataManager.ViewModels;

public class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware, IRecipient<AreaPersonsRequestMessageAsync>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;

    public Dictionary<int, TableRow<int, double>> Tabelle6 { get; set; } = new();
    public Dictionary<int, TableRow<int, double>> Tabelle7 { get; set; } = new();
    public Dictionary<int, TableRow<int, double>> Tabelle8 { get; set; } = new();

    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService,
                                       INavigationService navigationService, ICalculationsModule calculationsModuleService ,ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;

        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.ParamterDictionary is not null)
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;

        Tabelle6 = _calculationsModuleService.GetTable(nameof(Tabelle6));
        Tabelle7 = _calculationsModuleService.GetTable(nameof(Tabelle7));
        Tabelle8 = _calculationsModuleService.GetTable(nameof(Tabelle8));
    }

    public void Receive(AreaPersonsRequestMessageAsync message)
    {
        if (!message.HasReceivedResponse)
        {
            message.Reply(new CalculatedValues
            {
                Personen = PersonenBerechnet,
                NutzflaecheKabine = NutzflaecheGesamt
            });
        }
        IsActive = false;
    }

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);

    public string Aufzugstyp => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Aufzugstyp");
    public string CargoTyp
    {
        get
        {
            var cargoTyp = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == Aufzugstyp);
            return cargoTyp is not null ? cargoTyp.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
        }
    }
    public string DriveSystem
    {
        get
        {
            var driveSystem = _parametercontext.Set<LiftType>().Include(i => i.DriveType)
                                                               .ToList()
                                                               .FirstOrDefault(x => x.Name == Aufzugstyp);
            return driveSystem is not null ? driveSystem.DriveType!.Name! : "" ;
        }
    }

    public double Kabinenbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
    public double Kabinentiefe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");

    public bool ZugangA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_A");
    public bool ZugangB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
    public bool ZugangC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
    public bool ZugangD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");

    public int AnzahlKabinentueren => Convert.ToInt32(ZugangA) + Convert.ToInt32(ZugangB) + Convert.ToInt32(ZugangC) + Convert.ToInt32(ZugangD);

    public string TuertypA => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp");
    public string TuertypB => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp_B");
    public string TuertypC => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp_C");
    public string TuertypD => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp_D");

    public string TuerbezeichnungA => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung");
    public string TuerbezeichnungB => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung_B");
    public string TuerbezeichnungC => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung_C");
    public string TuerbezeichnungD => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung_D");

    public double TuerbreiteA => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
    public double TuerbreiteB => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_B");
    public double TuerbreiteC => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_C");
    public double TuerbreiteD => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_D");

    public double TuerEinbauA => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TuerEinbau");
    public double TuerEinbauB => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TuerEinbauB");
    public double TuerEinbauC => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TuerEinbauC");
    public double TuerEinbauD => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TuerEinbauD");

    public CarDoorDesignParameter? KabinenTuerA => ZugangA ? GetCarDoorDetails() : null;
    public CarDoorDesignParameter? KabinenTuerB => ZugangB ? GetCarDoorDetails() : null;
    public CarDoorDesignParameter? KabinenTuerC => ZugangC ? GetCarDoorDetails() : null;
    public CarDoorDesignParameter? KabinenTuerD => ZugangD ? GetCarDoorDetails() : null;

    public string InfoZugangA => NutzflaecheZugangA == 0 && ZugangA ? " Tiefe < 100" : string.Empty;
    public string InfoZugangB => NutzflaecheZugangB == 0 && ZugangB ? " Tiefe < 100" : string.Empty;
    public string InfoZugangC => NutzflaecheZugangC == 0 && ZugangC ? " Tiefe < 100" : string.Empty;
    public string InfoZugangD => NutzflaecheZugangD == 0 && ZugangD ? " Tiefe < 100" : string.Empty;


    public double NutzflaecheKabine => Math.Round(Kabinenbreite * Kabinentiefe / Math.Pow(10, 6), 2);
    public double NutzflaecheZugangA => ZugangA ? GetCarDoorArea(KabinenTuerA) : 0;
    public double NutzflaecheZugangB => ZugangB ? GetCarDoorArea(KabinenTuerB) : 0;
    public double NutzflaecheZugangC => ZugangC ? GetCarDoorArea(KabinenTuerC) : 0;
    public double NutzflaecheZugangD => ZugangD ? GetCarDoorArea(KabinenTuerD) : 0;
    public double NutzflaecheGesamt
    {
        get
        {
            var nutzflaeche = Math.Round(NutzflaecheKabine + NutzflaecheZugangA + NutzflaecheZugangB + NutzflaecheZugangC + NutzflaecheZugangD, 2);
            ParamterDictionary!["var_A_Kabine"].Value = Convert.ToString(nutzflaeche);
            return nutzflaeche;
        }
    }

    public double NennlastTabelle6 => _calculationsModuleService.GetLoadFromTable(NutzflaecheGesamt,nameof(Tabelle6));
    public double NennlastTabelle7 => _calculationsModuleService.GetLoadFromTable(NutzflaecheGesamt,nameof(Tabelle7));
    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");

    public string ErgebnisNennlast => _calculationsModuleService.ValdidateLiftLoad(Nennlast, NutzflaecheGesamt, CargoTyp, DriveSystem) ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";
    public SolidColorBrush ErgebnisNennlastColor => _calculationsModuleService.ValdidateLiftLoad(Nennlast, NutzflaecheGesamt, CargoTyp, DriveSystem) ? successColor : failureColor;

    private double GetCarDoorArea(CarDoorDesignParameter? kabinenTuer, [CallerMemberName] string membername = "")
    {
        var tuerEinbau = membername switch
        {
            nameof(NutzflaecheZugangA) => TuerEinbauA,
            nameof(NutzflaecheZugangB) => TuerEinbauB,
            nameof(NutzflaecheZugangC) => TuerEinbauC,
            nameof(NutzflaecheZugangD) => TuerEinbauD,
            _ => 0,
        };

        if (kabinenTuer is null)
        {
            return 0;
        }

        if ((tuerEinbau - kabinenTuer.TuerFluegelBreite) <= 100)
        {
            return 0;
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

    public int Personen75kg => (int)(Nennlast / 75);
    public int PersonenFlaeche => _calculationsModuleService.GetPersonenCarArea(NutzflaecheGesamt);
    public int PersonenBerechnet => (Personen75kg > PersonenFlaeche) ? PersonenFlaeche : Personen75kg;

    public void SetPersonen() => ParamterDictionary!["var_Personen"].Value = Convert.ToString(PersonenBerechnet);

    private void MarkTableSelectedRow()
    {
        if (PersonenFlaeche > 0 && PersonenFlaeche <= 20)
        {
            var personRow = Tabelle8.FirstOrDefault(x => x.Key == PersonenFlaeche);
            if (personRow.Value is not null) personRow.Value.IsSelected = true;
        }

        if (NennlastTabelle6 > 0 && NennlastTabelle6 <= 2500)
        {
            var loadTable6Row = Tabelle6.FirstOrDefault(x => x.Key == NennlastTabelle6);
            if (loadTable6Row.Value is not null)
            {
                loadTable6Row.Value.IsSelected = true;
            }
            else
            {
                var lowTable6Entry = Tabelle6.Where(x => x.Key < NennlastTabelle6).Last();
                var highTable6Entry = Tabelle6.Where(x => x.Key > NennlastTabelle6).First();
                if (lowTable6Entry.Value is not null) lowTable6Entry.Value.IsSelected = true;
                if (lowTable6Entry.Value is not null) highTable6Entry.Value.IsSelected = true;
            }
        }

        if (NennlastTabelle7 > 0 && NennlastTabelle7 <= 1600)
        {
            var loadTable7Row = Tabelle7.FirstOrDefault(x => x.Key == NennlastTabelle7);
            if (loadTable7Row.Value is not null)
            {
                loadTable7Row.Value.IsSelected = true;
            }
            else
            {
                var lowTable7Entry = Tabelle7.Where(x => x.Key < NennlastTabelle7).Last();
                var highTable7Entry = Tabelle7.Where(x => x.Key > NennlastTabelle7).First();
                if (lowTable7Entry.Value is not null) lowTable7Entry.Value.IsSelected = true;
                if (lowTable7Entry.Value is not null) highTable7Entry.Value.IsSelected = true;
            }
        }
    }

    // ToDo Türdaten aus Datenbankladen Beispieldaten einer TTK 25
    // Logic zur Auswahl fehlt noch 
    // CarDoor Model am sinnvollsten schon bei der AuswahlTüren erstellen und in einem ComponentModelsdictionary verwalten

    private CarDoorDesignParameter GetCarDoorDetails([CallerMemberName] string membername = "")
    {
        // Workaround da TürModell aktuell noch keine Türbreite besitzt
        var tuerbreite = membername switch
        {
            nameof(KabinenTuerA) => TuerbreiteA,
            nameof(KabinenTuerB) => TuerbreiteB,
            nameof(KabinenTuerC) => TuerbreiteC,
            nameof(KabinenTuerD) => TuerbreiteD,
            _ => 0,
        };

        return new CarDoorDesignParameter
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
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        MarkTableSelectedRow();
        SetPersonen();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();

    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}