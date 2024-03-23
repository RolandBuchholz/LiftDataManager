using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.ViewModels;

public partial class BausatzViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public BausatzViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_Bausatz")
        {
            ParameterDictionary!["var_Rahmengewicht"].Value = "";
            FangrahmenGewicht = GetFangrahmengewicht(message.NewValue);
            CheckCFPState();
        };
        if (message.PropertyName == "var_TypFV" ||
            message.PropertyName == "var_FuehrungsschieneFahrkorb" ||
            message.PropertyName == "var_Fuehrungsart")
        {
            SetSafetygearData();
        };
        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    public int MaxFuse => _calculationsModuleService.GetMaxFuse(ParameterDictionary!["var_ZA_IMP_Regler_Typ"].Value);

    [ObservableProperty]
    private bool isCFPFrame;

    [ObservableProperty]
    private bool isCFPDataBaseOverwritten;

    [ObservableProperty]
    private bool showCFPFrameInfo;

    [ObservableProperty]
    private string cWTRailName = "Führungsschienen GGW";

    [ObservableProperty]
    private string cWTGuideName = "Führungsart GGW";

    [ObservableProperty]
    private string cWTRailState = "Status Führungsschienen GGW";

    [ObservableProperty]
    private string cWTGuideTyp = "Typ Führung GGW";

    [ObservableProperty]
    private string safetygearworkarea = string.Empty;

    private double _FangrahmenGewicht;
    public double FangrahmenGewicht
    {
        get => _FangrahmenGewicht;
        set => SetProperty(ref _FangrahmenGewicht, value);
    }

    private void SetCarWeight()
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary!["var_Rahmengewicht"].Value))
        {
            FangrahmenGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Rahmengewicht");
        }
        else if (!string.IsNullOrWhiteSpace(ParameterDictionary!["var_Bausatz"].Value))
        {
            FangrahmenGewicht = GetFangrahmengewicht(ParameterDictionary!["var_Bausatz"].Value);
        }
        else
        {
            FangrahmenGewicht = 0;
        }
    }

    private double GetFangrahmengewicht(string? fangrahmenTyp)
    {
        if (string.IsNullOrEmpty(fangrahmenTyp))
            return 0;
        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null)
            return 0;
        CWTRailName = carFrameType.DriveTypeId == 2 ? "Führungsschienen Joch" : "Führungsschienen GGW";
        CWTGuideName = carFrameType.DriveTypeId == 2 ? "Führungsart Joch" : "Führungsart GGW";
        CWTRailState = carFrameType.DriveTypeId == 2 ? "Status Führungsschienen Joch" : "Status Führungsschienen GGW";
        CWTGuideTyp = carFrameType.DriveTypeId == 2 ? "Typ Führung Joch" : "Typ Führung GGW";
        return carFrameType.CarFrameWeight;
    }

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

    private void SetSafetygearData()
    {
        var safteyGearResult = _calculationsModuleService.GetSafetyGearCalculation(ParameterDictionary);
        Safetygearworkarea = $"{safteyGearResult.MinLoad} - {safteyGearResult.MaxLoad} kg | {safteyGearResult.CarRailSurface} / {safteyGearResult.Lubrication} | Schienenkopf : {safteyGearResult.AllowedRailHeads}";
    }

    [RelayCommand]
    private void GoToBausatzDetail() => _navigationService.NavigateTo("LiftDataManager.ViewModels.BausatzDetailViewModel");
    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        SetSafetygearData();
        SetCarWeight();
        CheckCFPState();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}