﻿using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public ObservableDictionary<string, double?> CarFloorSillParameter { get; set; }
    public ObservableCollection<string?> OpeningDirections { get; set; }

    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
     base(parameterDataService, dialogService, navigationService)
    {
        CarFloorSillParameter = new ObservableDictionary<string, double?>();
        SetupCarFloorSillParameter();
        OpeningDirections = new ObservableCollection<string?>
        {
            "einseitig öffnend",
            "einseitig öffnend (rechts)",
            "einseitig öffnend (links)"
        };
    }
    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;

        if (message.PropertyName == "var_AutogenerateFloorDoorData")
        {
            if (!string.IsNullOrWhiteSpace(message.NewValue))
                CanRefreshCarFloorSill = !Convert.ToBoolean(message.NewValue, CultureInfo.CurrentCulture);
        }
        else if (!string.IsNullOrWhiteSpace(message.PropertyName) && message.PropertyName.StartsWith("var_Tueroeffnung"))
        {
            FillCarFloorSillParameter();
        }
        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    public string? openingDirectionA;
    partial void OnOpeningDirectionAChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public string? openingDirectionB;
    partial void OnOpeningDirectionBChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_B"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public string? openingDirectionC;
    partial void OnOpeningDirectionCChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_C"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public string? openingDirectionD;
    partial void OnOpeningDirectionDChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_D"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public bool openingDirectionNotSelected;
    private void SetupCarFloorSillParameter()
    {
        CarFloorSillParameter.Clear();
        for (int i = 1; i < 18; i++)
        {
            CarFloorSillParameter.Add($"{i}_A", 0);
            CarFloorSillParameter.Add($"{i}_B", 0);
            CarFloorSillParameter.Add($"{i}_C", 0);
            CarFloorSillParameter.Add($"{i}_D", 0);
        }
    }

    public bool CanEditSillEntranceA => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A") &&
                                        ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung")).StartsWith("einseitig");
    public bool CanEditSillEntranceB => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B") &&
                                        ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B")).StartsWith("einseitig");
    public bool CanEditSillEntranceC => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C") &&
                                        ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C")).StartsWith("einseitig");
    public bool CanEditSillEntranceD => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D") &&
                                        ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D")).StartsWith("einseitig");

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCarFloorSillParameterCommand))]
    private bool canRefreshCarFloorSill;

    [RelayCommand]
    private void GoToKabine() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineViewModel");

    [RelayCommand(CanExecute = nameof(CanRefreshCarFloorSill))]
    private void RefreshCarFloorSillParameter()
    {
        string[] accesses = new string[] { "A", "B", "C", "D" };
        foreach (var access in accesses)
        {
            if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{access}"))
            {
                var carFloorSillParameterString = new StringBuilder();
                var parameters = CarFloorSillParameter.Where(x => x.Key.EndsWith(access)).Select(s => s.Value.ToString());
                carFloorSillParameterString.AppendJoin(';', parameters);
                ParameterDictionary![$"var_SchwellenUnterbau{access}"].Value = carFloorSillParameterString.ToString();
            }
        }
    }

    private void FillCarFloorSillParameter()
    {
        if (ParameterDictionary is not null && ParameterDictionary.Any())
        {
            string[] accesses = new string[] { "A", "B", "C", "D" };
            foreach (var access in accesses)
            {
                if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_SchwellenUnterbau{access}"].Value))
                {
                    var cardesignparametres = ParameterDictionary[$"var_SchwellenUnterbau{access}"].Value!.Split(";");
                    for (int i = 0; i < cardesignparametres.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(cardesignparametres[i]))
                        {
                            CarFloorSillParameter[$"{i + 1}_{access}"] = 0;
                        }
                        else
                        {
                            CarFloorSillParameter[$"{i + 1}_{access}"] = Convert.ToDouble(cardesignparametres[i], CultureInfo.CurrentCulture);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 17; i++)
                    {
                        CarFloorSillParameter[$"{i + 1}_{access}"] = 0;
                    }
                }
            }
        }
    }

    private void CheckIsOpeningDirectionSelected()
    {
        OpeningDirectionNotSelected = (!string.IsNullOrWhiteSpace(openingDirectionA) && string.Equals(openingDirectionA, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionB) && string.Equals(openingDirectionB, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionC) && string.Equals(openingDirectionC, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionD) && string.Equals(openingDirectionD, "einseitig öffnend"));
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
            CanRefreshCarFloorSill = !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_AutogenerateFloorDoorData");
            FillCarFloorSillParameter();
        }
        OpeningDirectionA = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung");
        OpeningDirectionB = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B");
        OpeningDirectionC = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C");
        OpeningDirectionD = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D");
        CheckIsOpeningDirectionSelected();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
