using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System.Text.Json;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly ILogger<BausatzDetailViewModel> _logger;
    public int[] EulerscheBucklingLoadCases { get; } = [1, 2, 3, 4];

    public BausatzDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService, ILogger<BausatzDetailViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _logger = logger;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_Anzahl_Puffer_FK" ||
            message.PropertyName == "var_Puffer_Profil_FK" ||
            message.PropertyName == "var_UK_Puffer_FK")
        {
            BufferCalculationDataCarFrame = GetBufferCalculationData("var_PufferCalculationData_FK", SelectedEulerCaseCarFrame, false);
        };

        if (message.PropertyName == "var_Anzahl_Puffer_GGW" ||
            message.PropertyName == "var_Puffer_Profil_GGW" ||
            message.PropertyName == "var_UK_Puffer_GGW")
        {
            BufferCalculationDataCounterWeight = GetBufferCalculationData("var_PufferCalculationData_GGW", SelectedEulerCaseCounterWeight, false);
        };

        if (message.PropertyName == "var_Anzahl_Puffer_EM_SK" ||
            message.PropertyName == "var_Puffer_Profil_EM_SK" ||
            message.PropertyName == "var_Pufferstuezenlaenge_EM_SK")
        {
            BufferCalculationDataReducedSafetyRoomHead = GetBufferCalculationData("var_PufferCalculationData_EM_SK", SelectedEulerCaseReducedSafetyRoomHead, BufferUnderCounterweight);
        };

        if (message.PropertyName == "var_Anzahl_Puffer_EM_SG" ||
            message.PropertyName == "var_Puffer_Profil_EM_SG" ||
            message.PropertyName == "var_Pufferstuezenlaenge_EM_SG")
        {
            BufferCalculationDataReducedSafetyRoomPit = GetBufferCalculationData("var_PufferCalculationData_EM_SG", SelectedEulerCaseReducedSafetyRoomPit, false);
        };

        if (message.PropertyName == "var_Tragseiltyp" ||
            message.PropertyName == "var_NumberOfRopes" ||
            message.PropertyName == "var_Mindestbruchlast" ||
            message.PropertyName == "var_RopeLength")
        {
            UpdateRopeCalculationData();
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    public CarFrameType? CarFrameTyp { get; set; }

    [ObservableProperty]
    private string carFrameTypName = "Kein Bausatz gewählt";

    [ObservableProperty]
    private double ropeDiameter = 0d;
    partial void OnRopeDiameterChanged(double value)
    {
        if (RopeCalculationData != null)
        {
            RopeCalculationData.RopeDiameter = value;
            UpdateRopeCalculationData();
        }
    }

    [ObservableProperty]
    private int wireStrength = 0;
    partial void OnWireStrengthChanged(int value)
    {
        if (RopeCalculationData != null)
        {
            RopeCalculationData.WireStrength = value;
            UpdateRopeCalculationData();
        }
    }

    [ObservableProperty]
    private double ropeWeight = 0;
    partial void OnRopeWeightChanged(double value)
    {
        if (RopeCalculationData != null)
        {
            RopeCalculationData.RopeWeight = value;
            UpdateRopeCalculationData();
        }
    }

    [ObservableProperty]
    private int maximumNumberOfRopes = 0;
    partial void OnMaximumNumberOfRopesChanged(int value)
    {
        if (RopeCalculationData != null)
        {
            RopeCalculationData.MaximumNumberOfRopes = value;
            UpdateRopeCalculationData();
        }
    }

    [ObservableProperty]
    private bool showCounterWeightBuffer;

    [ObservableProperty]
    private bool showReducedSafetyRoomHeadBuffer;

    [ObservableProperty]
    private bool showReducedSafetyRoomPitBuffer;

    [ObservableProperty]
    private RopeCalculationData? ropeCalculationData;

    [ObservableProperty]
    private BufferCalculationData? bufferCalculationDataCarFrame;

    [ObservableProperty]
    private BufferCalculationData? bufferCalculationDataCounterWeight;

    [ObservableProperty]
    private BufferCalculationData? bufferCalculationDataReducedSafetyRoomHead;

    [ObservableProperty]
    private BufferCalculationData? bufferCalculationDataReducedSafetyRoomPit;

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
    private bool hasRopes;

    [ObservableProperty]
    private int selectedEulerCaseCarFrame = 1;
    partial void OnSelectedEulerCaseCarFrameChanged(int value)
    {
        BufferCalculationDataCarFrame = GetBufferCalculationData("var_PufferCalculationData_FK", value, false);
    }

    [ObservableProperty]
    private int selectedEulerCaseCounterWeight = 1;
    partial void OnSelectedEulerCaseCounterWeightChanged(int value)
    {
        BufferCalculationDataCounterWeight = GetBufferCalculationData("var_PufferCalculationData_GGW", value, false);
    }

    [ObservableProperty]
    private int selectedEulerCaseReducedSafetyRoomHead = 1;
    partial void OnSelectedEulerCaseReducedSafetyRoomHeadChanged(int value)
    {
        BufferCalculationDataReducedSafetyRoomHead = GetBufferCalculationData("var_PufferCalculationData_EM_SK", value, BufferUnderCounterweight);
    }

    [ObservableProperty]
    private int selectedEulerCaseReducedSafetyRoomPit = 1;
    partial void OnSelectedEulerCaseReducedSafetyRoomPitChanged(int value)
    {
        BufferCalculationDataReducedSafetyRoomPit = GetBufferCalculationData("var_PufferCalculationData_EM_SG", value, false);
    }

    [ObservableProperty]
    private bool bufferUnderCounterweight;
    partial void OnBufferUnderCounterweightChanged(bool value)
    {
        BufferCalculationDataReducedSafetyRoomHead = GetBufferCalculationData("var_PufferCalculationData_EM_SK", SelectedEulerCaseReducedSafetyRoomHead, value);
    }

    [ObservableProperty]
    private string bufferDataCarFrame = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataCounterWeight = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataReducedSafetyRoomHead = "Keine Pufferdaten vorhanden";

    [ObservableProperty]
    private string bufferDataReducedSafetyRoomPit = "Keine Pufferdaten vorhanden";

    [RelayCommand]
    private static void GoToBausatzViewModel()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(BausatzPage));
    }

    [ObservableProperty]
    private PivotItem? selectedPivotItem;
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {

                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    private void CheckCFPState()
    {
        var carframe = ParameterDictionary!["var_Bausatz"].Value;
        if (!string.IsNullOrWhiteSpace(carframe))
        {
            CarFrameTyp = _parametercontext.Set<CarFrameType>().Include(i => i.CarFrameBaseType)
                                                        .Include(i => i.DriveType)
                                                        .FirstOrDefault(x => x.Name == carframe);
        }

        if (CarFrameTyp is null)
        {
            return;
        }

        CarFrameTypName = CarFrameTyp.Name;
        IsRopedrive = CarFrameTyp.DriveTypeId == 1;
        IsCantilever = CarFrameTyp.CarFrameBaseTypeId == 1 || CarFrameTyp.CarFrameBaseTypeId == 4;
        HasRopes = CarFrameTyp.DriveTypeId == 1 || CarFrameTyp.CarFrameBaseTypeId == 4 || CarFrameTyp.CarFrameBaseTypeId == 6;
        CarFrameDescription = $"{CarFrameTyp.DriveType?.Name} - {CarFrameTyp.CarFrameBaseType?.Name}";
        IsCFPFrame = CarFrameTyp.IsCFPControlled;
        CWTDGBName = IsRopedrive ? "Stichmaß GGW" : "Stichmaß Joch";
        ShowCFPFrameInfo = IsCFPFrame & !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_CFPdefiniert");
        if (IsCFPFrame)
        {
            if (string.IsNullOrWhiteSpace(FullPathXml))
            {
                return;
            }
            var basePath = Path.GetDirectoryName(FullPathXml);
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return;
            }
            var calculationsPath = Path.Combine(basePath, "Berechnungen");
            if (Directory.Exists(calculationsPath))
            {
                var calculations = Directory.EnumerateFiles(calculationsPath);
                IsCFPDataBaseOverwritten = calculations.Any(x => x.Contains("DB-Anpassungen"));
            }
        }
    }

    private BufferCalculationData GetBufferCalculationData(string parameterName, int eulerCase, bool bufferUnderCounterweight)
    {
        var bufferCalculation = _calculationsModuleService.GetBufferCalculationData(ParameterDictionary, parameterName, eulerCase, bufferUnderCounterweight);
        ParameterDictionary[parameterName].AutoUpdateParameterValue(JsonSerializer.Serialize(bufferCalculation, _options).Replace("\r\n", "\n"));
        return bufferCalculation;
    }

    private void RestorePufferCalculationData()
    {
        string[] pufferCalculationDataValues = ["var_PufferCalculationData_FK", "var_PufferCalculationData_GGW", "var_PufferCalculationData_EM_SK", "var_PufferCalculationData_EM_SG"];
        foreach (var item in pufferCalculationDataValues)
        {
            if (!string.IsNullOrWhiteSpace(ParameterDictionary[item].Value))
            {
                try
                {
                    var restoredBufferCalculation = JsonSerializer.Deserialize<BufferCalculationData>(ParameterDictionary[item].Value!);
                    if (restoredBufferCalculation is not null)
                    {
                        switch (item)
                        {
                            case "var_PufferCalculationData_FK":
                                SelectedEulerCaseCarFrame = restoredBufferCalculation.EulerCase;
                                break;
                            case "var_PufferCalculationData_GGW":
                                SelectedEulerCaseCounterWeight = restoredBufferCalculation.EulerCase;
                                break;
                            case "var_PufferCalculationData_EM_SK":
                                SelectedEulerCaseReducedSafetyRoomHead = restoredBufferCalculation.EulerCase;
                                BufferUnderCounterweight = restoredBufferCalculation.ReducedSafetyRoomBufferUnderCounterweight;
                                break;
                            case "var_PufferCalculationData_EM_SG":
                                SelectedEulerCaseReducedSafetyRoomPit = restoredBufferCalculation.EulerCase;
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning(60202, "Restore buffercalculationdata failed");
                }
            }
        }
    }

    private void RestoreRopeCalculationData()
    {
        if (HasRopes)
        {
            if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_RopeCalculationData"].Value))
            {
                try
                {
                    var tempRopeCalculationData = JsonSerializer.Deserialize<RopeCalculationData>(ParameterDictionary["var_RopeCalculationData"].Value!);
                    if (tempRopeCalculationData != null)
                    {
                        RopeDiameter = tempRopeCalculationData.RopeDiameter;
                        WireStrength = tempRopeCalculationData.WireStrength;
                        MaximumNumberOfRopes = tempRopeCalculationData.MaximumNumberOfRopes;
                        RopeWeight = tempRopeCalculationData.RopeWeight;
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning(60202, "Restore ropecalculationdata failed");
                }
            }

            string ropeDescription = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tragseiltyp");
            int numberOfRopes = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_NumberOfRopes");
            int minimumBreakingStrength = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Mindestbruchlast");
            double ropeLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_RopeLength");
            RopeCalculationData = new RopeCalculationData()
            {
                RopeDescription = ropeDescription,
                RopeDiameter = RopeDiameter,
                WireStrength = WireStrength,
                MaximumNumberOfRopes = MaximumNumberOfRopes,
                RopeWeight = RopeWeight,
                NumberOfRopes = numberOfRopes,
                MinimumBreakingStrength = minimumBreakingStrength,
                RopeLength = ropeLength
            };
            UpdateRopeCalculationData();
        }
    }

    private void GetPufferDetailData()
    {
        Tuple<string, string>[] puffers = [new("var_Puffertyp", "BufferDataCarFrame"),
                                           new("var_Puffertyp_GGW", "BufferDataCounterWeight"),
                                           new("var_Puffertyp_EM_SK", "BufferDataReducedSafetyRoomHead"),
                                           new("var_Puffertyp_EM_SG", "BufferDataReducedSafetyRoomPit")];

        double liftSpeed = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_v");

        if (liftSpeed == 0.0)
            return;

        foreach (var puffer in puffers)
        {
            if (!string.IsNullOrWhiteSpace(ParameterDictionary[puffer.Item1].Value))
            {
                var bufferProperty = GetType().GetProperty(puffer.Item2);
                bufferProperty?.SetValue(this, _calculationsModuleService.GetBufferDetails(ParameterDictionary[puffer.Item1].Value!, liftSpeed), null);
            }
        }
    }

    private void SetDetailDataVisibility()
    {
        ShowCounterWeightBuffer = IsRopedrive;
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Ersatzmassnahmen"].Value))
        {
            switch (ParameterDictionary["var_Ersatzmassnahmen"].Value)
            {
                case "Schachtkopf":
                    ShowReducedSafetyRoomHeadBuffer = true;
                    break;
                case "Schachtgrube":
                    ShowReducedSafetyRoomPitBuffer = true;
                    break;
                case "Schachtkopf und Schachtgrube":
                    ShowReducedSafetyRoomHeadBuffer = true;
                    ShowReducedSafetyRoomPitBuffer = true;
                    break;
                default:
                    break;
            }
        }
        if (ShowCounterWeightBuffer)
        {
            BufferCalculationDataCounterWeight = GetBufferCalculationData("var_PufferCalculationData_GGW", SelectedEulerCaseCounterWeight, false);
        }
        else
        {
            ParameterDictionary["var_PufferCalculationData_GGW"].AutoUpdateParameterValue(string.Empty);
        }
        if (ShowReducedSafetyRoomHeadBuffer)
        {
            BufferCalculationDataReducedSafetyRoomHead = GetBufferCalculationData("var_PufferCalculationData_EM_SK", SelectedEulerCaseReducedSafetyRoomHead, BufferUnderCounterweight);
        }
        else
        {
            ParameterDictionary["var_PufferCalculationData_EM_SK"].AutoUpdateParameterValue(string.Empty);
        }
        if (ShowReducedSafetyRoomPitBuffer)
        {
            BufferCalculationDataReducedSafetyRoomPit = GetBufferCalculationData("var_PufferCalculationData_EM_SG", SelectedEulerCaseReducedSafetyRoomPit, false);
        }
        else
        {
            ParameterDictionary["var_PufferCalculationData_EM_SG"].AutoUpdateParameterValue(string.Empty);
        }


    }

    private void UpdateRopeCalculationData()
    {
        if (RopeCalculationData != null)
        {
            ParameterDictionary["var_RopeCalculationData"].AutoUpdateParameterValue(JsonSerializer.Serialize(RopeCalculationData, _options).Replace("\r\n", "\n"));
        }
    }

    private async Task UpdateCarFrameDataAsync(int delay)
    {
        await Task.Delay(delay);
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Pufferstuetzenmaterial"].Value))
        {
            ParameterDictionary["var_Pufferstuetzenmaterial"].AutoUpdateParameterValue("1.0038-ST37-2-S235");
        }
        CheckCFPState();
        LiftParameterHelper.SetDefaultCarFrameData(ParameterDictionary, CarFrameTyp);
        RestorePufferCalculationData();
        RestoreRopeCalculationData();
        BufferCalculationDataCarFrame = GetBufferCalculationData("var_PufferCalculationData_FK", SelectedEulerCaseCarFrame, false);
        SetDetailDataVisibility();
        GetPufferDetailData();
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        UpdateCarFrameDataAsync(500).SafeFireAndForget();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}