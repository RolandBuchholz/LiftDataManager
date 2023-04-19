namespace LiftDataManager.Core.Models.CalculationResultsModels;

public class CarVentilationResult
{
    public int Tuerspalt { get; set; }
    public int Luftspaltoeffnung { get; set; }
    public int AnzahlLuftspaltoeffnungenFT { get; set; }

    public int AnzahlKabinentueren { get; set; }

    public double AKabine1Pozent { get; set; }
    public double Belueftung1Seite { get; set; }
    public double Belueftung2Seiten { get; set; }

    public bool ErgebnisBelueftungDecke { get; set; }

    public int AnzahlLuftspaltoeffnungenTB { get; set; }
    public int AnzahlLuftspaltoeffnungenTH { get; set; }
    public double FlaecheLuftspaltoeffnungenTB { get; set; }
    public double FlaecheLuftspaltoeffnungenTH { get; set; }
    public double EntlueftungTuerspalten50Pozent { get; set; }

    public int AnzahlLuftspaltoeffnungenFB { get; set; }

    public double FlaecheLuftspaltoeffnungenFB { get; set; }
    public double FlaecheLuftspaltoeffnungenFT { get; set; }

    public double FlaecheLuftspaltoeffnungenFBGesamt { get; set; }
    public double FlaecheLuftspaltoeffnungenFTGesamt { get; set; }

    public double FlaecheEntLueftungSockelleisten { get; set; }
    public double FlaecheEntLueftungGesamt { get; set; }

    public bool ErgebnisEntlueftung { get; set; }
}
