﻿using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailFloorViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    public ObservableDictionary<string, double?> CarFloorSillParameter { get; set; }
    public ObservableCollection<string?> OpeningDirections { get; set; }

    public KabineDetailFloorViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                                      ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
                                      base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        CarFloorSillParameter = [];
        SetupCarFloorSillParameter();
        OpeningDirections =
        [
            "einseitig öffnend",
            "einseitig öffnend (rechts)",
            "einseitig öffnend (links)"
        ];
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
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
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    [ObservableProperty]
    public partial string? OpeningDirectionA { get; set; }
    partial void OnOpeningDirectionAChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionB { get; set; }
    partial void OnOpeningDirectionBChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_B"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionC { get; set; }
    partial void OnOpeningDirectionCChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_C"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionD { get; set; }
    partial void OnOpeningDirectionDChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_D"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial bool OpeningDirectionNotSelected { get; set; }
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
    public partial bool CanRefreshCarFloorSill { get; set; }

    [RelayCommand(CanExecute = nameof(CanRefreshCarFloorSill))]
    private void RefreshCarFloorSillParameter()
    {
        string[] accesses = ["A", "B", "C", "D"];
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
            string[] accesses = ["A", "B", "C", "D"];
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
        OpeningDirectionNotSelected = (!string.IsNullOrWhiteSpace(OpeningDirectionA) && string.Equals(OpeningDirectionA, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionB) && string.Equals(OpeningDirectionB, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionC) && string.Equals(OpeningDirectionC, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionD) && string.Equals(OpeningDirectionD, "einseitig öffnend"));
    }

    [RelayCommand]
    private static void GoToKabine()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabinePage));
    }

    [ObservableProperty]
    public partial PivotItem? SelectedPivotItem { get; set; }
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {

                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
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
        NavigatedFromBaseActions();
    }
}