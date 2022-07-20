namespace LiftDataManager.ViewModels;

public class KabinenLüftungViewModel : DataViewModelBase, INavigationAware
{
    public KabinenLüftungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);

    private readonly int tuerspalt = 4;
    private readonly int luftspaltoeffnung = 10;
    private readonly int anzahlLuftspaltoeffnungenFT = 2;

    public int Tuerspalt => tuerspalt;
    public int Luftspaltoeffnung => luftspaltoeffnung;

    public double A_Kabine_1Pozent => (!string.IsNullOrWhiteSpace(ParamterDictionary["var_A_Kabine"].Value) ? Convert.ToDouble(ParamterDictionary["var_A_Kabine"].Value, CultureInfo.CurrentCulture) : 0) * 10000;
    public double Belueftung_1Seite => (!string.IsNullOrWhiteSpace(ParamterDictionary["var_KTI"].Value) ? Convert.ToDouble(ParamterDictionary["var_KTI"].Value, CultureInfo.CurrentCulture) : 0) * Luftspaltoeffnung;
    public double Belueftung_2Seiten => (!string.IsNullOrWhiteSpace(ParamterDictionary["var_KTI"].Value) ? Convert.ToDouble(ParamterDictionary["var_KTI"].Value, CultureInfo.CurrentCulture) : 0) * Luftspaltoeffnung * 2;
    
    public string ErgebnisBelueftungDecke => (Belueftung_2Seiten > A_Kabine_1Pozent) ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";
    public SolidColorBrush ErgebnisBelueftungDeckeColor => (Belueftung_2Seiten > A_Kabine_1Pozent) ? successColor : failureColor;

    public bool ZugangA => !string.IsNullOrWhiteSpace(ParamterDictionary["var_ZUGANSSTELLEN_A"].Value) && Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_A"].Value, CultureInfo.CurrentCulture);
    public bool ZugangB => !string.IsNullOrWhiteSpace(ParamterDictionary["var_ZUGANSSTELLEN_B"].Value) && Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_B"].Value, CultureInfo.CurrentCulture);
    public bool ZugangC => !string.IsNullOrWhiteSpace(ParamterDictionary["var_ZUGANSSTELLEN_C"].Value) && Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_C"].Value, CultureInfo.CurrentCulture);
    public bool ZugangD => !string.IsNullOrWhiteSpace(ParamterDictionary["var_ZUGANSSTELLEN_D"].Value) && Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_D"].Value, CultureInfo.CurrentCulture);

    public int AnzahlKabinentueren => Convert.ToInt32(ZugangA) + Convert.ToInt32(ZugangB) + Convert.ToInt32(ZugangC) + Convert.ToInt32(ZugangD);
    public int AnzahlKabinentuerfluegel => (!string.IsNullOrWhiteSpace(ParamterDictionary["var_AnzahlTuerfluegel"].Value) ? Convert.ToInt32(ParamterDictionary["var_AnzahlTuerfluegel"].Value, CultureInfo.CurrentCulture) : 0);

    public int AnzahlLuftspaltoeffnungenTB => AnzahlKabinentueren * 2;
    public int AnzahlLuftspaltoeffnungenTH => AnzahlKabinentueren * AnzahlKabinentuerfluegel;
    public double FlaecheLuftspaltoeffnungenTB => AnzahlLuftspaltoeffnungenTB * tuerspalt * (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TB"].Value) ? Convert.ToDouble(ParamterDictionary["var_TB"].Value, CultureInfo.CurrentCulture) : 0);
    public double FlaecheLuftspaltoeffnungenTH => AnzahlLuftspaltoeffnungenTH * tuerspalt * (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TH"].Value) ? Convert.ToDouble(ParamterDictionary["var_TH"].Value, CultureInfo.CurrentCulture) : 0);
    public double EntlueftungTuerspalten50Pozent => (FlaecheLuftspaltoeffnungenTB + FlaecheLuftspaltoeffnungenTH) * 0.5;

    public int AnzahlLuftspaltoeffnungenFB => (AnzahlKabinentueren > 1) ? 0 : 1;
    public int AnzahlLuftspaltoeffnungenFT => anzahlLuftspaltoeffnungenFT;

    public double FlaecheLuftspaltoeffnungenFB => Math.Round(((!string.IsNullOrWhiteSpace(ParamterDictionary["var_KBI"].Value) ? Convert.ToInt32(ParamterDictionary["var_KBI"].Value, CultureInfo.CurrentCulture) : 0) /50) * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
    public double FlaecheLuftspaltoeffnungenFT => Math.Round(((!string.IsNullOrWhiteSpace(ParamterDictionary["var_KTI"].Value) ? Convert.ToInt32(ParamterDictionary["var_KTI"].Value, CultureInfo.CurrentCulture) : 0) / 50) * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));

    public double FlaecheLuftspaltoeffnungenFBGesamt => AnzahlLuftspaltoeffnungenFB * FlaecheLuftspaltoeffnungenFB;
    public double FlaecheLuftspaltoeffnungenFTGesamt => AnzahlLuftspaltoeffnungenFT * FlaecheLuftspaltoeffnungenFT;

    public double FlaecheEntLueftungSockelleisten => Math.Round(FlaecheLuftspaltoeffnungenFB + FlaecheLuftspaltoeffnungenFT);
    public double FlaecheEntLueftungGesamt => Math.Round(FlaecheEntLueftungSockelleisten + EntlueftungTuerspalten50Pozent);

    public string ErgebnisEntlueftung => (FlaecheEntLueftungGesamt > A_Kabine_1Pozent) ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";

    public SolidColorBrush ErgebnisEntlueftungColor => (FlaecheEntLueftungGesamt > A_Kabine_1Pozent) ? successColor : failureColor;

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
