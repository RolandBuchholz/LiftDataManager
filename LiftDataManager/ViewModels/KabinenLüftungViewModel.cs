using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class KabinenLüftungViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ISettingService _settingService;
    private readonly IPdfService _pdfService;

    public CarVentilationResult CarVentilationResult = new();

    public KabinenLüftungViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService,
                                   ICalculationsModule calculationsModuleService, IPdfService pdfService, ISettingService settingsSelectorService) :
         base(parameterDataService, dialogService, navigationService)
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
        if (ParamterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(KabinenLüftungViewModel), ParamterDictionary, FullPathXml, true, _settingService.TonerSaveMode);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();
        CarVentilationResult = _calculationsModuleService.GetCarVentilationCalculation(ParamterDictionary);
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
