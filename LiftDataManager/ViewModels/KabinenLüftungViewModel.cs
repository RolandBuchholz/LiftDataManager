using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class KabinenLüftungViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ISettingService _settingService;
    private readonly IPdfService _pdfService;

    public CarVentilationResult CarVentilationResult = new();

    public KabinenLüftungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                   ICalculationsModule calculationsModuleService, IPdfService pdfService, ISettingService settingsSelectorService) :
                                   base(parameterDataService, dialogService, infoCenterService)
    {
        _calculationsModuleService = calculationsModuleService;
        _settingService = settingsSelectorService;
        _pdfService = pdfService;

    }

    private readonly SolidColorBrush successColor = new(Colors.LightGreen);
    private readonly SolidColorBrush failureColor = new(Colors.IndianRed);

    public string ErgebnisBelueftungDecke => CarVentilationResult.ErgebnisBelueftungDecke ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";
    public SolidColorBrush ErgebnisBelueftungDeckeColor => CarVentilationResult.ErgebnisBelueftungDecke ? successColor : failureColor;

    public string ErgebnisEntlueftung => CarVentilationResult.ErgebnisEntlueftung ? "Vorschrift erfüllt !" : "Vorschrift nicht erfüllt !";
    public SolidColorBrush ErgebnisEntlueftungColor => CarVentilationResult.ErgebnisEntlueftung ? successColor : failureColor;

    [RelayCommand]
    public void CreatePdf()
    {
        if (ParameterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(KabinenLüftungViewModel), ParameterDictionary, FullPathXml, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CarVentilationResult = _calculationsModuleService.GetCarVentilationCalculation(ParameterDictionary);
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
