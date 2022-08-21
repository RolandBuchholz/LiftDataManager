namespace LiftDataManager.ViewModels;

public class KabinengewichtViewModel : DataViewModelBase, INavigationAware
{
    public KabinengewichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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
    }

    private readonly double gewichtAbgehaengteDecke = 24;
    private readonly double gewichtDeckebelegt = 12;
    private readonly double gewichtSpiegel = 6 * 2.5;
    private readonly double gewichtSpiegelPaneele = 1.5 * 2.7 + 0.85 * 10 + 4 * 2.5;
    private readonly double gewichtKlemmkasten = 12;
    private readonly double gewichtSchraubenZubehoer = 5;
    private readonly double gewichtAussenVerkleidung = 12;

    public double Kabinenbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
    public double Kabinentiefe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
    public double Kabinenhoehe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHLicht");
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

    public double KabinenBodengewichtStandard => Math.Round(Kabinenbreite * Kabinentiefe * 59 / Math.Pow(10, 6));

    public double KabinenBodengewichtVerstaerkt => Math.Round(Kabinenbreite * Kabinentiefe * Bodenblech * 7.85 / Math.Pow(10, 6) +
                                    ((((Kabinenbreite / 230) + 1 + ((Kabinenbreite > 2000) ? 1 : 0)) * Kabinentiefe / 1000 +
                                    (((Kabinenbreite > 1250) || (Kabinentiefe > 2350)) ? 3 : 2) * Kabinenbreite / 1000 +
                                    AnzahlKabinentueren * Tuerbreite / 1000) * BodenProfilGewicht));

    public double KabinenBodengewichtStandardMitWanne => Math.Round(Kabinenbreite * Kabinentiefe * 70.8 / Math.Pow(10, 6));
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
    public double BodenBelagGewicht => Math.Round(Kabinenbreite * Kabinentiefe * BodenBelagGewichtproQm / Math.Pow(10, 6));

    public double SchottenStaerke => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Materialstaerke");
    public double SchottenLaenge => (!ZugangB ? glasLaengeWandB > 0 ? 0 : Kabinentiefe : 0) +
                                     (!ZugangC ? glasLaengeWandC > 0 ? 0 : Kabinenbreite : 0) +
                                     (!ZugangD ? glasLaengeWandD > 0 ? 0 : Kabinentiefe : 0);
    public double Schottengewicht => SchottenLaenge > 0 ? Math.Round(SchottenStaerke * (Kabinenhoehe + 161) * 339 * 8 / Math.Pow(10, 6) * (SchottenLaenge / 275)) : 0;

    public double Haelsegewicht => Math.Round(SchottenStaerke * ((Kabinenhoehe + 86) *
                                   ((HalsL1 + HalsR1 + HalsL2 + HalsR2 + HalsL3 + HalsR3 + HalsL4 + HalsR4) + AnzahlKabinentueren * 214 +
                                   AnzahlKabinentueren * (Oeffnungsrichtung == "einseitig öffnend" ? AnzahlKabinentuerfluegel * 45 : 0))
                                   + AnzahlKabinentueren * (Tuerbreite + 104) * (Kabinenhoehe - Tuerhoehe + 99)) * 8 / Math.Pow(10, 6));

    public double AndidroehnGewicht => Math.Round(2 * 1000 * 150 * 1.36 / Math.Pow(10, 6) * (SchottenLaenge / 275));

    public double SchuerzeGewicht => Math.Round(AnzahlKabinentueren * (((SchuerzeVerstaerkt ? 5 : 3) * 1.5 * (Tuerbreite - 100) *
                                     266 + 1.5 * Tuerbreite * 800 + (Tuerbreite / 300 + 1) *
                                     1.5 * 380 * 109 + 7 * (SchuerzeVerstaerkt ? 5 : 3) * 100 * 81) * 8 / Math.Pow(10, 6)));

    public double Deckegewicht => Math.Round(Kabinenbreite * Kabinentiefe * 42.6 / Math.Pow(10, 6));

    public double AbgehaengteDeckeGewichtproQm => gewichtAbgehaengteDecke;
    public double AbgehaengteDeckeGewicht => AbgehaengteDecke ? Math.Round(Kabinenbreite * Kabinentiefe * AbgehaengteDeckeGewichtproQm / Math.Pow(10, 6)) : 0;

    public double DeckeBelegtGewichtproQm => gewichtDeckebelegt;
    public double DeckeBelegtGewicht => BelegteDecke ? Math.Round(Kabinenbreite * Kabinentiefe * DeckeBelegtGewichtproQm / Math.Pow(10, 6)) : 0;

    public double BelagAufDerDeckeGewichtproQm => GetGewichtSonderblech(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BelagAufDemKabinendach"));
    public double BelagAufDerDeckeGewicht => BelagAufDerDecke ? Math.Round(Kabinenbreite * Kabinentiefe * BelagAufDerDeckeGewichtproQm / Math.Pow(10, 6)) : 0;

    //<!--  KabinenAussattung  -->
    public double SpiegelGewichtproQm => gewichtSpiegel;
    public double SpiegelQm => (Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelB) + Convert.ToInt32(SpiegelC) + Convert.ToInt32(SpiegelD)) * SpiegelHoehe * SpiegelBreite / Math.Pow(10, 6);
    public double SpiegelGewicht => Math.Round(SpiegelGewichtproQm * SpiegelQm, 1);

    public double PaneeleGewichtproQm => GetGewichtPaneele(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Paneelmaterial"));
    public double PaneeleQm => (PaneelPosA || PaneelPosB || PaneelPosC || PaneelPosD) ?
                                Math.Round((((Convert.ToInt32(PaneelPosA) + Convert.ToInt32(PaneelPosC) * Kabinenbreite / 1000) +
                               ((Convert.ToInt32(PaneelPosB) + Convert.ToInt32(PaneelPosD)) * Kabinentiefe / 1000) - TableauBreite / 1000)) * (Kabinenhoehe - SockelleisteHoehe) / 1000 - SpiegelQm, 2) : 0;
    public double PaneeleGewicht => Math.Round(PaneeleGewichtproQm * PaneeleQm, 1);

    public double PaneeleSpiegelGewichtproQm => gewichtSpiegelPaneele;
    public double PaneeleSpiegelQm => Math.Round(SpiegelPaneel ?
                                                 SpiegelHoehe > 0 ? ((Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelC)) * Kabinenbreite / 1000 + (Convert.ToInt32(SpiegelD) + Convert.ToInt32(SpiegelB)) * Kabinentiefe / 1000) * SpiegelHoehe / 1000
                                                 : ((Convert.ToInt32(SpiegelA) + Convert.ToInt32(SpiegelC)) * Kabinenbreite / 1000 + (Convert.ToInt32(SpiegelD) + Convert.ToInt32(SpiegelB)) * Kabinentiefe / 1000) * ((Kabinenhoehe - SockelleisteHoehe) / 1000)
                                                 : 0, 2);
    public double PaneeleSpiegelGewicht => Math.Round(PaneeleSpiegelGewichtproQm * PaneeleSpiegelQm, 1);

    public double glasLaengeWandA => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Eingangswand")).StartsWith("VSG")
                                     ? (Kabinenbreite - Convert.ToInt32(ZugangA) * Tuerbreite) / 1000 : 0;
    public double glasLaengeWandB => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite B")
                                     ? (Kabinentiefe - Convert.ToInt32(ZugangB) * Tuerbreite) / 1000 : 0;
    public double glasLaengeWandC => ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Rueckwand")).StartsWith("VSG")
                                     ? (Kabinenbreite - Convert.ToInt32(ZugangC) * Tuerbreite) / 1000 : 0;
    public double glasLaengeWandD => (((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite D") ||
                                     ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Seitenwaende")).Contains("Seite B/D"))
                                     ? (Kabinentiefe - Convert.ToInt32(ZugangD) * Tuerbreite) / 1000 : 0;
    public string VSGTyp => (glasLaengeWandA > 1 || glasLaengeWandB > 1 || glasLaengeWandC > 1 || glasLaengeWandD > 1) ? "VSG 12" : "VSG 10";
    public double VSGGewichtproQm => (VSGTyp == "VSG 10") ? 25.0 : 30.0;
    public double VSGQm => Math.Round(((glasLaengeWandA > 0.1 ? glasLaengeWandA - 0.1 : 0) +
                                       (glasLaengeWandB > 0.1 ? glasLaengeWandB - 0.1 : 0) +
                                       (glasLaengeWandC > 0.1 ? glasLaengeWandC - 0.1 : 0) +
                                       (glasLaengeWandD > 0.1 ? glasLaengeWandD - 0.1 : 0)) * (Kabinenhoehe > 0 ? (Kabinenhoehe - 200) / 1000 : 0), 2);
    public double VSGGewicht => Math.Round(VSGQm * VSGGewichtproQm, 1);

    public double AussenVerkleidungGewichtproQm => gewichtAussenVerkleidung;
    public double AussenVerkleidungQm => (AussenverkleidungA || AussenverkleidungB || AussenverkleidungC || AussenverkleidungD) ?
                                        Math.Round((((Convert.ToInt32(AussenverkleidungA) + Convert.ToInt32(AussenverkleidungC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(AussenverkleidungB) + Convert.ToInt32(AussenverkleidungD)) * Kabinentiefe / 1000)) * KabinenhoeheAussen / 1000 -
                                        AnzahlKabinentueren * (Tuerbreite / 1000 * Tuerhoehe / 1000), 2) : 0;
    public double AussenVerkleidungGewicht => Math.Round(AussenVerkleidungQm * AussenVerkleidungGewichtproQm, 1);

    public double StossleisteGewichtproMeter => Math.Round(GetGewichtRammschutz(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Rammschutz")), 2);
    public double StossleisteLaenge => Math.Round(((Convert.ToInt32(RammschutzA) + Convert.ToInt32(RammschutzC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(RammschutzB) + Convert.ToInt32(RammschutzD)) * Kabinentiefe / 1000), 2);
    public double StossleisteGewicht => Math.Round(StossleisteGewichtproMeter * StossleisteLaenge, 1);

    public double HandlaufGewichtproMeter => Math.Round(GetGewichtHandlauf(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Handlauf")), 2);
    public double HandlaufLaenge => Math.Round(((Convert.ToInt32(HandlaufA) + Convert.ToInt32(HandlaufC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(HandlaufB) + Convert.ToInt32(HandlaufD)) * Kabinentiefe / 1000), 2);
    public double HandlaufGewicht => Math.Round(HandlaufGewichtproMeter * HandlaufLaenge, 1);

    public double SockelleisteGewichtproMeter => Math.Round(GetGewichtSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Sockelleiste")), 2);
    public double SockelleisteLaenge => Math.Round(((Convert.ToInt32(SockelleisteA) + Convert.ToInt32(SockelleisteC)) * Kabinenbreite / 1000) +
                                        ((Convert.ToInt32(SockelleisteB) + Convert.ToInt32(SockelleisteD)) * Kabinentiefe / 1000), 2);
    public double SockelleisteGewicht => Math.Round(SockelleisteLaenge * SockelleisteGewichtproMeter, 1);
    public double SockelleisteHoehe => GetHoeheSockelleiste(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Sockelleiste"));

    //<!--  KabinenZubehör  -->
    public double SchutzgelaenderAnzahlPfosten => Math.Ceiling(Kabinentiefe / 400 * 2 + Kabinenbreite / 400 + 2);
    public double SchutzgelaenderGewicht => Math.Round((2 * 3 * 1.5 * (Kabinentiefe > 0 ? (Kabinentiefe - 250) : 0) * 137 +
                                           (AnzahlKabinentueren > 1 ? 0 : 1) * 3 * 1.5 * Kabinenbreite * 137) * 8 / Math.Pow(10, 6) +
                                            (1.5 * 670 * 120 + 5 * 50 * 85) * 8 / Math.Pow(10, 6) * SchutzgelaenderAnzahlPfosten);
    public double KlemmkastenGewicht => gewichtKlemmkasten;
    public double SchraubenZubehoerGewicht => gewichtSchraubenZubehoer;
    public double TableauGewicht => GetGewichtTableau(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_KabTabKabinentableau"));
    public double TableauBreite => GetBreiteTableau(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_KabTabKabinentableau"));

    //<!--  KabinenGewichtDetail  -->

    public double KabinenGewichtGesamt => Math.Round(KabinenBodengewicht + BodenBelagGewicht + Schottengewicht + AndidroehnGewicht + Haelsegewicht + SchuerzeGewicht + Deckegewicht + AbgehaengteDeckeGewicht +
                                          DeckeBelegtGewicht + BelagAufDerDeckeGewicht + SpiegelGewicht + PaneeleGewicht + PaneeleSpiegelGewicht + VSGGewicht + AussenVerkleidungGewicht +
                                          StossleisteGewicht + HandlaufGewicht + SockelleisteGewicht + SchutzgelaenderGewicht + KlemmkastenGewicht + SchraubenZubehoerGewicht + TableauGewicht);
    public double KabinenKorrekturGewicht => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_F_Korr");
    public double KabinenTuerGewicht => KabinentuerGewichtA + KabinentuerGewichtB + KabinentuerGewichtC + KabinentuerGewichtD;
    public double FangrahmenGewicht => _CurrentSpeziProperties!.FangrahmenGewicht;
    public double FahrkorbGewicht => Math.Round(KabinenGewichtGesamt + KabinenKorrekturGewicht + KabinenTuerGewicht + FangrahmenGewicht);

    // ToDo Parameter aus Datenbank abrufen

    private double GetGewichtSonderblech(string bodenblech)
    {
        return bodenblech switch
        {
            "kein" => 0,
            "4/6 Träneblech grundiert" => 33.4,
            "6/8 Tränenblech grundiert" => 49.1,
            "4/6 Träneblech feuerverzinkt" => 33.4,
            "6/8 Tränenblech feuerverzinkt" => 49.1,
            "4/6 Träneblech Edelstahl" => 33.4,
            "6/8 Tränenblech Edelstahl" => 49.1,
            "3,0 SE-TB1 R11 Edelstahl" => 10.25,
            "3,5/5 Alu quintett" => 10.34,
            _ => 0,
        };
    }

    private double GetGewichtBodenprofil(string bodenprofil)
    {
        return bodenprofil switch
        {
            "80 x 50 x 5" => 6.28,
            "100 x 50 x 5" => 7.065,
            "120 x 60 x 6" => 10.1736,
            "140 x 60 x 6" => 11.1156,
            "UNP 140" => 16,
            "UNP 160" => 18.8,
            "UNP 180" => 22,
            _ => 0,
        };
    }

    private double GetGewichtPaneele(string paneele)
    {
        return paneele switch
        {
            "V2A 240 Korn auf Holzkern" => 27.145,
            "V2A Leinen auf Holzkern" => 27.145,
            "V2A Karo auf Holzkern" => 27.145,
            "V2A Raute auf Holzkern" => 27.145,
            "Glas 4 mm auf Holzkern" => 22.55,
            "ESG 6 mm auf Alu geklebt" => 20.46,
            "ESG 6 mm direkt geklebt" => 15,
            "HPL auf ALU" => 6.55,
            _ => 0,
        };
    }

    private double GetGewichtRammschutz(string rammschutz)
    {
        return rammschutz switch
        {
            "Holz 100x22 1-reihig" => 2.2,
            "Holz 100x22 2-reihig" => 4.4,
            "Holz 100x22 3-reihig" => 6.6,
            "Holz 200x22 1-reihig" => 4.4,
            "Holz 200x22 2-reihig" => 8.8,
            "Holz 200x22 3-reihig" => 13.2,
            "V2A 100x20 1-reihig" => 3.632,
            "V2A 100x20 2-reihig" => 7.264,
            "V2A 100x20 3-reihig" => 10.896,
            "V2A 200x20 1-reihig" => 6.832,
            "V2A 200x20 2-reihig" => 13.664,
            "V2A 200x20 3-reihig" => 20.496,
            "V4A 100x20 1-reihig" => 3.632,
            "V4A 100x20 2-reihig" => 7.264,
            "V4A 100x20 3-reihig" => 10.896,
            "V4A 200x20 1-reihig" => 6.832,
            "V4A 200x20 2-reihig" => 13.664,
            "V4A 200x20 3-reihig" => 20.496,
            "Kunststoff 100 x 20 1-reihig" => 2.72,
            "Kunststoff 100 x 20 2-reihig" => 5.44,
            "Kunststoff 100 x 20 3-reihig" => 8.16,
            "Kunststoff 200 x 20 1-reihig" => 5.44,
            "Kunststoff 200 x 20 2-reihig" => 10.88,
            "Kunststoff 200 x 20 3-reihig" => 16.32,
            "V2A - HL 13 / D 30" => 6,
            "V2A - HL 13 / D 30 umlfd./Bogen" => 6,
            "V2A - HL 14 / D 30" => 6,
            "V2A - HL 14 / D 30 mit Endbogen" => 6,
            "V2A - HL 13 / D 40" => 6,
            "V2A - HL 13 / D 40 umlfd./Bogen" => 6,
            "V2A - HL 14 / D 40" => 6,
            "V2A - HL 14 / D 40 mit Endbogen" => 6,
            "V4A - HL 13 / D 30" => 6,
            "V4A - HL 13 / D 30 umlfd./Bogen" => 6,
            "V4A - HL 14 / D 30" => 6,
            "V4A - HL 14 / D 30 mit Endbogen" => 6,
            "V4A - HL 13 / D 40" => 6,
            "V4A - HL 13 / D 40 umlfd./Bogen" => 6,
            "V4A - HL 14 / D 40" => 6,
            _ => 0,
        };
    }

    private double GetGewichtHandlauf(string sockelleiste)
    {
        return sockelleiste switch
        {
            "V2A - HL 13 / D 30" => 6,
            "V2A - HL 13 / D 30 umlfd./Bogen" => 6,
            "V2A - HL 14 / D 30" => 6,
            "V2A - HL 14 / D 30 mit Endbogen" => 6,
            "V2A - HL 13 / D 40" => 6,
            "V2A - HL 13 / D 40 umlfd./Bogen" => 6,
            "V2A - HL 14 / D 40" => 6,
            "V2A - HL 14 / D 40 mit Endbogen" => 6,
            "V4A - HL 13 / D 30" => 6,
            "V4A - HL 13 / D 30 umlfd./Bogen" => 6,
            "V4A - HL 14 / D 30" => 6,
            "V4A - HL 14 / D 30 mit Endbogen" => 6,
            "V4A - HL 13 / D 40" => 6,
            "V4A - HL 13 / D 40 umlfd./Bogen" => 6,
            "V4A - HL 14 / D 40" => 6,
            "V4A - HL 14 / D 40 mit Endbogen" => 6,
            "Buche - HL 18" => 3,
            _ => 0,
        };
    }

    private double GetGewichtSockelleiste(string sockelleiste)
    {
        return sockelleiste switch
        {
            "V2A 90 x 15" => 1.056,
            "V2A 85 x 22" => 1.936,
            "V2A 70 x 4" => 2.24,
            "V2A 100 x 15" => 1.136,
            "V4A 90 x 15" => 1.056,
            "V4A 85 x 22" => 1.936,
            "V4A 70 x 4" => 2.24,
            "V4A 100 x 15" => 1.136,
            _ => 0,
        };
    }

    private double GetHoeheSockelleiste(string sockelleiste)
    {
        return sockelleiste switch
        {
            "V2A 90 x 15" => 90,
            "V2A 85 x 22" => 85,
            "V2A 70 x 4" => 70,
            "V2A 100 x 15" => 100,
            "V4A 90 x 15" => 90,
            "V4A 85 x 22" => 85,
            "V4A 70 x 4" => 70,
            "V4A 100 x 15" => 100,
            _ => 0,
        };
    }

    private double GetGewichtTableau(string tableau)
    {
        return tableau switch
        {
            "Drehtableau / 1 Stück" => 21,
            "Drehtableau / 2 Stück" => 42,
            "Pulttableau" => 6,
            "Münchner-Standard 1 Stück" => 25,
            "Münchner-Standard 2 Stück" => 50,
            "Klapptableau / 1 Stück" => 20,
            "Klapptableau / 2 Stück" => 40,
            "aufgesetzte Tafel / 1 Stück" => 10,
            "aufgesetzte Tafel / 2 Stück" => 20,
            "Drehtableau / 1 Stück & Pulttableau" => 27,
            "Drehtableau / 2 Stück & Pulttableau" => 49,
            "Klapptableau / 1 Stück & Pulttableau" => 26,
            "Klapptableau / 2 Stück & Pulttableau" => 46,
            _ => 0,
        };
    }

    private double GetBreiteTableau(string tableau)
    {
        return tableau switch
        {
            "Drehtableau / 1 Stück" => 250,
            "Drehtableau / 2 Stück" => 500,
            "Pulttableau" => 0,
            "Münchner-Standard 1 Stück" => 250,
            "Münchner-Standard 2 Stück" => 500,
            "Klapptableau / 1 Stück" => 250,
            "Klapptableau / 2 Stück" => 500,
            "aufgesetzte Tafel / 1 Stück" => 0,
            "aufgesetzte Tafel / 2 Stück" => 0,
            "Drehtableau / 1 Stück & Pulttableau" => 250,
            "Drehtableau / 2 Stück & Pulttableau" => 500,
            "Klapptableau / 1 Stück & Pulttableau" => 250,
            "Klapptableau / 2 Stück & Pulttableau" => 500,
            _ => 0,
        };
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
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
