using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;



namespace LiftDataManager.ViewModels;

public partial class KabineViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ICalculationsModule _calculationsModuleService;


    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _calculationsModuleService = calculationsModuleService;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null)
        {
            if (message.PropertyName == "var_KBI" ||
                 message.PropertyName == "var_KTI")
            {
                _ = SetCalculatedValuesAsync();
            };
            if (message.PropertyName == "var_Bodenbelag" ||
                message.PropertyName == "var_Bodentyp" ||
                message.PropertyName == "var_Bodenbelagsdicke")
            {
                SetCanEditFlooringProperties(message.PropertyName, message.NewValue, message.OldValue);
            }
            SetInfoSidebarPanelText(message);
            _ = SetModelStateAsync();
        }
    }

    [ObservableProperty]
    public bool canEditFlooringProperties;

    [ObservableProperty]
    public bool canEditFloorWeightAndHeight;

    public void SetCanEditFlooringProperties(string name, string newValue, string oldValue)
    {
        CanEditFlooringProperties = ParamterDictionary!["var_Bodenbelag"].Value == "Nach Beschreibung" || ParamterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
        CanEditFloorWeightAndHeight = ParamterDictionary!["var_Bodentyp"].Value == "sonder" || ParamterDictionary["var_Bodentyp"].Value == "extern";
        if (!CanEditFloorWeightAndHeight)
        {
            ParamterDictionary["var_SonderExternBodengewicht"].Value = string.Empty;
            return;
        }

        double currentFloorHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KU");

        switch (name)
        {
            case "var_Bodenbelagsdicke":
                double newFloorThinkness = string.IsNullOrWhiteSpace(newValue) ? 0 : Convert.ToDouble(newValue, CultureInfo.CurrentCulture);
                double oldFloorThinkness = string.IsNullOrWhiteSpace(oldValue) ? 0 : Convert.ToDouble(oldValue, CultureInfo.CurrentCulture);
                ParamterDictionary["var_KU"].Value = currentFloorHeight <= 0? Convert.ToString(newFloorThinkness) : 
                                                                              Convert.ToString(currentFloorHeight - oldFloorThinkness + newFloorThinkness);
                break;
            case "var_Bodentyp":
                ParamterDictionary["var_KU"].Value = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsdicke");
                break;
            default:
                break;
        };
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
        TextBox CarFloortextBox = (TextBox)sender;
        double floorHeight = string.IsNullOrWhiteSpace(CarFloortextBox.Text) ? 0 : Convert.ToDouble(CarFloortextBox.Text, CultureInfo.CurrentCulture);
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
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}