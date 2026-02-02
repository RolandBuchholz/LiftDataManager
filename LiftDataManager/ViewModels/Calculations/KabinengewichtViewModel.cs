using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class KabinengewichtViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly IPdfService _pdfService;

    public KabinengewichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                   ISettingService settingService, ILogger<DataViewModelBase> baseLogger, ICalculationsModule calculationsModuleService,
                                   IPdfService pdfService) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;
        ParameterDictionary = _parameterDataService.GetParameterDictionary();
        CarWeightResult ??= new();
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
        {
            return;
        }
        if (!(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_ImprovedCarWeightCalc")
        {
            if (!string.Equals(message.NewValue, message.OldValue))
            {
                if (message.NewValue == "False")
                {
                    ParameterDictionary["var_ForceOldCarWeightCalc"].AutoUpdateParameterValue("True");
                }
                CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParameterDictionary);
            }
        }
 
        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }


    [RelayCommand]
    public void CreatePdf()
    {
        if (ParameterDictionary is not null)
            _pdfService.MakeSinglePdfDocument(nameof(KabinengewichtViewModel), ParameterDictionary, FullPathXml, true, _settingService.TonerSaveMode, _settingService.LowHighlightMode);
    }

    [ObservableProperty]
    public partial CarWeightResult CarWeightResult { get; set; }

    [ObservableProperty]
    public partial bool OverridenCarWeight { get; set; }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            OverridenCarWeight = !string.IsNullOrWhiteSpace(ParameterDictionary["var_KabinengewichtCAD"].Value);
            CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParameterDictionary);
            ParameterDictionary["var_F"].AutoUpdateParameterValue(Convert.ToString(CarWeightResult.FahrkorbGewicht));
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}