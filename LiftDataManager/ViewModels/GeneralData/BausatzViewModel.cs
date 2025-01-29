using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.ViewModels;

public partial class BausatzViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public BausatzViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                            ISettingService settingService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, infoCenterService, settingService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        if (message.PropertyName == "var_Bausatz")
        {
            ParameterDictionary["var_Rahmengewicht"].Value = "";
            ParameterDictionary["var_CFPdefiniert"].AutoUpdateParameterValue("False");
            CarSlingWeight = GetCarSlingWeight(message.NewValue);
            CheckCFPStateAsync(message.NewValue, message.OldValue).SafeFireAndForget();
            UpdateCarFrameDataAsync(message.NewValue, 0).SafeFireAndForget();
            Messenger.Send(new QuicklinkControlMessage(new QuickLinkControlParameters()
            {
                SetDriveData = false,
                UpdateQuicklinks = true
            }));

        };
        if (message.PropertyName == "var_TypFV" ||
            message.PropertyName == "var_FuehrungsschieneFahrkorb" ||
            message.PropertyName == "var_Fuehrungsart")
        {
            SetSafetygearDataAsync().SafeFireAndForget();
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    public int MaxFuse => _calculationsModuleService.GetMaxFuse(ParameterDictionary!["var_ZA_IMP_Regler_Typ"].Value);

    [ObservableProperty]
    public partial bool IsCFPFrame { get; set; }

    [ObservableProperty]
    public partial bool IsCFPDataBaseOverwritten { get; set; }

    [ObservableProperty]
    public partial bool ShowCFPFrameInfo { get; set; }

    [ObservableProperty]
    public partial string CFPFrameInfoToolTip { get; set; } = "Empfehlung: Bausatzkonfiguration im CFP konfigurieren";

    [ObservableProperty]
    public partial string CWTRailName { get; set; } = "Führungsschienen GGW";

    [ObservableProperty]
    public partial string CWTGuideName { get; set; } = "Führungsart GGW";

    [ObservableProperty]
    public partial string CWTRailState { get; set; } = "Status Führungsschienen GGW";

    [ObservableProperty]
    public partial string CWTGuideTyp { get; set; } = "Typ Führung GGW";

    [ObservableProperty]
    public partial string Safetygearworkarea { get; set; } = string.Empty;

    [ObservableProperty]
    public partial double CarSlingWeight { get; set; }

    private async Task SetCarWeightAsync()
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Rahmengewicht"].Value))
        {
            CarSlingWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Rahmengewicht");
        }
        else if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Bausatz"].Value))
        {
            CarSlingWeight = GetCarSlingWeight(ParameterDictionary["var_Bausatz"].Value);
        }
        else
        {
            CarSlingWeight = 0;
        }
        await Task.CompletedTask;
    }

    private double GetCarSlingWeight(string? fangrahmenTyp)
    {
        if (string.IsNullOrEmpty(fangrahmenTyp))
        {
            return 0;
        }
        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null)
        {
            return 0;
        }
        CWTRailName = carFrameType.DriveTypeId == 2 ? "Führungsschienen Joch" : "Führungsschienen GGW";
        CWTGuideName = carFrameType.DriveTypeId == 2 ? "Führungsart Joch" : "Führungsart GGW";
        CWTRailState = carFrameType.DriveTypeId == 2 ? "Status Führungsschienen Joch" : "Status Führungsschienen GGW";
        CWTGuideTyp = carFrameType.DriveTypeId == 2 ? "Typ Führung Joch" : "Typ Führung GGW";
        return carFrameType.CarFrameWeight;
    }

    private async Task CheckCFPStateAsync(string? newCarFrame, string? oldCarFrame)
    {
        if (string.IsNullOrWhiteSpace(newCarFrame))
        {
            return;
        }

        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == newCarFrame);
        if (carFrameType is null)
        {
            return;
        }
        IsCFPFrame = carFrameType.IsCFPControlled;
        ShowCFPFrameInfo = IsCFPFrame & !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_CFPdefiniert");
        CFPFrameInfoToolTip = ShowCFPFrameInfo ? "Empfehlung: Bausatzkonfiguration im CFP konfigurieren" : "Bausatz wurde im CFP konfiguriert";
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
        if (string.IsNullOrWhiteSpace(oldCarFrame))
        {
            return;
        }
        var oldCarFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == oldCarFrame);
        if (oldCarFrameType is null)
        {
            return;
        }
        if (oldCarFrameType.DriveTypeId != carFrameType.DriveTypeId)
        {
            await _dialogService.MessageDialogAsync(
                            "Antriebssystemwechsel",
                            "Achtung nicht benötigte Berechnungen auf den Staus veraltet setzen!\n\n" +
                            "Ab Vault 2025 wird dieser Statuswechsel automatisch ausgeführt.");
        }
    }

    private async Task UpdateCarFrameDataAsync(string? carFrame, int delay)
    {
        await Task.Delay(delay);
        if (string.IsNullOrWhiteSpace(carFrame))
        {
            return;
        }

        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == carFrame);
        if (carFrameType is not null)
        {
            LiftParameterHelper.SetDefaultCarFrameData(ParameterDictionary, carFrameType);
        }
    }

    private async Task SetSafetygearDataAsync()
    {
        var safteyGearResult = _calculationsModuleService.GetSafetyGearCalculation(ParameterDictionary);
        Safetygearworkarea = $"{safteyGearResult.MinLoad} - {safteyGearResult.MaxLoad} kg | {safteyGearResult.CarRailSurface} / {safteyGearResult.Lubrication} | Schienenkopf : {safteyGearResult.AllowedRailHeads}";
        await Task.CompletedTask;
    }

    [RelayCommand]
    private static void GoToBausatzDetail()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(BausatzDetailPage));
    }
    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();

        if (CurrentSpeziProperties is not null)
        {
            SetSafetygearDataAsync().SafeFireAndForget();
            SetCarWeightAsync().SafeFireAndForget();
            CheckCFPStateAsync(ParameterDictionary["var_Bausatz"].Value, null).SafeFireAndForget();
            UpdateCarFrameDataAsync(ParameterDictionary["var_Bausatz"].Value, 1000).SafeFireAndForget();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}