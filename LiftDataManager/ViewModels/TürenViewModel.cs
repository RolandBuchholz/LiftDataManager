﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class TürenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public TürenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {

    }

    [ObservableProperty]
    private bool showCarDoorDataB;

    [ObservableProperty]
    private bool showCarDoorDataC;

    [ObservableProperty]
    private bool showCarDoorDataD;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetVariableCarDoorDataCommand))]
    private bool canSetVariableCarDoorData;

    [RelayCommand(CanExecute = nameof(CanSetVariableCarDoorData))]
    private async Task SetVariableCarDoorDataAsync()
    {
        var currentVariableCarDoorData = Convert.ToBoolean(ParamterDictionary!["var_Variable_Tuerdaten"].Value);
        ParamterDictionary["var_Variable_Tuerdaten"].Value = (currentVariableCarDoorData) ? "false" : "true";
        SetCarDoorDataVisibility();
        await Task.CompletedTask;
    }

    private void SetCarDoorDataVisibility()
    {
        var variableCarDoorData = Convert.ToBoolean(ParamterDictionary!["var_Variable_Tuerdaten"].Value);
        var zugangB = Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_B"].Value);
        var zugangC = Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_C"].Value);
        var zugangD = Convert.ToBoolean(ParamterDictionary["var_ZUGANSSTELLEN_D"].Value);

        CanSetVariableCarDoorData = (zugangB || zugangC || zugangD);

        ShowCarDoorDataB = (variableCarDoorData && zugangB);
        ShowCarDoorDataC = (variableCarDoorData && zugangC);
        ShowCarDoorDataD = (variableCarDoorData && zugangD);
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null)
            _ = SetModelStateAsync();
        SetCarDoorDataVisibility();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
