using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Models.CalculationResultsModels;

namespace LiftDataManager.ViewModels;

public partial class KabinengewichtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly IPdfService _pdfService;

    public CarWeightResult CarWeightResult = new();

    public KabinengewichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext,
                                   ICalculationsModule calculationsModuleService, IPdfService pdfService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;

        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (CurrentSpeziProperties.ParamterDictionary is not null)
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;
    }

    [RelayCommand]
    public void CreatePdf()
    {
        if (ParamterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(KabinengewichtViewModel), ParamterDictionary, FullPathXml, true);
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
            CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(ParamterDictionary);
            ParamterDictionary!["var_F"].Value = Convert.ToString(CarWeightResult.FahrkorbGewicht);
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}