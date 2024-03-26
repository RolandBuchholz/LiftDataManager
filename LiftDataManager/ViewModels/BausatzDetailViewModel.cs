using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public BausatzDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
    }

    public string CarFrameTyp => string.IsNullOrWhiteSpace(ParameterDictionary["var_Bausatz"].Value) ? "Kein Bausatz gewählt" : ParameterDictionary["var_Bausatz"].Value!;

    [ObservableProperty]
    private bool isCFPFrame;

    [ObservableProperty]
    private bool isCFPDataBaseOverwritten;

    [ObservableProperty]
    private bool showCFPFrameInfo;

    [RelayCommand]
    private void GoToBausatzViewModel() => _navigationService.NavigateTo("LiftDataManager.ViewModels.BausatzViewModel");

    private void CheckCFPState()
    {
        var fangrahmenTyp = ParameterDictionary!["var_Bausatz"].Value;
        if (string.IsNullOrWhiteSpace(fangrahmenTyp))
            return;
        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null)
            return;
        IsCFPFrame = carFrameType.IsCFPControlled;
        ShowCFPFrameInfo = IsCFPFrame & !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_CFPdefiniert");
        if (IsCFPFrame)
        {
            if (string.IsNullOrWhiteSpace(FullPathXml))
                return;

            var basePath = Path.GetDirectoryName(FullPathXml);
            if (string.IsNullOrWhiteSpace(basePath))
                return;

            var calculationsPath = Path.Combine(basePath, "Berechnungen");

            if (Directory.Exists(calculationsPath))
            {
                var calculations = Directory.EnumerateFiles(calculationsPath);
                IsCFPDataBaseOverwritten = calculations.Any(x => x.Contains("DB-Anpassungen"));
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CheckCFPState();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}