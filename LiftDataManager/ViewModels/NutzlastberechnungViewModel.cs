using LiftDataManager.core.Helpers;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class NutzlastberechnungViewModel : DataViewModelBase, INavigationAware
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;
    private readonly IPdfService _pdfService;

    public Dictionary<int, TableRow<int, double>> Tabelle6 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle7 { get; }
    public Dictionary<int, TableRow<int, double>> Tabelle8 { get; }

    public PayLoadResult PayLoadResult = new();

    public NutzlastberechnungViewModel(IParameterDataService parameterDataService, IDialogService dialogService,
                                       INavigationService navigationService, ICalculationsModule calculationsModuleService ,ParameterContext parametercontext, IPdfService pdfService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
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

    public double NennlastTabelle6 => _calculationsModuleService.GetLoadFromTable(PayLoadResult.NutzflaecheGesamt,nameof(Tabelle6));
    public double NennlastTabelle7 => _calculationsModuleService.GetLoadFromTable(PayLoadResult.NutzflaecheGesamt,nameof(Tabelle7));
    public double Nennlast => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");

    public string ErgebnisNennlast => PayLoadResult.PayloadAllowed ? "Nennlast enspricht der EN81:20!" : "Nennlast enspricht nicht der EN81:20!";

    private void MarkTableSelectedRow()
    {
        ClearSelectedRows(Tabelle6);
        ClearSelectedRows(Tabelle7);
        ClearSelectedRows(Tabelle8);

        if (PayLoadResult.PersonenFlaeche > 0 && PayLoadResult.PersonenFlaeche <= 20)
        {
            var personRow = Tabelle8.FirstOrDefault(x => x.Key == PayLoadResult.PersonenFlaeche);
            if (personRow.Value is not null)
                personRow.Value.IsSelected = true;
        }

        if (NennlastTabelle6 > 0 && NennlastTabelle6 <= 2500)
        {
            var loadTable6Row = Tabelle6.FirstOrDefault(x => x.Key == NennlastTabelle6);
            if (loadTable6Row.Value is not null)
            {
                loadTable6Row.Value.IsSelected = true;
            }
            else
            {
                var lowTable6Entry = Tabelle6.Where(x => x.Key < NennlastTabelle6).Last();
                var highTable6Entry = Tabelle6.Where(x => x.Key > NennlastTabelle6).First();
                if (lowTable6Entry.Value is not null)
                    lowTable6Entry.Value.IsSelected = true;
                if (lowTable6Entry.Value is not null)
                    highTable6Entry.Value.IsSelected = true;
            }
        }

        if (NennlastTabelle7 > 0 && NennlastTabelle7 <= 1600)
        {
            var loadTable7Row = Tabelle7.FirstOrDefault(x => x.Key == NennlastTabelle7);
            if (loadTable7Row.Value is not null)
            {
                loadTable7Row.Value.IsSelected = true;
            }
            else
            {
                var lowTable7Entry = Tabelle7.Where(x => x.Key < NennlastTabelle7).Last();
                var highTable7Entry = Tabelle7.Where(x => x.Key > NennlastTabelle7).First();
                if (lowTable7Entry.Value is not null)
                    lowTable7Entry.Value.IsSelected = true;
                if (lowTable7Entry.Value is not null)
                    highTable7Entry.Value.IsSelected = true;
            }
        }
    }

    private static void ClearSelectedRows(Dictionary<int, TableRow<int, double>> table)
    {
        foreach (var row in table)
        {
           if (row.Value.IsSelected)
                row.Value.IsSelected = false;
        }
    }

    [RelayCommand]
    public void CreatePdf()
    {
        if (ParamterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(NutzlastberechnungViewModel), ParamterDictionary, FullPathXml, true);
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
            _calculationsModuleService.SetPayLoadResult(ParamterDictionary!, PayLoadResult.PersonenBerechnet,PayLoadResult.NutzflaecheGesamt);
            MarkTableSelectedRow();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}