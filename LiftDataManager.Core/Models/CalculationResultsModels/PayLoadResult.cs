namespace LiftDataManager.Core.Models.CalculationResultsModels;

public class PayLoadResult
{
    public string? CargoTyp { get; set; }
    public string? DriveSystem { get; set; }
    public int AnzahlKabinentueren { get; set; }

    public bool ZugangA { get; set; }
    public bool ZugangB { get; set; }
    public bool ZugangC { get; set; }
    public bool ZugangD { get; set; }

    public double NutzflaecheKabine { get; set; }
    public double NutzflaecheZugangA { get; set; }
    public double NutzflaecheZugangB { get; set; }
    public double NutzflaecheZugangC { get; set; }
    public double NutzflaecheZugangD { get; set; }
    public double NutzflaecheGesamt { get; set; }

    public bool PayloadAllowed { get; set; }

    public int Personen75kg  {get; set; }
    public int PersonenFlaeche  {get; set; }
    public int PersonenBerechnet { get; set; }
}
