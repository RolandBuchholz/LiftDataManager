using LiftDataManager.core.Helpers;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ISettingService _settingService;
    private readonly ParameterContext _parametercontext;
    private readonly IPdfService _pdfService;

    public Dictionary<int, TableRow<int, double>> Tabelle6 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle7 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle8 { get; }

    public PayLoadResult PayLoadResult = new();

    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                                       ICalculationsModule calculationsModuleService, ISettingService settingsSelectorService, ParameterContext parametercontext, IPdfService pdfService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _settingService = settingsSelectorService;
        _pdfService = pdfService;

        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.ParamterDictionary is not null)
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;

        Tabelle6 = _calculationsModuleService.Table6;
        Tabelle7 = _calculationsModuleService.Table7;
        Tabelle8 = _calculationsModuleService.Table8;
    }

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);
    public SolidColorBrush ErgebnisNennlastColor => PayLoadResult.PayloadAllowed ? successColor : failureColor;

    public string InfoZugangA => PayLoadResult.NutzflaecheZugangA == 0 && PayLoadResult.ZugangA ? " Tiefe < 100" : string.Empty;
    public string InfoZugangB => PayLoadResult.NutzflaecheZugangB == 0 && PayLoadResult.ZugangB ? " Tiefe < 100" : string.Empty;
    public string InfoZugangC => PayLoadResult.NutzflaecheZugangC == 0 && PayLoadResult.ZugangC ? " Tiefe < 100" : string.Empty;
    public string InfoZugangD => PayLoadResult.NutzflaecheZugangD == 0 && PayLoadResult.ZugangD ? " Tiefe < 100" : string.Empty;

    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");

    public string ErgebnisNennlast => PayLoadResult.PayloadAllowed ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";

    [RelayCommand]
    public void CreateNutzlastberechnungPdf()
    {
        if (ParamterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(NutzlastberechnungViewModel), ParamterDictionary, FullPathXml, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
            PayLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParamterDictionary);
            _calculationsModuleService.SetPayLoadResult(ParamterDictionary!, PayLoadResult.PersonenBerechnet, PayLoadResult.NutzflaecheGesamt);
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}