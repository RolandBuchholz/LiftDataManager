using CommunityToolkit.Mvvm.ComponentModel;

namespace LiftDataManager.Core.Models.CalculationResultsModels;

public partial class CarWeightResult : ObservableObject
{
    public int AnzahlKabinentueren { get; set; }
    public int AnzahlKabinentuerfluegel { get; set; }
    public bool SchuerzeVerstaerkt { get; set; }

    public bool ZugangA { get; set; }
    public bool ZugangB { get; set; }
    public bool ZugangC { get; set; }
    public bool ZugangD { get; set; }

    public double BodenBelagGewichtproQm { get; set; }
    public double AbgehaengteDeckeGewichtproQm { get; set; }
    public double DeckeBelegtGewichtproQm { get; set; }
    public double SpiegelGewichtproQm { get; set; }
    public double PaneeleSpiegelGewichtproQm { get; set; }
    public double PaneeleGewichtproQm { get; set; }
    public double AussenVerkleidungGewichtproQm { get; set; }
    public double BelagAufDerDeckeGewichtproQm { get; set; }
    public double VSGGewichtproQm { get; set; }
    public int AnzahlReihenStossleiste { get; set; }
    public double StossleisteGewichtproMeter { get; set; }
    public double HandlaufGewichtproMeter { get; set; }
    public double SockelleisteGewichtproMeter { get; set; }

    public double GewichtKlemmkasten { get; set; }
    public double GewichtSchraubenZubehoer { get; set; }

    public double GewichtAbgehaengteDecke { get; set; }
    public double BodenProfilGewicht { get; set; }
    public double Bodenblech { get; set; }
    public double KabinenBodengewicht { get; set; }
    public double BodenBelagGewicht { get; set; }
    public double Schottengewicht { get; set; }
    public double Haelsegewicht { get; set; }
    public double AndidroehnGewicht { get; set; }
    public double SchuerzeGewicht { get; set; }
    public double Deckegewicht { get; set; }
    public double DeckeBelegtGewicht { get; set; }
    public double BelagAufDerDeckeGewicht { get; set; }
    public double SpiegelQm { get; set; }
    public double SpiegelGewicht { get; set; }
    public double PaneeleQm { get; set; }
    public double PaneeleGewicht { get; set; }
    public double PaneeleSpiegelQm { get; set; }
    public double PaneeleSpiegelGewicht { get; set; }
    public string? VSGTyp { get; set; }
    public double VSGQm { get; set; }
    public double VSGGewicht { get; set; }
    public double AussenVerkleidungQm { get; set; }
    public double AussenVerkleidungGewicht { get; set; }
    public double StossleisteLaenge { get; set; }
    public double StossleisteGewicht { get; set; }
    public double HandlaufLaenge { get; set; }
    public double HandlaufGewicht { get; set; }
    public double SockelleisteLaenge { get; set; }
    public double SockelleisteGewicht { get; set; }
    public double SchutzgelaenderAnzahlPfosten { get; set; }
    public double SchutzgelaenderGewicht { get; set; }
    public double TableauBreite { get; set; }
    public double TableauGewicht { get; set; }
    public double KlemmkastenGewicht { get; set; }
    public double SchraubenZubehoerGewicht { get; set; }
    public double KabinenGewichtGesamt { get; set; }
    public double KabinenTuerGewicht { get; set; }
    public double FangrahmenGewicht { get; set; }
    public double FahrkorbGewicht { get; set; }
}
