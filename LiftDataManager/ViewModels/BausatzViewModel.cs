using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Services;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.ViewModels;

public class BausatzViewModel : DataViewModelBase, INavigationAware,IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;

    public BausatzViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null)
        {
            if (message.PropertyName == "var_Bausatz")
            {
                ParamterDictionary!["var_Rahmengewicht"].Value = "";
                FangrahmenGewicht = GetFangrahmengewicht(message.NewValue);
            };
            if (message.PropertyName == "var_Fangvorrichtung")
            {
                //ParamterDictionary!["var_TypFV"].DropDownList.
            };
            SetInfoSidebarPanelText(message);

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
            FangrahmenGewicht = GetFangrahmengewicht(ParamterDictionary!["var_Bausatz"].Value);
        }
        else
        {
            FangrahmenGewicht = 0;
        }
    }

    private double GetFangrahmengewicht(string? fangrahmenTyp)
    {
        if (string.IsNullOrEmpty(fangrahmenTyp)) return 0;
        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null) return 0;
        return carFrameType.CarFrameWeight;
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        SetCarWeight();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}