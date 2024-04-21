using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class KabinengewichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ISettingService _settingService;
    private readonly IPdfService _pdfService;

    public CarWeightResult CarWeightResult = new();

    public KabinengewichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
        ParameterContext parametercontext, ICalculationsModule calculationsModuleService, ISettingService settingsSelectorService, IPdfService pdfService) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _settingService = settingsSelectorService;
        _pdfService = pdfService;

        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.ParameterDictionary is not null)
            ParameterDictionary = CurrentSpeziProperties.ParameterDictionary;
    }

    [RelayCommand]
    public void CreatePdf()
    {
        if (ParameterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(KabinengewichtViewModel), ParameterDictionary, FullPathXml, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
        }
    }

    [ObservableProperty]
    private bool overridenCarWeight;

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            OverridenCarWeight = !string.IsNullOrWhiteSpace(ParameterDictionary!["var_KabinengewichtCAD"].Value);
            CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParameterDictionary);
            ParameterDictionary!["var_F"].AutoUpdateParameterValue(Convert.ToString(CarWeightResult.FahrkorbGewicht));
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}