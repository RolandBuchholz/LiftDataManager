namespace LiftDataManager.ViewModels;

public class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware
{
    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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

    public int AnzahlKabinentueren => Convert.ToInt32(ZugangA) + Convert.ToInt32(ZugangB) + Convert.ToInt32(ZugangC) + Convert.ToInt32(ZugangD);
    public int AnzahlKabinentuerfluegel => LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_AnzahlTuerfluegel");



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
