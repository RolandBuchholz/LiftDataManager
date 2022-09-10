using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class BausatzViewModel : DataViewModelBase, INavigationAware, IRecipient<CarFrameWeightRequestMessageAsync>,IRecipient<PropertyChangedMessage<string>>
{
    public BausatzViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    public BausatzViewModel()
    {
        CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();

        if (CurrentSpeziProperties.ParamterDictionary is not null)
        {
            ParamterDictionary = CurrentSpeziProperties.ParamterDictionary;
        }
        SetCarWeight();
    }

    public void Receive(CarFrameWeightRequestMessageAsync message)
    {
        if (!message.HasReceivedResponse)
        {
            message.Reply(new CalculatedValues
            {
                FangrahmenGewicht = FangrahmenGewicht
            });
        }
        IsActive=false;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null)
        {
            // ToDo Validation Service integrieren
            if (message.PropertyName == "var_Bausatz")
            {
                ParamterDictionary!["var_Rahmengewicht"].Value = "";
                GetFangrahmengewicht(message.NewValue);
            };
            SetInfoSidebarPanelText(message);
            //TODO Make Async
            _ = SetModelStateAsync();
        }
    }

    private double _FangrahmenGewicht;
    public double FangrahmenGewicht
    {
        get => _FangrahmenGewicht;
        set => SetProperty(ref _FangrahmenGewicht, value);
    }

    private void SetCarWeight()
    {
        if (!string.IsNullOrWhiteSpace(ParamterDictionary!["var_Rahmengewicht"].Value))
        {
            FangrahmenGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Rahmengewicht");
        }
        else if (!string.IsNullOrWhiteSpace(ParamterDictionary!["var_Bausatz"].Value))
        {
            GetFangrahmengewicht(ParamterDictionary!["var_Bausatz"].Value);
        }
        else
        {
            FangrahmenGewicht = 0;
        }
    }

    // ToDo Parameter aus Datenbank abrufen

    private void GetFangrahmengewicht(string fangrahmenTyp)
    {
        FangrahmenGewicht = fangrahmenTyp switch
        {
            "BRR-15 MK2" => 165,
            "BRR-25 MK2" => 255,
            "EZE-SR1250" => 195,
            "EZE-SR1400" => 200,
            "EZE-SR1600" => 280,
            "EZE-SR3200" => 580,
            "ZZE-S2600" => 430,
            "ZZE-S3200" => 500,
            "ZZE-S3600" => 700,
            "ZZE-S4200" => 850,
            "ZZE-S6200" => 900,
            "DZE-S8600" => 1680,
            "VZE-S10000" => 1950,
            "EZE-S1250" => 170,
            "EZE-S1600" => 220,
            "EZE-S2600" => 340,
            "EZE-S3600" => 550,
            "EZE-S4200" => 650,
            "BR1-15 MK2" => 165,
            "BR1-25 MK2" => 255,
            "BR1-35 MK2" => 389,
            "BR2-15 MK2" => 165,
            "BR2-25 MK2" => 255,
            "BR2-35 MK2" => 389,
            "TG2-15 MK2" => 131,
            "TG2-25 MK2" => 171,
            "BT1-40" => 765,
            "BT1-50" => 790,
            "BT1-60" => 812,
            "BT1-75" => 922,
            "BT1-90" => 1194,
            "BT1-150" => 1260,
            "BT1-170" => 1260,
            "BT2-40" => 765,
            "BT2-50" => 790,
            "BT2-60" => 812,
            "BT2-75" => 922,
            "BT2-90" => 1194,
            "BT2-120" => 1260,
            "BT2-170" => 1620,
            _ => 0,
        };
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        SetCarWeight();
        if (CurrentSpeziProperties is not null && CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}