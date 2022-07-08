namespace LiftDataManager.ViewModels;

public class KabineViewModel : DataViewModelBase, INavigationAware
{
    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
