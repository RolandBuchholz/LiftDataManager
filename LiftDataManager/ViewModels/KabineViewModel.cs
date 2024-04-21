using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Controls;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.ViewModels;

public partial class KabineViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;

    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                           ICalculationsModule calculationsModuleService, ParameterContext parametercontext) :
                           base(parameterDataService, dialogService, infoCenterService)
    {
        _calculationsModuleService = calculationsModuleService;
        _parametercontext = parametercontext;
    }

    private readonly string[] carEquipment = [ "var_SpiegelA", "var_SpiegelB", "var_SpiegelC", "var_SpiegelD",
                                               "var_HandlaufA", "var_HandlaufB", "var_HandlaufC", "var_HandlaufD",
                                               "var_SockelleisteA", "var_SockelleisteB", "var_SockelleisteC", "var_SockelleisteD",
                                               "var_RammschutzA", "var_RammschutzB", "var_RammschutzC", "var_RammschutzD",
                                               "var_PaneelPosA", "var_PaneelPosB", "var_PaneelPosC", "var_PaneelPosD",
                                               "var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D"];

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_KBI" ||
            message.PropertyName == "var_KTI" ||
            message.PropertyName == "var_TuerEinbau" ||
            message.PropertyName == "var_TuerEinbauB" ||
            message.PropertyName == "var_TuerEinbauC" ||
            message.PropertyName == "var_TuerEinbauD")
        {
            _ = SetCalculatedValuesAsync();
            SetDistanceBetweenDoors();
        };

        if (message.PropertyName == "var_Bodenbelag" ||
            message.PropertyName == "var_Bodentyp" ||
            message.PropertyName == "var_Bodenbelagsdicke")
        {
            SetCanEditFlooringProperties(message.PropertyName, message.NewValue, message.OldValue);
        }

        if (message.PropertyName == "var_BodenbelagsTyp")
        {
            SetFloorImagePath();
        }

        if (carEquipment.Contains(message.PropertyName))
        {
            var liftparameter = message.Sender as Parameter;
            if (liftparameter is not null && liftparameter.Name is not null)
            {
                string[] zugang = { $"var_ZUGANSSTELLEN_{liftparameter.Name[^1..]}" };
                _ = liftparameter.AfterValidateRangeParameterAsync(zugang);
            }
        }

        if (message.PropertyName == "var_Rueckwand")
        {
            var liftparameter = message.Sender as Parameter;
            string[] zugang = { "var_ZUGANSSTELLEN_C" };
            if (liftparameter is not null)
                _ = liftparameter.AfterValidateRangeParameterAsync(zugang);
        }

        if (message.PropertyName == "var_Paneelmaterial")
        {
            CanShowGlassPanels(message.NewValue);
        }

        if (message.PropertyName == "var_PaneelmaterialGlas")
        {
            CanShowGlassPanelsColor(message.NewValue);
        }

        if (message.PropertyName == "var_SpiegelA" ||
            message.PropertyName == "var_SpiegelB" ||
            message.PropertyName == "var_SpiegelC" ||
            message.PropertyName == "var_SpiegelD" ||
            message.PropertyName == "var_Spiegelausfuehrung")
        {
            CanShowMirrorDimensions();
        }

        if (message.PropertyName == "var_ZUGANSSTELLEN_A" ||
            message.PropertyName == "var_ZUGANSSTELLEN_B" ||
            message.PropertyName == "var_ZUGANSSTELLEN_C" ||
            message.PropertyName == "var_ZUGANSSTELLEN_D")
        {
            SetDistanceBetweenDoors();
        }

        if (message.PropertyName == "var_Sockelleiste")
        {
            SetSkirtingBoardHeight(true);
        }

        if (message.PropertyName == "var_Fahrkorbtyp")
        {
            if (ParameterDictionary is not null)
                CheckIsDefaultCarTyp();
        }

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    private bool isCarCeilingReadOnly;

    [ObservableProperty]
    private string carCeilingReadOnlyInfo = "Parameter wird automatisch gesetzt";

    [ObservableProperty]
    private bool showCarWidthBetweenDoors;

    [ObservableProperty]
    private bool showCarDepthBetweenDoors;

    [ObservableProperty]
    private string? infoTextCarWidthBetweenDoors;

    [ObservableProperty]
    private string? infoTextCarDepthBetweenDoors;

    [ObservableProperty]
    private string? infoTextCarCeiling;

    [ObservableProperty]
    private bool isCarCeilingOverwritten;

    [ObservableProperty]
    private string? mirrorDimensionsWidth1;

    [ObservableProperty]
    private string? mirrorDimensionsWidth2;

    [ObservableProperty]
    private string? mirrorDimensionsWidth3;

    [ObservableProperty]
    private string? mirrorDimensionsHeight1;

    [ObservableProperty]
    private string? mirrorDimensionsHeight2;

    [ObservableProperty]
    private string? mirrorDimensionsHeight3;

    [ObservableProperty]
    private bool showMirrorDimensions2;

    [ObservableProperty]
    private bool showMirrorDimensions3;

    [ObservableProperty]
    private string floorImagePath = @"/Images/NoImage.png";

    [ObservableProperty]
    private bool showFlooringColors;

    [ObservableProperty]
    private bool canEditFlooringProperties;

    [ObservableProperty]
    private bool canEditFloorWeightAndHeight;

    [ObservableProperty]
    private bool showGlassPanels;

    [ObservableProperty]
    private bool showGlassPanelsColor;

    private double _floorHeight;

    private void SetCanEditFlooringProperties(string name, string newValue, string oldValue)
    {
        CanEditFlooringProperties = ParameterDictionary!["var_Bodenbelag"].Value == "Nach Beschreibung" || ParameterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
        CanEditFloorWeightAndHeight = ParameterDictionary!["var_Bodentyp"].Value == "sonder" || ParameterDictionary["var_Bodentyp"].Value == "extern";
        ShowFlooringColors = ParameterDictionary!["var_BodenbelagsTyp"].DropDownList.Any();

        if (!CanEditFloorWeightAndHeight)
        {
            if (ParameterDictionary["var_SonderExternBodengewicht"].Value != "0")
            {
                ParameterDictionary["var_SonderExternBodengewicht"].AutoUpdateParameterValue(string.Empty);
            }
            return;
        }

        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KU");

        switch (name)
        {
            case "var_Bodenbelagsdicke":
                double newFloorThinkness = string.IsNullOrWhiteSpace(newValue) ? 0 : Convert.ToDouble(newValue, CultureInfo.CurrentCulture);
                double oldFloorThinkness = string.IsNullOrWhiteSpace(oldValue) ? 0 : Convert.ToDouble(oldValue, CultureInfo.CurrentCulture);
                ParameterDictionary["var_KU"].AutoUpdateParameterValue(currentFloorHeight <= 0 ? Convert.ToString(newFloorThinkness) :
                                                                              Convert.ToString(currentFloorHeight - oldFloorThinkness + newFloorThinkness));
                break;
            case "var_Bodentyp":
                ParameterDictionary["var_KU"].AutoUpdateParameterValue(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodenbelagsdicke"));
                break;
            default:
                break;
        };
    }

    private void SetFloorImagePath()
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary!["var_BodenbelagsTyp"].Value))
        {
            FloorImagePath = @"/Images/NoImage.png";
            return;
        }
        var floorColor = _parametercontext.Set<CarFloorColorTyp>().FirstOrDefault(x => x.Name.Equals(ParameterDictionary!["var_BodenbelagsTyp"].Value));
        if (floorColor is not null)
        {
            FloorImagePath = $@"C:/Work/Administration/Standardeinstellungen/Inventor/Textures/flooring/{floorColor.Image}";
        }
        else
        {
            FloorImagePath = @"/Images/NoImage.png";
        }
    }

    private void CanShowMirrorDimensions()
    {
        List<string> mirrors = new();

        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelA"))
            mirrors.Add("A");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelB"))
            mirrors.Add("B");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelC"))
            mirrors.Add("C");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelD"))
            mirrors.Add("D");

        ShowMirrorDimensions2 = mirrors.Count > 1;
        ShowMirrorDimensions3 = mirrors.Count > 2;
        MirrorDimensionsWidth1 = "Breite Spiegel";
        MirrorDimensionsHeight1 = "Höhe Spiegel";

        if (mirrors.Count > 0)
        {
            MirrorDimensionsWidth1 = $"Breite Spiegel Wand {mirrors[0]}";
            MirrorDimensionsHeight1 = $"Höhe Spiegel Wand {mirrors[0]}";
        }
        if (mirrors.Count > 1)
        {
            MirrorDimensionsWidth2 = $"Breite Spiegel Wand {mirrors[1]}";
            MirrorDimensionsHeight2 = $"Höhe Spiegel Wand {mirrors[1]}";
        }
        if (mirrors.Count > 2)
        {
            MirrorDimensionsWidth3 = $"Breite Spiegel Wand {mirrors[2]}";
            MirrorDimensionsHeight3 = $"Höhe Spiegel Wand {mirrors[2]}";
        }
    }

    private void CanShowGlassPanels(string? paneelmaterial)
    {
        ShowGlassPanels = !string.IsNullOrWhiteSpace(paneelmaterial) && paneelmaterial.StartsWith("ESG");
    }

    private void CanShowGlassPanelsColor(string? paneelmaterialGlas)
    {
        ShowGlassPanelsColor = !string.IsNullOrWhiteSpace(paneelmaterialGlas) && paneelmaterialGlas.StartsWith("Euro");
    }

    private void SetSkirtingBoardHeight(bool modify)
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary!["var_Sockelleiste"].Value))
        {
            ParameterDictionary!["var_SockelleisteOKFF"].Value = string.Empty;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(ParameterDictionary!["var_SockelleisteOKFF"].Value) || modify)
            {

                var skirtingHeight = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == ParameterDictionary!["var_Sockelleiste"].Value)?.Height;
                if (skirtingHeight is not null)
                {
                    ParameterDictionary!["var_SockelleisteOKFF"].Value = (skirtingHeight + 10d).ToString();
                }
            }
        }
    }

    private void CheckCarCeilingIsOverwritten()
    {
        IsCarCeilingOverwritten = !string.IsNullOrWhiteSpace(ParameterDictionary!["var_overrideDefaultCeiling"].Value) ||
                                  !string.IsNullOrWhiteSpace(ParameterDictionary!["var_overrideSuspendedCeiling"].Value);
        if (IsCarCeilingOverwritten)
        {
            InfoTextCarCeiling += string.IsNullOrWhiteSpace(ParameterDictionary!["var_overrideDefaultCeiling"].Value) ? string.Empty : $"|{ParameterDictionary!["var_overrideDefaultCeiling"].Value} mm| Hauptdecke überschieben\n";
            InfoTextCarCeiling += string.IsNullOrWhiteSpace(ParameterDictionary!["var_overrideSuspendedCeiling"].Value) ? string.Empty : $"|{ParameterDictionary!["var_overrideSuspendedCeiling"].Value} mm| abgehängte Decke überschieben";
        }
    }

    private void SetDistanceBetweenDoors()
    {
        var zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A");
        var zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
        var zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
        var zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");

        ShowCarDepthBetweenDoors = zugangA || zugangC;
        ShowCarWidthBetweenDoors = zugangB || zugangD;
        if (ShowCarDepthBetweenDoors)
            InfoTextCarDepthBetweenDoors = _calculationsModuleService.GetDistanceBetweenDoors(ParameterDictionary, "Kabinentiefe");
        if (ShowCarWidthBetweenDoors)
            InfoTextCarWidthBetweenDoors = _calculationsModuleService.GetDistanceBetweenDoors(ParameterDictionary, "Kabinenbreite");
    }

    private void CheckIsDefaultCarTyp()
    {
        IsCarCeilingReadOnly = LiftParameterHelper.IsDefaultCarTyp(ParameterDictionary!["var_Fahrkorbtyp"].Value);
        CarCeilingReadOnlyInfo = IsCarCeilingReadOnly ? "Parameter wird automatisch gesetzt" : "Parameter können von händisch eingegeben werden";
    }

    [RelayCommand]
    private void GoToKabineDetail()
    {
        //TODO navigationService
        //_navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineDetailViewModel");
    }

    private async Task SetCalculatedValuesAsync()
    {
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParameterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParameterDictionary!, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);
        await Task.CompletedTask;
    }

    public void CarFloor_GotFocus(object sender, RoutedEventArgs e)
    {
        ParameterNumberTextBox CarFloortextBox = (ParameterNumberTextBox)sender;
        _floorHeight = string.IsNullOrWhiteSpace(CarFloortextBox.LiftParameter?.Value) ? 0 : Convert.ToDouble(CarFloortextBox.LiftParameter?.Value, CultureInfo.CurrentCulture);
    }

    public void CarFloor_LostFocus(object sender, RoutedEventArgs e)
    {
        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KU");
        double currentFloorThinkness = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Bodenbelagsdicke");
        if (_floorHeight != currentFloorHeight)
        {
            ParameterDictionary!["var_KU"].AutoUpdateParameterValue(Convert.ToString(currentFloorHeight + currentFloorThinkness, CultureInfo.CurrentCulture));
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            SetCanEditFlooringProperties("OnNavigatedTo", string.Empty, string.Empty);
            SetFloorImagePath();
            CanShowMirrorDimensions();
            SetSkirtingBoardHeight(false);
            CheckCarCeilingIsOverwritten();
            CanShowGlassPanels(ParameterDictionary!["var_Paneelmaterial"].Value);
            CanShowGlassPanelsColor(ParameterDictionary!["var_PaneelmaterialGlas"].Value);
            SetDistanceBetweenDoors();
            CheckIsDefaultCarTyp();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}