using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Controls;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.ViewModels;

public partial class KabineViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;

    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                           ISettingService settingService, ILogger<DataViewModelBase> baseLogger, ICalculationsModule calculationsModuleService, ParameterContext parametercontext) :
                           base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _calculationsModuleService = calculationsModuleService;
        _parametercontext = parametercontext;
    }

    private readonly string[] carEquipment = [ "var_SpiegelA", "var_SpiegelB", "var_SpiegelC", "var_SpiegelD",
                                               "var_HandlaufA", "var_HandlaufB", "var_HandlaufC", "var_HandlaufD",
                                               "var_SockelleisteA", "var_SockelleisteB", "var_SockelleisteC", "var_SockelleisteD",
                                               "var_TeilungsleisteA", "var_TeilungsleisteB", "var_TeilungsleisteC", "var_TeilungsleisteD",
                                               "var_RammschutzA", "var_RammschutzB", "var_RammschutzC", "var_RammschutzD",
                                               "var_PaneelPosA", "var_PaneelPosB", "var_PaneelPosC", "var_PaneelPosD",
                                               "var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D"];

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
            SetFlooringColorAndSurface();
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
                string[] zugang = [$"var_ZUGANSSTELLEN_{liftparameter.Name[^1..]}"];
                _ = liftparameter.AfterValidateRangeParameterAsync(zugang);
            }
        }

        if (message.PropertyName == "var_Rueckwand")
        {
            var liftparameter = message.Sender as Parameter;
            string[] zugang = ["var_ZUGANSSTELLEN_C"];
            if (liftparameter is not null)
                _ = liftparameter.AfterValidateRangeParameterAsync(zugang);
        }

        if (message.PropertyName == "var_Paneelmaterial")
        {
            CanShowGlassPanels(((Parameter)message.Sender).DropDownListValue?.Id);
        }

        if (message.PropertyName == "var_PaneelmaterialGlas")
        {
            CanShowGlassPanelsColor(((Parameter)message.Sender).DropDownListValue?.Id);
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

        if (message.PropertyName == "var_Sockelleiste" ||
            message.PropertyName == "var_SockelleisteHoeheBenutzerdefiniert")
        {
            SetSkirtingBoardHeight(true);
        }

        if (message.PropertyName == "var_Rammschutz" ||
            message.PropertyName == "var_AnzahlReihenRammschutz")
        {
            SetRammingProtectionSelection();
        }

        if (message.PropertyName == "var_Fahrkorbtyp")
        {
            if (ParameterDictionary is not null)
            {
                CheckIsDefaultCarTyp();
            }
        }

        if (message.PropertyName == "var_Handlauf")
        {
            if (ParameterDictionary is not null)
            {
                SetHandRailHeight();
            }
        }

        if (message.PropertyName == "var_VentilatorLuftmenge")
        {
            if (ParameterDictionary is not null && message.NewValue == "False")
            {
                ParameterDictionary["var_VentilatorAnzahl"].Value = string.Empty;
            }
        }

        if (message.PropertyName == "var_Teilungsleiste")
        {
            if (ParameterDictionary is not null)
            {
                SetDivisionBarHeight();
            }
        }

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    public partial bool IsCarCeilingReadOnly { get; set; }

    [ObservableProperty]
    public partial string CarCeilingReadOnlyInfo { get; set; } = "Parameter wird automatisch gesetzt";

    [ObservableProperty]
    public partial bool ShowCarWidthBetweenDoors { get; set; }

    [ObservableProperty]
    public partial bool ShowCarDepthBetweenDoors { get; set; }

    [ObservableProperty]
    public partial string? InfoTextCarWidthBetweenDoors { get; set; }

    [ObservableProperty]
    public partial string? InfoTextCarDepthBetweenDoors { get; set; }

    [ObservableProperty]
    public partial string? InfoTextCarCeiling { get; set; }

    [ObservableProperty]
    public partial bool IsCarCeilingOverwritten { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsWidth1 { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsWidth2 { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsWidth3 { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsHeight1 { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsHeight2 { get; set; }

    [ObservableProperty]
    public partial string? MirrorDimensionsHeight3 { get; set; }

    [ObservableProperty]
    public partial bool ShowMirrorDimensions2 { get; set; }

    [ObservableProperty]
    public partial bool ShowMirrorDimensions3 { get; set; }

    [ObservableProperty]
    public partial string FloorImagePath { get; set; } = @"/Images/NoImage.png";

    [ObservableProperty]
    public partial bool ShowFlooringColors { get; set; }

    [ObservableProperty]
    public partial bool ShowFlooringSurface { get; set; }

    [ObservableProperty]
    public partial bool CanEditFlooringProperties { get; set; }

    [ObservableProperty]
    public partial bool CanEditFloorWeightAndHeight { get; set; }

    [ObservableProperty]
    public partial bool ShowGlassPanels { get; set; }

    [ObservableProperty]
    public partial bool ShowGlassPanelsColor { get; set; }

    [ObservableProperty]
    public partial bool ShowCustomSkirtingBoards { get; set; }

    [ObservableProperty]
    public partial bool ShowCustomRammingProtections { get; set; }

    [ObservableProperty]
    public partial bool ShowRammingProtectionsRowsCount { get; set; }

    [ObservableProperty]
    public partial bool ShowRammingProtectionsRowHeight1 { get; set; }

    [ObservableProperty]
    public partial bool ShowRammingProtectionsRowHeight2 { get; set; }

    [ObservableProperty]
    public partial bool ShowRammingProtectionsRowHeight3 { get; set; }

    private double _floorHeight;

    private void SetCanEditFlooringProperties(string name, string newValue, string oldValue)
    {
        CanEditFlooringProperties = ParameterDictionary["var_Bodenbelag"].Value == "Nach Beschreibung" || ParameterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
        CanEditFloorWeightAndHeight = ParameterDictionary["var_Bodentyp"].Value == "sonder" || ParameterDictionary["var_Bodentyp"].Value == "extern";

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

    private void SetFlooringColorAndSurface()
    {
        ShowFlooringColors = !string.IsNullOrWhiteSpace(ParameterDictionary["var_Bodenbelag"].Value) &&
                             ParameterDictionary["var_BodenbelagsTyp"].DropDownList.Any();
        ShowFlooringSurface = ParameterDictionary["var_Bodenbelag"].DropDownListValue?.Id == 7;
        if (ShowFlooringSurface && string.IsNullOrWhiteSpace(ParameterDictionary["var_BodenbelagOberflaeche"].Value))
        {
            ParameterDictionary["var_BodenbelagOberflaeche"].AutoUpdateParameterValue("poliert R9 (matt)");
        }
        if (!ShowFlooringSurface && !string.IsNullOrWhiteSpace(ParameterDictionary["var_BodenbelagOberflaeche"].Value))
        {
            ParameterDictionary["var_BodenbelagOberflaeche"].AutoUpdateParameterValue(string.Empty);
        }
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
        List<string> mirrors = [];

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

    private void CanShowGlassPanels(int? id)
    {
        ShowGlassPanels = id == 1;
    }

    private void CanShowGlassPanelsColor(int? id)
    {
        ShowGlassPanelsColor = id == 4 || id == 5;
    }

    private void SetSkirtingBoardHeight(bool modify)
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Sockelleiste"].Value))
        {
            ParameterDictionary["var_SockelleisteOKFF"].Value = string.Empty;
            ParameterDictionary["var_SockelleisteHoeheBenutzerdefiniert"].Value = string.Empty;
            ParameterDictionary["var_SockelleisteBreiteBenutzerdefiniert"].Value = string.Empty;
            ParameterDictionary["var_SockelleisteGewichtBenutzerdefiniert"].Value = string.Empty;
            ShowCustomSkirtingBoards = false;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(ParameterDictionary["var_SockelleisteOKFF"].Value) || modify)
            {
                var skirtingHeight = _parametercontext.Set<SkirtingBoard>().FirstOrDefault(x => x.Name == ParameterDictionary["var_Sockelleiste"].Value)?.Height;
                if (skirtingHeight is not null)
                {
                    if (skirtingHeight != 0)
                    {
                        ParameterDictionary["var_SockelleisteOKFF"].Value = (skirtingHeight + 10d).ToString();
                        ParameterDictionary["var_SockelleisteHoeheBenutzerdefiniert"].Value = string.Empty;
                        ParameterDictionary["var_SockelleisteBreiteBenutzerdefiniert"].Value = string.Empty;
                        ParameterDictionary["var_SockelleisteGewichtBenutzerdefiniert"].Value = string.Empty;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_SockelleisteHoeheBenutzerdefiniert"].Value))
                        {
                            if (double.TryParse(ParameterDictionary["var_SockelleisteHoeheBenutzerdefiniert"].Value, out double customSkirtingHeight))
                            {
                                ParameterDictionary["var_SockelleisteOKFF"].Value = (customSkirtingHeight + 10d).ToString();
                            }
                        }
                        else
                        {
                            ParameterDictionary["var_SockelleisteOKFF"].Value = string.Empty;
                        }
                    }
                }
            }
            ShowCustomSkirtingBoards = ParameterDictionary["var_Sockelleiste"].Value == "gemäß Beschreibung";
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

    private void SetHandRailHeight()
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_HoeheHandlauf"].Value))
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Handlauf"].Value))
        {
            return;
        }
        ParameterDictionary["var_HoeheHandlauf"].Value = "900";
    }

    private void SetDivisionBarHeight()
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_TeilungsleisteOKFF"].Value))
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Teilungsleiste"].Value))
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Handlauf"].Value))
        {
            ParameterDictionary["var_TeilungsleisteOKFF"].Value = "900";
        }
        else
        {
            ParameterDictionary["var_TeilungsleisteOKFF"].Value = ParameterDictionary["var_Handlauf"].Value!.Contains("HL 13") ? 
                                                                  ParameterDictionary["var_HoeheHandlauf"].Value :
                                                                  (LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_HoeheHandlauf") - 52d).ToString();
        }
    }

    private void SetRammingProtectionSelection()
    {
        var rammingProtectionTyp = ParameterDictionary["var_Rammschutz"].Value;

        if (string.IsNullOrWhiteSpace(rammingProtectionTyp))
        {
            ParameterDictionary["var_AnzahlReihenRammschutz"].Value = string.Empty;
            ParameterDictionary["var_RammschutzHoeheBenutzerdefiniert"].Value = string.Empty;
            ParameterDictionary["var_RammschutzBreiteBenutzerdefiniert"].Value = string.Empty;
            ParameterDictionary["var_RammschutzGewichtBenutzerdefiniert"].Value = string.Empty;
            ShowCustomRammingProtections = false;
            ShowRammingProtectionsRowsCount = false;
            ShowRammingProtectionsRowHeight1 = false;
            ShowRammingProtectionsRowHeight2 = false;
            ShowRammingProtectionsRowHeight3 = false;
        }
        else
        {
            ShowCustomRammingProtections = rammingProtectionTyp == "Rammschutz siehe Beschreibung";

            var rammingRowsDB = _calculationsModuleService.GetRammingProtectionRows(ParameterDictionary, rammingProtectionTyp);
            if (rammingRowsDB != -1)
            {
                ShowRammingProtectionsRowsCount = rammingRowsDB == 0;

                int rammingRows;

                if (rammingRowsDB == 0)
                {
                    if (int.TryParse(ParameterDictionary["var_AnzahlReihenRammschutz"].Value, out int result))
                    {
                        rammingRows = result;
                    }
                    else
                    {
                        rammingRows = 1;
                        ParameterDictionary["var_AnzahlReihenRammschutz"].Value = rammingRows.ToString();
                    }
                }
                else
                {
                    rammingRows = rammingRowsDB;
                    ParameterDictionary["var_AnzahlReihenRammschutz"].Value = rammingRowsDB.ToString();
                }
                ShowRammingProtectionsRowHeight1 = rammingRows >= 1;
                ShowRammingProtectionsRowHeight2 = rammingRows >= 2;
                ShowRammingProtectionsRowHeight3 = rammingRows >= 3;
            }
        }
    }

    private void AktivateAutoMirrorCalculation()
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_BreiteSpiegel"].Value) && string.IsNullOrWhiteSpace(ParameterDictionary["var_HoeheSpiegel"].Value))
        {
            ParameterDictionary["var_AutoDimensionsMirror"].AutoUpdateParameterValue("True");
        }
    }

    [RelayCommand]
    private static void GoToKabineDetail()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabineDetailPage));
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
            ParameterDictionary["var_KU"].AutoUpdateParameterValue(Convert.ToString(currentFloorHeight + currentFloorThinkness, CultureInfo.CurrentCulture));
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            SetCanEditFlooringProperties("OnNavigatedTo", string.Empty, string.Empty);
            SetFlooringColorAndSurface();
            SetFloorImagePath();
            CanShowMirrorDimensions();
            SetSkirtingBoardHeight(false);
            CheckCarCeilingIsOverwritten();
            CanShowGlassPanels(ParameterDictionary["var_Paneelmaterial"].DropDownListValue?.Id);
            CanShowGlassPanelsColor(ParameterDictionary["var_PaneelmaterialGlas"].DropDownListValue?.Id);
            SetDistanceBetweenDoors();
            CheckIsDefaultCarTyp();
            SetHandRailHeight();
            SetRammingProtectionSelection();
            AktivateAutoMirrorCalculation();
            SetDivisionBarHeight();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}