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
                                               "var_PaneelPosA", "var_PaneelPosB", "var_PaneelPosC", "var_PaneelPosD"};

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
        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    public string floorImagePath = @"/Images/NoImage.png";

    [ObservableProperty]
    public bool showFlooringColors;

    [ObservableProperty]
    public bool canEditFlooringProperties;

    [ObservableProperty]
    public bool canEditFloorWeightAndHeight;

    public void SetCanEditFlooringProperties(string name, string newValue, string oldValue)
    {
        CanEditFlooringProperties = ParamterDictionary!["var_Bodenbelag"].Value == "Nach Beschreibung" || ParamterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
        CanEditFloorWeightAndHeight = ParamterDictionary!["var_Bodentyp"].Value == "sonder" || ParamterDictionary["var_Bodentyp"].Value == "extern";
        ShowFlooringColors = ParamterDictionary!["var_BodenbelagsTyp"].DropDownList.Any();

        if (!CanEditFloorWeightAndHeight)
        {
            if (ParamterDictionary["var_SonderExternBodengewicht"].Value != "0")
            {
                ParamterDictionary["var_SonderExternBodengewicht"].Value = string.Empty;
            }
            return;
        }

        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KU");

        switch (name)
        {
            case "var_Bodenbelagsdicke":
                double newFloorThinkness = string.IsNullOrWhiteSpace(newValue) ? 0 : Convert.ToDouble(newValue, CultureInfo.CurrentCulture);
                double oldFloorThinkness = string.IsNullOrWhiteSpace(oldValue) ? 0 : Convert.ToDouble(oldValue, CultureInfo.CurrentCulture);
                ParamterDictionary["var_KU"].Value = currentFloorHeight <= 0 ? Convert.ToString(newFloorThinkness) :
                                                                              Convert.ToString(currentFloorHeight - oldFloorThinkness + newFloorThinkness);
                break;
            case "var_Bodentyp":
                ParamterDictionary["var_KU"].Value = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsdicke");
                break;
            default:
                break;
        };
    }

    public void SetFloorImagePath()
    {
        if (string.IsNullOrWhiteSpace(ParamterDictionary!["var_BodenbelagsTyp"].Value))
        {
            FloorImagePath = @"/Images/NoImage.png";
            return;
        }
        var floorColor = _parametercontext.Set<CarFloorColorTyp>().FirstOrDefault(x => x.Name.Equals(ParamterDictionary!["var_BodenbelagsTyp"].Value));
        if (floorColor is not null)
        {
            FloorImagePath = $@"C:/Work/Administration/Standardeinstellungen/Inventor/Textures/flooring/{floorColor.Image}";
        }
        else
        {
            FloorImagePath = @"/Images/NoImage.png";
        }
    }

    [RelayCommand]
    private void GoToKabineDetail() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineDetailViewModel");

    private async Task SetCalculatedValuesAsync()
    {
        var payLoadResult = _calculationsModuleService.GetPayLoadCalculation(ParamterDictionary);
        _calculationsModuleService.SetPayLoadResult(ParamterDictionary!, payLoadResult.PersonenBerechnet, payLoadResult.NutzflaecheGesamt);
        await Task.CompletedTask;
    }

    public void CarFloor_LostFocus(object sender, RoutedEventArgs e)
    {
        ParameterNumberTextBox CarFloortextBox = (ParameterNumberTextBox)sender;
        double floorHeight = string.IsNullOrWhiteSpace(CarFloortextBox.LiftParameter?.Value) ? 0 : Convert.ToDouble(CarFloortextBox.LiftParameter?.Value, CultureInfo.CurrentCulture);
        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KU");
        double currentFloorThinkness = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke");
        if (floorHeight + currentFloorThinkness != currentFloorHeight)
        {
            ParamterDictionary!["var_KU"].Value = Convert.ToString(floorHeight + currentFloorThinkness, CultureInfo.CurrentCulture);
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
            SetCanEditFlooringProperties("OnNavigatedTo", string.Empty, string.Empty);
            SetFloorImagePath();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}