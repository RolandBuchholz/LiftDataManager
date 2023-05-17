﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using System.Reflection.PortableExecutable;

namespace LiftDataManager.ViewModels;

public partial class BausatzViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public BausatzViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
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
            if (message.PropertyName == "var_TypFV" ||
                message.PropertyName == "var_FuehrungsschieneFahrkorb"||
                message.PropertyName == "var_Fuehrungsart")
            {
                SetSafetygearData();
            };
            SetInfoSidebarPanelText(message);
            _ = SetModelStateAsync();
        }
    }

    [ObservableProperty]
    private string cWTRailName = "Führungsschienen GGW";

    [ObservableProperty]
    private string cWTGuideName = "Führungsart GGW";

    [ObservableProperty]
    private string maxFuse = string.Empty;

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
        if (string.IsNullOrEmpty(fangrahmenTyp))
            return 0;
        var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
        if (carFrameType is null)
            return 0;
        CWTRailName = carFrameType.DriveTypeId == 2 ? "Führungsschienen Joch" : "Führungsschienen GGW";
        CWTGuideName = carFrameType.DriveTypeId == 2 ? "Führungsart Joch" : "Führungsart GGW";
        return carFrameType.CarFrameWeight;
    }

    private void SetMaxFuse()
    {
        if (!string.IsNullOrWhiteSpace(ParamterDictionary!["var_ZA_IMP_Regler_Typ"].Value))
        {
            var inverterSize = ParamterDictionary!["var_ZA_IMP_Regler_Typ"].Value!.Replace(" ","")[8..11];
            var inverterType = _parametercontext.Set<LiftInverterType>().FirstOrDefault(x => x.Name.Contains(inverterSize));
            MaxFuse =  inverterType is not null ? inverterType.MaxFuseSize.ToString() : string.Empty;
        }
        else
        {
            MaxFuse = string.Empty;
        }
    }

    private void SetSafetygearData()
    {
        var safteyGearResult = _calculationsModuleService.GetSafetyGearCalculation(ParamterDictionary);
        Safetygearworkarea = $"{safteyGearResult.MinLoad} - {safteyGearResult.MaxLoad} kg | {safteyGearResult.CarRailSurface} / {safteyGearResult.Lubrication} | Schienenkopf : {safteyGearResult.AllowedRailHeads}";
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        SetMaxFuse();
        SetSafetygearData();
        SetCarWeight();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}