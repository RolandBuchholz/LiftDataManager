using LiftDataManager.Core.Models.CalculationResultsModels;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels;

public partial class NutzlastberechnungViewModel : DataViewModelBase, INavigationAwareEx
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;
    private readonly IPdfService _pdfService;

    public Dictionary<int, TableRow<int, double>> Tabelle6 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle7 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle8 { get; }

    public PayLoadResult PayLoadResult = new();

    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ISettingService settingService, 
                                       ILogger<DataViewModelBase> baseLogger,ICalculationsModule calculationsModuleService, ParameterContext parametercontext, IPdfService pdfService) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;
        ParameterDictionary = _parameterDataService.GetParameterDictionary();
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

    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");

    public string ErgebnisNennlast => PayLoadResult.PayloadAllowed ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";

    [RelayCommand]
    public void CreateNutzlastberechnungPdf()
    {
        if (ParameterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(NutzlastberechnungViewModel), ParameterDictionary, FullPathXml, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            PayLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParameterDictionary);
            _calculationsModuleService.SetPayLoadResult(ParameterDictionary, PayLoadResult.PersonenBerechnet, PayLoadResult.NutzflaecheGesamt);
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}