using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Controls;
using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.ViewModels;

public partial class KabineViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ParameterContext _parametercontext;
 
    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ICalculationsModule calculationsModuleService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _calculationsModuleService = calculationsModuleService;
        _parametercontext = parametercontext;
    }

    private readonly string[] carEquipment = { "var_SpiegelA", "var_SpiegelB", "var_SpiegelC", "var_SpiegelD", 
                                               "var_HandlaufA", "var_HandlaufB", "var_HandlaufC", "var_HandlaufD",
                                               "var_SockelleisteA", "var_SockelleisteB", "var_SockelleisteC", "var_SockelleisteD",
                                               "var_RammschutzA", "var_RammschutzB", "var_RammschutzC", "var_RammschutzD",
                                               "var_PaneelPosA", "var_PaneelPosB", "var_PaneelPosC", "var_PaneelPosD",
                                               "var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D"};

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

        if (message.PropertyName == "var_SpiegelA" ||
            message.PropertyName == "var_SpiegelB" || 
            message.PropertyName == "var_SpiegelC" || 
            message.PropertyName == "var_SpiegelD" ||
            message.PropertyName == "var_Spiegelausfuehrung")
        {
            CanShowMirrorDimensions();
        }

        if (message.PropertyName == "var_Sockelleiste")
        {
            SetSkirtingBoardHeight(true);
        }

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

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

    private void SetCanEditFlooringProperties(string name, string newValue, string oldValue)
    {
        CanEditFlooringProperties = ParameterDictionary!["var_Bodenbelag"].Value == "Nach Beschreibung" || ParameterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
        CanEditFloorWeightAndHeight = ParameterDictionary!["var_Bodentyp"].Value == "sonder" || ParameterDictionary["var_Bodentyp"].Value == "extern";
        ShowFlooringColors = ParameterDictionary!["var_BodenbelagsTyp"].DropDownList.Any();

        if (!CanEditFloorWeightAndHeight)
        {
            if (ParameterDictionary["var_SonderExternBodengewicht"].Value != "0")
            {
                ParameterDictionary["var_SonderExternBodengewicht"].Value = string.Empty;
            }
            return;
        }

        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KU");

        switch (name)
        {
            case "var_Bodenbelagsdicke":
                double newFloorThinkness = string.IsNullOrWhiteSpace(newValue) ? 0 : Convert.ToDouble(newValue, CultureInfo.CurrentCulture);
                double oldFloorThinkness = string.IsNullOrWhiteSpace(oldValue) ? 0 : Convert.ToDouble(oldValue, CultureInfo.CurrentCulture);
                ParameterDictionary["var_KU"].Value = currentFloorHeight <= 0 ? Convert.ToString(newFloorThinkness) :
                                                                              Convert.ToString(currentFloorHeight - oldFloorThinkness + newFloorThinkness);
                break;
            case "var_Bodentyp":
                ParameterDictionary["var_KU"].Value = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodenbelagsdicke");
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

    [RelayCommand]
    private void GoToKabineDetail() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineDetailViewModel");

    private async Task SetCalculatedValuesAsync()
    {
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParameterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParameterDictionary!, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);
        await Task.CompletedTask;
    }

    public void CarFloor_LostFocus(object sender, RoutedEventArgs e)
    {
        ParameterNumberTextBox CarFloortextBox = (ParameterNumberTextBox)sender;
        double floorHeight = string.IsNullOrWhiteSpace(CarFloortextBox.LiftParameter?.Value) ? 0 : Convert.ToDouble(CarFloortextBox.LiftParameter?.Value, CultureInfo.CurrentCulture);
        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KU");
        double currentFloorThinkness = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Bodenbelagsdicke");
        if (floorHeight + currentFloorThinkness != currentFloorHeight)
        {
            ParameterDictionary!["var_KU"].Value = Convert.ToString(floorHeight + currentFloorThinkness, CultureInfo.CurrentCulture);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
            SetCanEditFlooringProperties("OnNavigatedTo", string.Empty, string.Empty);
            SetFloorImagePath();
            CanShowMirrorDimensions();
            SetSkirtingBoardHeight(false);
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}