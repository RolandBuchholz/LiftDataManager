using System.Runtime.CompilerServices;

namespace LiftDataManager.ViewModels;

public class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware
{

    private Dictionary<int, double> Tabelle6 { get; set; } = new();
    private Dictionary<int, double> Tabelle7 { get; set; } = new();
    private Dictionary<int, double> Tabelle8 { get; set; } = new();

    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
        {
            if (m is not null && m.Value.IsDirty)
            {
                SetInfoSidebarPanelText(m);
                await CheckUnsavedParametresAsync();
            }
        });
        FillTabelle6WithData();
        FillTabelle7WithData();
        FillTabelle8WithData();
    }

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);

    public string Aufzugstyp => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Aufzugstyp");
    public string AufzugsArt => !string.IsNullOrWhiteSpace(Aufzugstyp) ? Aufzugstyp.Contains("Personen") ? "Personenaufzug" : "Lastenaufzug" : "Aufzugstyp noch nicht gewählt !";
    public string AufzugsArt2 => !string.IsNullOrWhiteSpace(Aufzugstyp) ? Aufzugstyp.Contains("Seil") ? "(Seil)" : "(Hydraulik)" : "";

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

    public CarDoor KabinenTuerA => ZugangA ? GetCarDoorDetails() : null;
    public CarDoor KabinenTuerB => ZugangB ? GetCarDoorDetails() : null;
    public CarDoor KabinenTuerC => ZugangC ? GetCarDoorDetails() : null;
    public CarDoor KabinenTuerD => ZugangD ? GetCarDoorDetails() : null;

    public string InfoZugangA => NutzflaecheZugangA == 0 && ZugangA ? " Tiefe < 100" : string.Empty ;
    public string InfoZugangB => NutzflaecheZugangB == 0 && ZugangB ? " Tiefe < 100" : string.Empty;
    public string InfoZugangC => NutzflaecheZugangC == 0 && ZugangC ? " Tiefe < 100" : string.Empty;
    public string InfoZugangD => NutzflaecheZugangD == 0 && ZugangD ? " Tiefe < 100" : string.Empty;


    public double NutzflaecheKabine => Math.Round(Kabinenbreite * Kabinentiefe / Math.Pow(10, 6), 2);
    public double NutzflaecheZugangA => ZugangA ? GetCarDoorArea(KabinenTuerA) : 0 ;
    public double NutzflaecheZugangB => ZugangB ? GetCarDoorArea(KabinenTuerB) : 0 ;
    public double NutzflaecheZugangC => ZugangC ? GetCarDoorArea(KabinenTuerC) : 0 ;
    public double NutzflaecheZugangD => ZugangD ? GetCarDoorArea(KabinenTuerD) : 0 ;
    public double NutzflaecheGesamt => Math.Round(NutzflaecheKabine + NutzflaecheZugangA + NutzflaecheZugangB + NutzflaecheZugangC + NutzflaecheZugangD, 2) ;


    public double NennlastTabelle6 => GetLoadFromTable(Tabelle6);
    public double NennlastTabelle7 => GetLoadFromTable(Tabelle7);
    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");


    public string ErgebnisNennlast => ValdidateNennlast() ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";
    public SolidColorBrush ErgebnisNennlastColor => ValdidateNennlast() ? successColor : failureColor;

    private double GetLoadFromTable(Dictionary<int,double> table, [CallerMemberName] string membername = "")
    {
        if (table == null) { return 0; };
        if (NutzflaecheGesamt <=0) { return 0; };

        if (membername == nameof(NennlastTabelle6) && NutzflaecheGesamt > 5.0)
        {
            return 2500 + (NutzflaecheGesamt - 5.0) / 0.16 * 100;
        };

        if (membername == nameof(NennlastTabelle6) && NutzflaecheGesamt < 0.37)
        {
            return 0;
        };

        if (membername == nameof(NennlastTabelle7) && NutzflaecheGesamt > 5.04)
        {
            return 1600 + (NutzflaecheGesamt - 5.04) / 0.40 * 100;
        };

        if (membername == nameof(NennlastTabelle7) && NutzflaecheGesamt < 1.68) 
        {
            return 0;
        };

        if (table.Any(x => x.Value == NutzflaecheGesamt))
        {
            return table.FirstOrDefault(x => x.Value == NutzflaecheGesamt).Key;
        };

        var lowTableEntry = table.Where(x => x.Value < NutzflaecheGesamt).Last();
        var highTableEntry = table.Where(x => x.Value > NutzflaecheGesamt).First();
        return Math.Round(lowTableEntry.Key + (highTableEntry.Key - lowTableEntry.Key) / (highTableEntry.Value - lowTableEntry.Value) * (NutzflaecheGesamt - lowTableEntry.Value));
    }


    private bool ValdidateNennlast()
    {
        if (Nennlast >= NennlastTabelle6){return true;}
        if (AufzugsArt == "Lastenaufzug" && AufzugsArt2== "(Hydraulik)" && Nennlast >= NennlastTabelle7){return true;}
        return false;    
    }

    private double GetCarDoorArea(CarDoor kabinenTuer, [CallerMemberName] string membername = "")
    {
        var tuerEinbau = membername switch
        {
            nameof(NutzflaecheZugangA) => TuerEinbauA,
            nameof(NutzflaecheZugangB) => TuerEinbauB,
            nameof(NutzflaecheZugangC) => TuerEinbauC,
            nameof(NutzflaecheZugangD) => TuerEinbauD,
            _ => 0,
        };

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
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel * kabinenTuer.TuerFluegelBreite + 2*kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),

            4 => Math.Round((kabinenTuer.Tuerbreite / 2 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel / 2 * kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),

            6 => Math.Round((kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            2 * kabinenTuer.Tuerbreite / 3 * (kabinenTuer.TuerFluegelBreite + kabinenTuer.TuerFluegelAbstand) +
                            kabinenTuer.Tuerbreite * (tuerEinbau - (kabinenTuer.AnzahlTuerFluegel /2 * kabinenTuer.TuerFluegelBreite + 2 * kabinenTuer.TuerFluegelAbstand))) / Math.Pow(10, 6), 3),
            _ => 0,
        };
    }

    public int Personen75kg => (int)(Nennlast / 75);
    public int PersonenFlaeche => GetPersonenFromTable(Tabelle8);
    public int PersonenBerechnet => (Personen75kg > PersonenFlaeche) ? PersonenFlaeche : Personen75kg;

    private int GetPersonenFromTable(Dictionary<int, double> table)
    {
        if (table == null) { return 0; };
        if (NutzflaecheGesamt < 0.28) { return 0; };

        if ( NutzflaecheGesamt > 3.13)
        {
            return Convert.ToInt32(20 + (NutzflaecheGesamt - 3.13) / 0.115);
        };

        if (table.Any(x => x.Value == NutzflaecheGesamt))
        {
            return table.FirstOrDefault(x => x.Value == NutzflaecheGesamt).Key;
        };

        return table.Where(x => x.Value < NutzflaecheGesamt).Last().Key;
    }

    // ToDo RequestMessage für Personenanzahl

    public void SetPersonen()
    {
        ParamterDictionary["var_Personen"].Value = Convert.ToString(PersonenBerechnet);
    }

    // ToDo Türdaten aus Datenbankladen Beispieldaten einer TTK 25
    // Logic zur Auswahl fehlt noch 
    // CarDoor Model am sinnvollsten schon bei der AuswahlTüren erstellen und in einem ComponentModelsdictionary verwalten

    private CarDoor GetCarDoorDetails([CallerMemberName] string membername = "")
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
        
        return new CarDoor
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

    private void FillTabelle6WithData()
    {
        Tabelle6.Add(100, 0.37);
        Tabelle6.Add(180, 0.58);
        Tabelle6.Add(225, 0.70);
        Tabelle6.Add(300, 0.90);
        Tabelle6.Add(375, 1.10); 
        Tabelle6.Add(400, 1.17);
        Tabelle6.Add(450, 1.30);
        Tabelle6.Add(525, 1.45);
        Tabelle6.Add(600, 1.60);
        Tabelle6.Add(630, 1.66);
        Tabelle6.Add(675, 1.75);
        Tabelle6.Add(750, 1.90);
        Tabelle6.Add(800, 2.00);
        Tabelle6.Add(825, 2.05);
        Tabelle6.Add(900, 2.20);
        Tabelle6.Add(975, 2.35);
        Tabelle6.Add(1000, 2.40);
        Tabelle6.Add(1050, 2.50);
        Tabelle6.Add(1125, 2.65);
        Tabelle6.Add(1200, 2.80);
        Tabelle6.Add(1250, 2.90);
        Tabelle6.Add(1275, 2.95);
        Tabelle6.Add(1350, 3.10);
        Tabelle6.Add(1425, 3.25);
        Tabelle6.Add(1500, 3.40);
        Tabelle6.Add(1600, 3.56);
        Tabelle6.Add(2000, 4.20);
        Tabelle6.Add(2500, 5.00);
    }

    private void FillTabelle7WithData()
    {
        Tabelle7.Add(400, 1.68);
        Tabelle7.Add(450, 1.84);
        Tabelle7.Add(525, 2.08);
        Tabelle7.Add(600, 2.32);
        Tabelle7.Add(630, 2.42);
        Tabelle7.Add(675, 2.56);
        Tabelle7.Add(750, 2.80);
        Tabelle7.Add(800, 2.96);
        Tabelle7.Add(825, 3.04);
        Tabelle7.Add(900, 3.28);
        Tabelle7.Add(975, 3.52);
        Tabelle7.Add(1000, 3.60);
        Tabelle7.Add(1050, 3.72);
        Tabelle7.Add(1125, 3.90);
        Tabelle7.Add(1200, 4.08);
        Tabelle7.Add(1250, 4.20);
        Tabelle7.Add(1275, 4.26);
        Tabelle7.Add(1350, 4.44);
        Tabelle7.Add(1425, 4.62);
        Tabelle7.Add(1500, 4.80);
        Tabelle7.Add(1600, 5.04);
    }

    private void FillTabelle8WithData()
    {
        Tabelle8.Add(1, 0.28);
        Tabelle8.Add(2, 0.49);
        Tabelle8.Add(3, 0.60);
        Tabelle8.Add(4, 0.79);
        Tabelle8.Add(5, 0.98);
        Tabelle8.Add(6, 1.17);
        Tabelle8.Add(7, 1.31);
        Tabelle8.Add(8, 1.45);
        Tabelle8.Add(9, 1.59);
        Tabelle8.Add(10, 1.73);
        Tabelle8.Add(11, 1.87);
        Tabelle8.Add(12, 2.01);
        Tabelle8.Add(13, 2.15);
        Tabelle8.Add(14, 2.29);
        Tabelle8.Add(15, 2.43);
        Tabelle8.Add(16, 2.57);
        Tabelle8.Add(17, 2.71);
        Tabelle8.Add(18, 2.85);
        Tabelle8.Add(19, 2.99);
        Tabelle8.Add(20, 3.13);
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
        // ToDo RequestMessage für Personenanzahl
        SetPersonen();
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
