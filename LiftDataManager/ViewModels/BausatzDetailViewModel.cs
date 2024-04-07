using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using System.Text.Json;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    public int[] EulerscheBucklingLoadCases { get; } = [1, 2, 3,4];
    public BufferCalculationData? BufferCalculationDataCarFrame { get; set; }
    public BufferCalculationData? BufferCalculationDataCounterWeight { get; set; }
    public BufferCalculationData? BufferCalculationDataReducedSafetyRoomHead { get; set; }
    public BufferCalculationData? BufferCalculationDataReducedSafetyRoomPit { get; set; }

    public BausatzDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
    }

    public string CarFrameTyp => string.IsNullOrWhiteSpace(ParameterDictionary["var_Bausatz"].Value) ? "Kein Bausatz gewählt" : ParameterDictionary["var_Bausatz"].Value!;

    [ObservableProperty]
    private string carFrameDescription = "Keine Bausatzinformationen";

    [ObservableProperty]
    private string cWTDGBName = "Stichmaß GGW";

    [ObservableProperty]
    private bool isCFPFrame;

    [ObservableProperty]
    private bool isCFPDataBaseOverwritten;

    [ObservableProperty]
    private bool showCFPFrameInfo;

    [ObservableProperty]
    private bool isRopedrive;

    [ObservableProperty]
    private bool isCantilever;

    [ObservableProperty]
    private int selectedEulerCaseCarFrame = 1;

    [ObservableProperty]
    private int selectedEulerCaseCounterWeight = 1;

    [ObservableProperty]
    private int selectedEulerCaseReducedSafetyRoomHead = 1;

    [ObservableProperty]
    private int selectedEulerCaseReducedSafetyRoomPit = 1;

    [ObservableProperty]
    private bool bufferUnderCounterweight;

    [ObservableProperty]
    private string bufferDataCarFrame = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataCounterWeight = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataReducedSafetyRoomHead = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataReducedSafetyRoomPit = "Keine Pufferdaten vorhanden";

    [RelayCommand]
    private void GoToBausatzViewModel() => _navigationService.NavigateTo("LiftDataManager.ViewModels.BausatzViewModel");

    private void CheckCFPState()
    {
        var fangrahmenTyp = ParameterDictionary!["var_Bausatz"].Value;
        if (string.IsNullOrWhiteSpace(fangrahmenTyp))
            return;
        var carFrameType = _parametercontext.Set<CarFrameType>().Include(i => i.CarFrameBaseType)
                                                                .Include(i => i.DriveType)
                                                                .FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null)
            return;
        IsRopedrive = carFrameType.DriveTypeId == 1;
        IsCantilever = carFrameType.CarFrameBaseTypeId == 1;   
        CarFrameDescription = $"{carFrameType.DriveType?.Name} - {carFrameType.CarFrameBaseType?.Name}";
        IsCFPFrame = carFrameType.IsCFPControlled;
        CWTDGBName = IsRopedrive ? "Stichmaß GGW" : "Stichmaß Joch";
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

    private BufferCalculationData GetBufferCalculationData(string parameterName, bool bufferUnderCounterweight)
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[parameterName].Value))
        {
            var bufferCalculation = JsonSerializer.Deserialize<BufferCalculationData>(ParameterDictionary[parameterName].Value!);
            if (bufferCalculation is not null)
            {
                return bufferCalculation;
            }
        }
        var newBufferCalculation = _calculationsModuleService.GetBufferCalculationData(ParameterDictionary, parameterName, selectedEulerCaseCarFrame, bufferUnderCounterweight);
        SetBufferCalculationData(parameterName, newBufferCalculation);
        return newBufferCalculation;
    }

    private void SetBufferCalculationData(string prameterName, BufferCalculationData bufferCalculation)
    {
        ParameterDictionary[prameterName].AutoUpdateParameterValue(JsonSerializer.Serialize(bufferCalculation, _options).Replace("\r\n", "\n"));
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CheckCFPState();
        BufferCalculationDataCarFrame = GetBufferCalculationData("var_PufferCalculationData_FK", false);
        BufferCalculationDataCounterWeight = GetBufferCalculationData("var_PufferCalculationData_GGW", false);
        BufferCalculationDataReducedSafetyRoomHead = GetBufferCalculationData("var_PufferCalculationData_EM_SK", bufferUnderCounterweight);
        BufferCalculationDataReducedSafetyRoomPit = GetBufferCalculationData("var_PufferCalculationData_EM_SG",false);
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}