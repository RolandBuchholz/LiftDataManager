namespace LiftDataManager.Core.Models.ComponentModels;

public class CarDoorDesignParameter
{
    public string Hersteller{get; set;} = string.Empty;
    public string Typ { get; set; } = "StandardCarDoorTyp";
    public string Baumusterpruefbescheinigung{get; set;} = "None";
    public double Tuerbreite { get; set; } = 800;
    public int AnzahlTuerFluegel { get; set; } = 2;
    public double TuerFluegelBreite { get; set; } = 36;
    public double TuerFluegelAbstand { get; set; } = 6;
    public double Schwellenbreite { get; set; }
    public double Gewicht{get; set;}
    public double MinimalerEinbau{get; set;}
}
