using System.Runtime.CompilerServices;
using LiftDataManager.Core.Messenger.Messages;
namespace LiftDataManager.ViewModels;

public class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware , IRecipient<AreaPersonsRequestMessage>
{

    public Dictionary<int, TableRow<int, double>> Tabelle6 { get; set; } = new();
    public Dictionary<int, TableRow<int, double>> Tabelle7 { get; set; } = new();
    public Dictionary<int, TableRow<int, double>> Tabelle8 { get; set; } = new();

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

        FillTablesWithData();
    }

    public NutzlastberechnungViewModel()
    {
        _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();

        if (_CurrentSpeziProperties.ParamterDictionary is not null)
        {
            ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
        }

        FillTablesWithData();
    }

    public void Receive(AreaPersonsRequestMessage message)
    {
        message.Reply(new CalculatedValues
        {
            Personen = PersonenBerechnet, 
            NutzflaecheKabine = NutzflaecheGesamt 
        });
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

    public CarDoor? KabinenTuerA => ZugangA ? GetCarDoorDetails() : null;
    public CarDoor? KabinenTuerB => ZugangB ? GetCarDoorDetails() : null;
    public CarDoor? KabinenTuerC => ZugangC ? GetCarDoorDetails() : null;
    public CarDoor? KabinenTuerD => ZugangD ? GetCarDoorDetails() : null;

    public string InfoZugangA => NutzflaecheZugangA == 0 && ZugangA ? " Tiefe < 100" : string.Empty;
    public string InfoZugangB => NutzflaecheZugangB == 0 && ZugangB ? " Tiefe < 100" : string.Empty;
    public string InfoZugangC => NutzflaecheZugangC == 0 && ZugangC ? " Tiefe < 100" : string.Empty;
    public string InfoZugangD => NutzflaecheZugangD == 0 && ZugangD ? " Tiefe < 100" : string.Empty;


    public double NutzflaecheKabine => Math.Round(Kabinenbreite * Kabinentiefe / Math.Pow(10, 6), 2);
    public double NutzflaecheZugangA => ZugangA ? GetCarDoorArea(KabinenTuerA) : 0;
    public double NutzflaecheZugangB => ZugangB ? GetCarDoorArea(KabinenTuerB) : 0;
    public double NutzflaecheZugangC => ZugangC ? GetCarDoorArea(KabinenTuerC) : 0;
    public double NutzflaecheZugangD => ZugangD ? GetCarDoorArea(KabinenTuerD) : 0;
    public double NutzflaecheGesamt => Math.Round(NutzflaecheKabine + NutzflaecheZugangA + NutzflaecheZugangB + NutzflaecheZugangC + NutzflaecheZugangD, 2);

    public double NennlastTabelle6 => GetLoadFromTable(Tabelle6);
    public double NennlastTabelle7 => GetLoadFromTable(Tabelle7);
    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");


    public string ErgebnisNennlast => ValdidateNennlast() ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";
    public SolidColorBrush ErgebnisNennlastColor => ValdidateNennlast() ? successColor : failureColor;

    private double GetLoadFromTable(Dictionary<int, TableRow<int, double>> table, [CallerMemberName] string membername = "")
    {
        TableRow<int, double>? nutzlast = null;
        if (table == null) { return 0; };
        if (NutzflaecheGesamt <= 0) { return 0; };

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

        if (table.Any(x => x.Value.SecondValue == NutzflaecheGesamt))
        {
            nutzlast = table.FirstOrDefault(x => x.Value.SecondValue == NutzflaecheGesamt).Value;
            nutzlast.IsSelected = true;
            return nutzlast.FirstValue;
        };

        var lowTableEntry = table.Where(x => x.Value.SecondValue < NutzflaecheGesamt).Last();
        lowTableEntry.Value.IsSelected = true;
        var highTableEntry = table.Where(x => x.Value.SecondValue > NutzflaecheGesamt).First();
        highTableEntry.Value.IsSelected = true;
        return Math.Round(lowTableEntry.Value.FirstValue + (highTableEntry.Value.FirstValue - lowTableEntry.Value.FirstValue) /
                (highTableEntry.Value.SecondValue - lowTableEntry.Value.SecondValue) * (NutzflaecheGesamt - lowTableEntry.Value.SecondValue));
    }
    private bool ValdidateNennlast()
    {
        if (Nennlast >= NennlastTabelle6) { return true; }
        if (AufzugsArt == "Lastenaufzug" && AufzugsArt2 == "(Hydraulik)" && Nennlast >= NennlastTabelle7) { return true; }
        return false;
    }

    private double GetCarDoorArea(CarDoor? kabinenTuer, [CallerMemberName] string membername = "")
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
    public int PersonenFlaeche => GetPersonenFromTable(Tabelle8);
    public int PersonenBerechnet => (Personen75kg > PersonenFlaeche) ? PersonenFlaeche : Personen75kg;

    private int GetPersonenFromTable(Dictionary<int, TableRow<int, double>> table)
    {
        TableRow<int, double>? personenAnzahl = null;
        if (table == null) { return 0; };
        if (NutzflaecheGesamt < 0.28) { return 0; };

        if (NutzflaecheGesamt > 3.13)
        {
            return Convert.ToInt32(20 + (NutzflaecheGesamt - 3.13) / 0.115);
        };

        if (table.Any(x => x.Value.SecondValue == NutzflaecheGesamt))
        {
            personenAnzahl = table.FirstOrDefault(x => x.Value.SecondValue == NutzflaecheGesamt).Value;
            personenAnzahl.IsSelected = true;
            return personenAnzahl.FirstValue;
        };
        personenAnzahl = table.Where(x => x.Value.SecondValue < NutzflaecheGesamt).Last().Value;
        personenAnzahl.IsSelected = true;
        return personenAnzahl.FirstValue;
    }

    public void SetPersonen()
    {
        ParamterDictionary!["var_Personen"].Value = Convert.ToString(PersonenBerechnet);
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

    private void FillTablesWithData()
    {
        var tabelle6 = new KeyValuePair<int, double>[]
        {
        new KeyValuePair<int, double>(100, 0.37),
        new KeyValuePair<int, double>(180, 0.58),
        new KeyValuePair<int, double>(225, 0.70),
        new KeyValuePair<int, double>(300, 0.90),
        new KeyValuePair<int, double>(375, 1.10),
        new KeyValuePair<int, double>(400, 1.17),
        new KeyValuePair<int, double>(450, 1.30),
        new KeyValuePair<int, double>(525, 1.45),
        new KeyValuePair<int, double>(600, 1.60),
        new KeyValuePair<int, double>(630, 1.66),
        new KeyValuePair<int, double>(675, 1.75),
        new KeyValuePair<int, double>(750, 1.90),
        new KeyValuePair<int, double>(800, 2.00),
        new KeyValuePair<int, double>(825, 2.05),
        new KeyValuePair<int, double>(900, 2.20),
        new KeyValuePair<int, double>(975, 2.35),
        new KeyValuePair<int, double>(1000, 2.40),
        new KeyValuePair<int, double>(1050, 2.50),
        new KeyValuePair<int, double>(1125, 2.65),
        new KeyValuePair<int, double>(1200, 2.80),
        new KeyValuePair<int, double>(1250, 2.90),
        new KeyValuePair<int, double>(1275, 2.95),
        new KeyValuePair<int, double>(1350, 3.10),
        new KeyValuePair<int, double>(1425, 3.25),
        new KeyValuePair<int, double>(1500, 3.40),
        new KeyValuePair<int, double>(1600, 3.56),
        new KeyValuePair<int, double>(2000, 4.20),
        new KeyValuePair<int, double>(2500, 5.00)
        };
        var tabelle7 = new KeyValuePair<int, double>[] {
        new KeyValuePair<int, double>(400, 1.68),
        new KeyValuePair<int, double>(450, 1.84),
        new KeyValuePair<int, double>(525, 2.08),
        new KeyValuePair<int, double>(600, 2.32),
        new KeyValuePair<int, double>(630, 2.42),
        new KeyValuePair<int, double>(675, 2.56),
        new KeyValuePair<int, double>(750, 2.80),
        new KeyValuePair<int, double>(800, 2.96),
        new KeyValuePair<int, double>(825, 3.04),
        new KeyValuePair<int, double>(900, 3.28),
        new KeyValuePair<int, double>(975, 3.52),
        new KeyValuePair<int, double>(1000, 3.60),
        new KeyValuePair<int, double>(1050, 3.72),
        new KeyValuePair<int, double>(1125, 3.90),
        new KeyValuePair<int, double>(1200, 4.08),
        new KeyValuePair<int, double>(1250, 4.20),
        new KeyValuePair<int, double>(1275, 4.26),
        new KeyValuePair<int, double>(1350, 4.44),
        new KeyValuePair<int, double>(1425, 4.62),
        new KeyValuePair<int, double>(1500, 4.80),
        new KeyValuePair<int, double>(1600, 5.04)
    };
        var tabelle8 = new KeyValuePair<int, double>[] {
        new KeyValuePair<int, double>(1, 0.28),
        new KeyValuePair<int, double>(2, 0.49),
        new KeyValuePair<int, double>(3, 0.60),
        new KeyValuePair<int, double>(4, 0.79),
        new KeyValuePair<int, double>( 5, 0.98),
        new KeyValuePair<int, double>( 6, 1.17),
        new KeyValuePair<int, double>( 7, 1.31),
        new KeyValuePair<int, double>(8, 1.45),
        new KeyValuePair<int, double>(9, 1.59),
        new KeyValuePair<int, double>(10, 1.73),
        new KeyValuePair<int, double>(11, 1.87),
        new KeyValuePair<int, double>(12, 2.01),
        new KeyValuePair<int, double>(13, 2.15),
        new KeyValuePair<int, double>(14, 2.29),
        new KeyValuePair<int, double>(15, 2.43),
        new KeyValuePair<int, double>(16, 2.57),
        new KeyValuePair<int, double>(17, 2.71),
        new KeyValuePair<int, double>(18, 2.85),
        new KeyValuePair<int, double>(19, 2.99),
        new KeyValuePair<int, double>(20, 3.13)
    };

        Tabelle6 = SetTableData(tabelle6, "kg", "m²");
        Tabelle7 = SetTableData(tabelle7, "kg", "m²");
        Tabelle8 = SetTableData(tabelle8, "Pers.", "m²");
    }

    private Dictionary<int, TableRow<int, double>> SetTableData(KeyValuePair<int, double>[] tabledata, string firstUnit, string secondUnit)
    {
        var dic = new Dictionary<int, TableRow<int, double>>();

        foreach (var item in tabledata)
        {
            dic.Add(item.Key, new TableRow<int, double>
            {
                FirstValue = item.Key,
                SecondValue = item.Value,
                FirstUnit = firstUnit,
                SecondUnit = secondUnit
            });
        }

        return dic;
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        SetPersonen();
        if (_CurrentSpeziProperties is not null && _CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}