using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.ViewModels;

public class KabinengewichtViewModel : DataViewModelBase, INavigationAware, IRecipient<CarWeightRequestMessageAsync>, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;

    public KabinengewichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;
    }

    public void Receive(CarWeightRequestMessageAsync message)
    {
        if (!message.HasReceivedResponse)
        {
            message.Reply(new CalculatedValues
            {
                KabinenGewicht = KabinenGewichtGesamt,
                KabinenTuerenGewicht = KabinenTuerGewicht,
                FangrahmenGewicht = FangrahmenGewicht,
                Fahrkorbgewicht = FahrkorbGewicht
            });
        }
        IsActive = false;
    }

    private readonly double gewichtAbgehaengteDecke = 24;
    private readonly double gewichtDeckebelegt = 12;
    private readonly double gewichtSpiegel = 6 * 2.5;
    private readonly double gewichtSpiegelPaneele = 1.5 * 2.7 + 0.85 * 10 + 4 * 2.5;
    private readonly double gewichtKlemmkasten = 10;
    private readonly double gewichtSchraubenZubehoer = 5;
    private readonly double gewichtAussenVerkleidung = 12;

    public double Kabinenbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
    public double Kabinentiefe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
    public double KabineundAbgehaengteDeckeHoehe => AbgehaengteDecke ? LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHLicht") + 50 : 
                                                                       LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHLicht");
    public double KabinenhoeheAussen => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHA");
    public double Tuerbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
    public double Tuerhoehe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TH");
    public string Oeffnungsrichtung => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tueroeffnung");
    public bool ZugangA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_A");
    public bool ZugangB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
    public bool ZugangC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
    public bool ZugangD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");
    public string BodenTyp => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodentyp");
    public bool SchuerzeVerstaerkt => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Schuerzeverstaerkt");
    public bool AbgehaengteDecke => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_abgDecke");
    public bool BelegteDecke => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Decke")).StartsWith("Sichtseite belegt");


    
    public bool BelagAufDerDecke => !(((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BelagAufDemKabinendach")).StartsWith("kein") ||
                                    string.IsNullOrEmpty((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BelagAufDemKabinendach")));
    public double HalsL1 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L1");
    public double HalsR1 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_R1");
    public double HalsL2 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L2");
    public double HalsR2 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_R2");
    public double HalsL3 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L3");
    public double HalsR3 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_R3");
    public double HalsL4 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L4");
    public double HalsR4 => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_R4");

    public int AnzahlKabinentueren => Convert.ToInt32(ZugangA) + Convert.ToInt32(ZugangB) + Convert.ToInt32(ZugangC) + Convert.ToInt32(ZugangD);
    public int AnzahlKabinentuerfluegel => LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_AnzahlTuerfluegel");
    public double KabinentuerGewichtA => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht");
    public double KabinentuerGewichtB => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht_B");
    public double KabinentuerGewichtC => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht_C");
    public double KabinentuerGewichtD => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht_D");
    public double Bodenblech => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenblech");
    public double BodenProfilGewicht => Math.Round(GetGewichtBodenprofil(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BoPr")), 2);

    public double KabinenBodengewichtStandard => Kabinenbreite * Kabinentiefe * 59 / Math.Pow(10, 6);

    public double KabinenBodengewichtVerstaerkt => Kabinenbreite * Kabinentiefe * Bodenblech * 7.85 / Math.Pow(10, 6) +
                                    ((((Kabinenbreite / 230) + 1 + ((Kabinenbreite > 2000) ? 1 : 0)) * Kabinentiefe / 1000 +
                                    (((Kabinenbreite > 1250) || (Kabinentiefe > 2350)) ? 3 : 2) * Kabinenbreite / 1000 +
                                    AnzahlKabinentueren * Tuerbreite / 1000) * BodenProfilGewicht);

    public double KabinenBodengewichtStandardMitWanne => Kabinenbreite * Kabinentiefe * 70.8 / Math.Pow(10, 6);
    public double KabinenBodengewicht =>
        BodenTyp switch
        {
            "standard" => KabinenBodengewichtStandard,
            "verstärkt" => KabinenBodengewichtVerstaerkt,
            "standard mit Wanne" => KabinenBodengewichtStandardMitWanne,
            "extern" => 0,
            _ => 0,
        };

    public bool SpiegelA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SpiegelA");
    public bool SpiegelB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SpiegelB");
    public bool SpiegelC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SpiegelC");
    public bool SpiegelD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SpiegelD");
    public double SpiegelHoehe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_HoeheSpiegel");
    public double SpiegelBreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_BreiteSpiegel");

    public bool PaneelPosA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_PaneelPosA");
    public bool PaneelPosB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_PaneelPosB");
    public bool PaneelPosC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_PaneelPosC");
    public bool PaneelPosD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_PaneelPosD");

    public bool SpiegelPaneel => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SpiegelPaneel");

    public bool AussenverkleidungA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AussenverkleidungA");
    public bool AussenverkleidungB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AussenverkleidungB");
    public bool AussenverkleidungC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AussenverkleidungC");
    public bool AussenverkleidungD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AussenverkleidungD");

    public bool RammschutzA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_RammschutzA");
    public bool RammschutzB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_RammschutzB");
    public bool RammschutzC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_RammschutzC");
    public bool RammschutzD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_RammschutzD");

    public bool HandlaufA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_HandlaufA");
    public bool HandlaufB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_HandlaufB");
    public bool HandlaufC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_HandlaufC");
    public bool HandlaufD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_HandlaufD");

    public bool SockelleisteA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SockelleisteA");
    public bool SockelleisteB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SockelleisteB");
    public bool SockelleisteC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SockelleisteC");
    public bool SockelleisteD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_SockelleisteD");

    //<!--  KabinenAusführung  -->       
    public double BodenBelagGewichtproQm => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsgewicht");
    public double BodenBelagGewicht => Kabinenbreite * Kabinentiefe * BodenBelagGewichtproQm / Math.Pow(10, 6);

    public double SchottenStaerke => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Materialstaerke");
    public double SchottenLaenge => (!ZugangB ? GlasLaengeWandB > 0 ? 0 : Kabinentiefe : 0) +
                                     (!ZugangC ? GlasLaengeWandC > 0 ? 0 : Kabinenbreite : 0) +
                                     (!ZugangD ? GlasLaengeWandD > 0 ? 0 : Kabinentiefe : 0);
    public double Schottengewicht => SchottenLaenge > 0 ? SchottenStaerke * (KabineundAbgehaengteDeckeHoehe + 161) * 339 * 8 / Math.Pow(10, 6) * (SchottenLaenge / 275) : 0;

    public double Haelsegewicht => SchottenStaerke * ((KabineundAbgehaengteDeckeHoehe + 86) *
                                   ((HalsL1 + HalsR1 + HalsL2 + HalsR2 + HalsL3 + HalsR3 + HalsL4 + HalsR4) + AnzahlKabinentueren * 214 +
                                   AnzahlKabinentueren * (Oeffnungsrichtung == "einseitig öffnend" ? AnzahlKabinentuerfluegel * 45 : 0))
                                   + AnzahlKabinentueren * (Tuerbreite + 104) * (KabineundAbgehaengteDeckeHoehe - Tuerhoehe + 99)) * 8 / Math.Pow(10, 6);

    public double AndidroehnGewicht => 2 * 1000 * 150 * 1.36 / Math.Pow(10, 6) * (SchottenLaenge / 275);

    public double SchuerzeGewicht => (Kabinenbreite > 0 && Kabinenbreite > 0) ?
                                     AnzahlKabinentueren * (((SchuerzeVerstaerkt ? 5 : 3) * 1.5 * (Tuerbreite - 100) *
                                     266 + 1.5 * Tuerbreite * 800 + (Tuerbreite / 300 + 1) *
                                     1.5 * 380 * 109 + 7 * (SchuerzeVerstaerkt ? 5 : 3) * 100 * 81) * 8 / Math.Pow(10, 6)) : 0;

    public double Deckegewicht => Kabinenbreite * Kabinentiefe * 42.6 / Math.Pow(10, 6);

    public double AbgehaengteDeckeGewichtproQm => gewichtAbgehaengteDecke;
    public double AbgehaengteDeckeGewicht => AbgehaengteDecke ? Kabinenbreite * Kabinentiefe * AbgehaengteDeckeGewichtproQm / Math.Pow(10, 6) : 0;

    public double DeckeBelegtGewichtproQm => gewichtDeckebelegt;
    public double DeckeBelegtGewicht => BelegteDecke ? Kabinenbreite * Kabinentiefe * DeckeBelegtGewichtproQm / Math.Pow(10, 6) : 0;

    public double BelagAufDerDeckeGewichtproQm => GetGewichtSonderblech(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BelagAufDemKabinendach"));
    public double BelagAufDerDeckeGewicht => BelagAufDerDecke ? Math.Round(Kabinenbreite * Kabinentiefe * BelagAufDerDeckeGewichtproQm / Math.Pow(10, 6)) : 0;

    //<!--  KabinenAussattung  -->
    public double SpiegelGewichtproQm => gewichtSpiegel;
    public double SpiegelQm => !SpiegelPaneel ? (Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelB) + Convert.ToInt32(SpiegelC) + Convert.ToInt32(SpiegelD)) * SpiegelHoehe * SpiegelBreite / Math.Pow(10, 6) : 0;
    public double SpiegelGewicht => SpiegelGewichtproQm * SpiegelQm;

    public double PaneeleGewichtproQm => GetGewichtPaneele(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Paneelmaterial"));
    public double PaneeleQm => (PaneelPosA || PaneelPosB || PaneelPosC || PaneelPosD) ?
                                (((Convert.ToInt32(PaneelPosA) + Convert.ToInt32(PaneelPosC) * Kabinenbreite / 1000) +
                               ((Convert.ToInt32(PaneelPosB) + Convert.ToInt32(PaneelPosD)) * Kabinentiefe / 1000) - TableauBreite / 1000)) * (KabineundAbgehaengteDeckeHoehe - SockelleisteHoehe) / 1000 - SpiegelQm : 0;
    public double PaneeleGewicht => PaneeleGewichtproQm * PaneeleQm;

    public double PaneeleSpiegelGewichtproQm => gewichtSpiegelPaneele;
    public double PaneeleSpiegelQm => SpiegelPaneel ?
                                                 SpiegelHoehe > 0 ? ((Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelC)) * Kabinenbreite / 1000 + (Convert.ToInt32(SpiegelD) + Convert.ToInt32(SpiegelB)) * Kabinentiefe / 1000) * SpiegelHoehe / 1000
                                                 : ((Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelC)) * Kabinenbreite / 1000 + (Convert.ToInt32(SpiegelD) + Convert.ToInt32(SpiegelB)) * Kabinentiefe / 1000) * ((KabineundAbgehaengteDeckeHoehe - SockelleisteHoehe) / 1000)
                                                 : 0;
    public double PaneeleSpiegelGewicht => PaneeleSpiegelGewichtproQm * PaneeleSpiegelQm;

    public double GlasLaengeWandA => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Eingangswand")).StartsWith("VSG")
                                     ? (Kabinenbreite - Convert.ToInt32(ZugangA) * Tuerbreite) / 1000 : 0;
    public double GlasLaengeWandB => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite B")
                                     ? (Kabinentiefe - Convert.ToInt32(ZugangB) * Tuerbreite) / 1000 : 0;
    public double GlasLaengeWandC => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Rueckwand")).StartsWith("VSG")
                                     ? (Kabinenbreite - Convert.ToInt32(ZugangC) * Tuerbreite) / 1000 : 0;
    public double GlasLaengeWandD => (((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite D") ||
                                     ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite B/D"))
                                     ? (Kabinentiefe - Convert.ToInt32(ZugangD) * Tuerbreite) / 1000 : 0;
    public string VSGTyp => (GlasLaengeWandA > 1 || GlasLaengeWandB > 1 || GlasLaengeWandC > 1 || GlasLaengeWandD > 1) ? "VSG 12" : "VSG 10";
    public double VSGGewichtproQm => (VSGTyp == "VSG 10") ? 25.0 : 30.0;
    public double VSGQm => ((GlasLaengeWandA > 0.1 ? GlasLaengeWandA - 0.1 : 0) +
                                       (GlasLaengeWandB > 0.1 ? GlasLaengeWandB - 0.1 : 0) +
                                       (GlasLaengeWandC > 0.1 ? GlasLaengeWandC - 0.1 : 0) +
                                       (GlasLaengeWandD > 0.1 ? GlasLaengeWandD - 0.1 : 0)) * (KabineundAbgehaengteDeckeHoehe > 0 ? (KabineundAbgehaengteDeckeHoehe - 200) / 1000 : 0);
    public double VSGGewicht => VSGQm * VSGGewichtproQm;

    public double AussenVerkleidungGewichtproQm => gewichtAussenVerkleidung;
    public double AussenVerkleidungQm => (AussenverkleidungA || AussenverkleidungB || AussenverkleidungC || AussenverkleidungD) ?
                                        (((Convert.ToInt32(AussenverkleidungA) + Convert.ToInt32(AussenverkleidungC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(AussenverkleidungB) + Convert.ToInt32(AussenverkleidungD)) * Kabinentiefe / 1000)) * KabinenhoeheAussen / 1000 -
                                        AnzahlKabinentueren * (Tuerbreite / 1000 * Tuerhoehe / 1000) : 0;
    public double AussenVerkleidungGewicht => Math.Round(AussenVerkleidungQm * AussenVerkleidungGewichtproQm, 0);

    public double StossleisteGewichtproMeter => Math.Round(GetGewichtRammschutz(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Rammschutz")), 2);
    public double StossleisteLaenge => ((Convert.ToInt32(RammschutzA) + Convert.ToInt32(RammschutzC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(RammschutzB) + Convert.ToInt32(RammschutzD)) * Kabinentiefe / 1000);
    public double StossleisteGewicht => StossleisteGewichtproMeter * StossleisteLaenge;

    public double HandlaufGewichtproMeter => GetGewichtHandlauf(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Handlauf"));
    public double HandlaufLaenge => ((Convert.ToInt32(HandlaufA) + Convert.ToInt32(HandlaufC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(HandlaufB) + Convert.ToInt32(HandlaufD)) * Kabinentiefe / 1000);
    public double HandlaufGewicht => HandlaufGewichtproMeter * HandlaufLaenge;

    public double SockelleisteGewichtproMeter => GetGewichtSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Sockelleiste"));
    public double SockelleisteLaenge => ((Convert.ToInt32(SockelleisteA) + Convert.ToInt32(SockelleisteC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(SockelleisteB) + Convert.ToInt32(SockelleisteD)) * Kabinentiefe / 1000);
    public double SockelleisteGewicht => SockelleisteLaenge * SockelleisteGewichtproMeter;
    public double SockelleisteHoehe => GetHoeheSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Sockelleiste"));

    //<!--  KabinenZubehör  -->
    public double SchutzgelaenderAnzahlPfosten => Math.Ceiling(Kabinentiefe / 400 * 2 + Kabinenbreite / 400 + 2);
    public double SchutzgelaenderGewicht => (Kabinenbreite > 0 && Kabinenbreite > 0) ?
                                            ((2 * 3 * 1.5 * (Kabinentiefe > 0 ? (Kabinentiefe - 250) : 0) * 137 +
                                            (AnzahlKabinentueren > 1 ? 0 : 1) * 3 * 1.5 * Kabinenbreite * 137) * 8 / Math.Pow(10, 6) +
                                            (1.5 * 670 * 120 + 5 * 50 * 85) * 8 / Math.Pow(10, 6) * SchutzgelaenderAnzahlPfosten) : 0;
    public double KlemmkastenGewicht => (Kabinenbreite > 0 && Kabinenbreite > 0) ? gewichtKlemmkasten : 0;
    public double SchraubenZubehoerGewicht => (Kabinenbreite > 0 && Kabinenbreite > 0 ) ? gewichtSchraubenZubehoer : 0;
    public double TableauGewicht => GetGewichtTableau(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_KabTabKabinentableau"));
    public double TableauBreite => GetBreiteTableau(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_KabTabKabinentableau"));

    //<!--  KabinenGewichtDetail  -->

    public double KabinenGewichtGesamt => KabinenBodengewicht + BodenBelagGewicht + Schottengewicht + AndidroehnGewicht + Haelsegewicht + SchuerzeGewicht + Deckegewicht + AbgehaengteDeckeGewicht +
                                          DeckeBelegtGewicht + BelagAufDerDeckeGewicht + SpiegelGewicht + PaneeleGewicht + PaneeleSpiegelGewicht + VSGGewicht + AussenVerkleidungGewicht +
                                          StossleisteGewicht + HandlaufGewicht + SockelleisteGewicht + SchutzgelaenderGewicht + KlemmkastenGewicht + SchraubenZubehoerGewicht + TableauGewicht;
    public double KabinenKorrekturGewicht => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_F_Korr");

    public bool VariableTuerdaten => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Variable_Tuerdaten");
    public double KabinenTuerGewicht => VariableTuerdaten ?  KabinentuerGewichtA + KabinentuerGewichtB + KabinentuerGewichtC + KabinentuerGewichtD : KabinentuerGewichtA * AnzahlKabinentueren ;

    public double FangrahmenGewicht => GetFangrahmengewichtAsync();

    private double GetFangrahmengewichtAsync()
    {

            if (!string.IsNullOrWhiteSpace(ParamterDictionary!["var_Rahmengewicht"].Value))
            {
                return LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Rahmengewicht");
            }
            else if (!string.IsNullOrWhiteSpace(ParamterDictionary!["var_Bausatz"].Value))
            {
                var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == ParamterDictionary!["var_Bausatz"].Value);
                if (carFrameType is null)
                    return 0;
                return carFrameType.CarFrameWeight;
            }
            else
            {
                return 0;
            }
    }

    public double FahrkorbGewicht
    {
        get
        {
            var fahrkorbGewicht = Math.Round(KabinenGewichtGesamt + KabinenKorrekturGewicht + KabinenTuerGewicht + FangrahmenGewicht);
            ParamterDictionary!["var_F"].Value = Convert.ToString(fahrkorbGewicht);
            return fahrkorbGewicht;
        }
    }

    //Database

    private double? GetGewichtSonderblech(string bodenblech)
    {
        if (string.IsNullOrEmpty(bodenblech)) return 0;
        var blech = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenblech && x.SpecialSheet == true);
        if (blech is null) return 0;
        return blech.WeightPerSquareMeter;
    }

    private double? GetGewichtBodenprofil(string bodenprofil)
    {
        if (string.IsNullOrEmpty(bodenprofil)) return 0;
        var profil = _parametercontext.Set<CarFloorProfile>().FirstOrDefault(x => x.Name == bodenprofil);
        if (profil is null) return 0;
        return profil.WeightPerMeter;
    }

    private double? GetGewichtPaneele(string paneele)
    {
        if (string.IsNullOrEmpty(paneele)) return 0;
        var coverPanel = _parametercontext.Set<CarCoverPanel>().FirstOrDefault(x => x.Name == paneele);
        if (coverPanel is null) return 0;
        return coverPanel.WeightPerSquareMeter;
    }

    private double? GetGewichtRammschutz(string rammschutz)
    {
        if (string.IsNullOrEmpty(rammschutz))return 0;
        var rammingProtection = _parametercontext.Set<RammingProtection>().FirstOrDefault(x => x.Name == rammschutz);
        if (rammingProtection is null)return 0;
        return rammingProtection.WeightPerMeter;
    }

    private double? GetGewichtHandlauf(string handlauf)
    {
        if (string.IsNullOrEmpty(handlauf)) return 0;
        var handrail = _parametercontext.Set<Handrail>().FirstOrDefault(x => x.Name == handlauf);
        if (handrail is null) return 0;
        return handrail.WeightPerMeter;
    }

    private double? GetGewichtSockelleiste(string sockelleiste)
    {
        if (string.IsNullOrEmpty(sockelleiste)) return 0;
        var skirtingBoard = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == sockelleiste);
        if (skirtingBoard is null) return 0;
        return skirtingBoard.WeightPerMeter;
    }

    private double? GetHoeheSockelleiste(string sockelleiste)
    {
        if (string.IsNullOrEmpty(sockelleiste)) return 0;
        var skirtingBoard = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == sockelleiste);
        if (skirtingBoard is null) return 0;
        return skirtingBoard.Height;
    }

    private double? GetGewichtTableau(string tableau)
    {
        if (string.IsNullOrEmpty(tableau)) return 0;
        var carPanel = _parametercontext.Set<CarPanel>().FirstOrDefault(x => x.Name == tableau);
        if (carPanel is null) return 0;
        return carPanel.Weight;
    }

    private double? GetBreiteTableau(string tableau)
    {
        if (string.IsNullOrEmpty(tableau)) return 0;
        var carPanel = _parametercontext.Set<CarPanel>().FirstOrDefault(x => x.Name == tableau);
        if (carPanel is null) return 0;
        return carPanel.Width;
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}