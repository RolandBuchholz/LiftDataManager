using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class KabineViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        GoToKabineDetailCommand = new RelayCommand(GoToKabineDetail);
    }

    public IRelayCommand GoToKabineDetailCommand
    {
        get;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null)
        {
            // ToDo Validation Service integrieren
            if (message.PropertyName == "var_Bodenbelag")
            {
                SetGewichtBodenbelag(message.NewValue);
            };
            SetInfoSidebarPanelText(message);
            //TODO Make Async
            _ = CheckUnsavedParametresAsync();
        }
    }

    private void GoToKabineDetail() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineDetailViewModel");

    //private void CheckDeckenhoehe()
    //{
    //    Debug.WriteLine("Kabinendecke wird überprüft");
    //    decimal bodenHoehe = Convert.ToDecimal(ParamterDictionary["var_KU"].Value);
    //    decimal kabinenHoeheInnen = Convert.ToDecimal(ParamterDictionary["var_KHLicht"].Value);
    //    decimal kabinenHoeheAussen = Convert.ToDecimal(ParamterDictionary["var_KHA"].Value);
    //    decimal deckenhoehe = Convert.ToDecimal(_Deckenhoehe);

    //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && kabinenHoeheAussen > 0 && deckenhoehe == 0)
    //    {
    //        Debug.WriteLine("Kabinendecke wird berechnet");
    //        decimal berechneteDeckenhoehe = kabinenHoeheAussen - kabinenHoeheInnen - bodenHoehe;
    //        _Deckenhoehe = Convert.ToString(berechneteDeckenhoehe);
    //    }

    //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && deckenhoehe > 0 && kabinenHoeheAussen ==0)
    //    {
    //        Debug.WriteLine("Kabinenaussen wird berechnet");
    //        decimal berechneteHoeheAussen = bodenHoehe + kabinenHoeheInnen + deckenhoehe;
    //        ParamterDictionary["var_KHA"].Value = Convert.ToString(berechneteHoeheAussen);
    //    }

    //    if (bodenHoehe > 0 && kabinenHoeheInnen > 0 && deckenhoehe > 0 && kabinenHoeheAussen > 0)
    //    {
    //        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe != kabinenHoeheAussen)
    //        {
    //            Debug.WriteLine("Kabinenaussen wird angepasst");
    //            decimal berechneteHoeheAussen = bodenHoehe + kabinenHoeheInnen + deckenhoehe;
    //            ParamterDictionary["var_KHA"].Value = Convert.ToString(berechneteHoeheAussen);
    //        }
    //    }
    //}

    // ToDo Parameter aus Datenbank abrufen

    //SetGewichtBodenbelag(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelag"));

    private void SetGewichtBodenbelag(string bodenbelag)
    {
        var bodenbelagGewicht = bodenbelag switch
        {
            "kein" => "0",
            "4/6 Träneblech grundiert" => "33,4",
            "6/8 Tränenblech grundiert" => "49,1",
            "4/6 Träneblech feuerverzinkt" => "33,4",
            "6/8 Tränenblech feuerverzinkt" => "49,1",
            "4/6 Träneblech Edelstahl" => "33,4",
            "6/8 Tränenblech Edelstahl" => "49,1",
            "3,0 SE-TB1 R11 Edelstahl" => "10,25",
            "3,5/5 Alu quintett" => "10,34",
            "PVC-Mipolam" => "3",
            "PVC-Mipolam 1010" => "3",
            "PVC-Mipolam 1060" => "3",
            "Mondo" => "3",
            "Linoleum" => "3",
            "Norament 926/354" => "3",
            "Norament 926 grano" => "3",
            "Grama Blend" => "18",
            "bauseits Stein 10 mm" => "25",
            "bauseits Stein 20 mm" => "50",
            "bauseits Stein 25 mm" => "62,5",
            "bauseits Stein 30 mm" => "75",
            "Alu Quintett 3,5/5" => "10,34",
            "bauseits lt. Beschreibung" => ParamterDictionary!["var_Bodenbelagsgewicht"].Value,
            _ => "0",
        };

        var bodenbelagDicke = bodenbelag switch
        {
            "kein" => "0",
            "4/6 Träneblech grundiert" => "6",
            "6/8 Tränenblech grundiert" => "8",
            "4/6 Träneblech feuerverzinkt" => "6",
            "6/8 Tränenblech feuerverzinkt" => "8",
            "4/6 Träneblech Edelstahl" => "6",
            "6/8 Tränenblech Edelstahl" => "8",
            "3,0 SE-TB1 R11 Edelstahl" => "5",
            "3,5/5 Alu quintett" => "5",
            "PVC-Mipolam" => "3",
            "PVC-Mipolam 1010" => "3",
            "PVC-Mipolam 1060" => "3",
            "Mondo" => "2",
            "Linoleum" => "4",
            "Norament 926/354" => "3",
            "Norament 926 grano" => "3",
            "Grama Blend" => "10",
            "bauseits Stein 10 mm" => "10",
            "bauseits Stein 20 mm" => "20",
            "bauseits Stein 25 mm" => "25",
            "bauseits Stein 30 mm" => "30",
            "Alu Quintett 3,5/5" => "5",
            "bauseits lt. Beschreibung" => ParamterDictionary!["var_Bodenbelagsdicke"].Value,
            _ => "0",
        };

        ParamterDictionary!["var_Bodenbelagsgewicht"].Value = bodenbelagGewicht;
        ParamterDictionary!["var_Bodenbelagsdicke"].Value = bodenbelagDicke;
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties is not null && _CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
