using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;

namespace LiftDataManager.ViewModels;

public class KabinenLüftungViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public KabinenLüftungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);

    private readonly int tuerspalt = 4;
    private readonly int luftspaltoeffnung = 10;
    private readonly int anzahlLuftspaltoeffnungenFT = 2;

    public int Tuerspalt => tuerspalt;
    public int Luftspaltoeffnung => luftspaltoeffnung;
    public double A_Kabine => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_A_Kabine");
    public double Kabinenbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
    public double Kabinentiefe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
    public double Tuerbreite => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
    public double Tuerhoehe => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TH");
    public bool ZugangA => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_A");
    public bool ZugangB => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
    public bool ZugangC => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
    public bool ZugangD => LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");
    public int AnzahlKabinentueren => Convert.ToInt32(ZugangA) + Convert.ToInt32(ZugangB) + Convert.ToInt32(ZugangC) + Convert.ToInt32(ZugangD);
    public int AnzahlKabinentuerfluegel => LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_AnzahlTuerfluegel");

    public double A_Kabine_1Pozent => A_Kabine * 10000;
    public double Belueftung_1Seite => Kabinentiefe * Luftspaltoeffnung;
    public double Belueftung_2Seiten => Kabinentiefe * Luftspaltoeffnung * 2;

    public string ErgebnisBelueftungDecke => (Belueftung_2Seiten > A_Kabine_1Pozent) ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";
    public SolidColorBrush ErgebnisBelueftungDeckeColor => (Belueftung_2Seiten > A_Kabine_1Pozent) ? successColor : failureColor;

    public int AnzahlLuftspaltoeffnungenTB => AnzahlKabinentueren * 2;
    public int AnzahlLuftspaltoeffnungenTH => AnzahlKabinentueren * AnzahlKabinentuerfluegel;
    public double FlaecheLuftspaltoeffnungenTB => AnzahlLuftspaltoeffnungenTB * tuerspalt * Tuerbreite;
    public double FlaecheLuftspaltoeffnungenTH => AnzahlLuftspaltoeffnungenTH * tuerspalt * Tuerhoehe;
    public double EntlueftungTuerspalten50Pozent => (FlaecheLuftspaltoeffnungenTB + FlaecheLuftspaltoeffnungenTH) * 0.5;

    public int AnzahlLuftspaltoeffnungenFB => (AnzahlKabinentueren > 1) ? 0 : 1;
    public int AnzahlLuftspaltoeffnungenFT => anzahlLuftspaltoeffnungenFT;

    public double FlaecheLuftspaltoeffnungenFB => Math.Round((Kabinenbreite / 50) * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));
    public double FlaecheLuftspaltoeffnungenFT => Math.Round((Kabinentiefe / 50) * ((Math.Pow(9, 2) * Math.PI / 4) + ((14 - 9) * 9)));

    public double FlaecheLuftspaltoeffnungenFBGesamt => AnzahlLuftspaltoeffnungenFB * FlaecheLuftspaltoeffnungenFB;
    public double FlaecheLuftspaltoeffnungenFTGesamt => AnzahlLuftspaltoeffnungenFT * FlaecheLuftspaltoeffnungenFT;

    public double FlaecheEntLueftungSockelleisten => Math.Round(FlaecheLuftspaltoeffnungenFB + FlaecheLuftspaltoeffnungenFT);
    public double FlaecheEntLueftungGesamt => Math.Round(FlaecheEntLueftungSockelleisten + EntlueftungTuerspalten50Pozent);

    public string ErgebnisEntlueftung => (FlaecheEntLueftungGesamt > A_Kabine_1Pozent) ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";
    public SolidColorBrush ErgebnisEntlueftungColor => (FlaecheEntLueftungGesamt > A_Kabine_1Pozent) ? successColor : failureColor;

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
