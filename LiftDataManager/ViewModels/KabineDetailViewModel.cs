using Cogs.Collections;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public ObservableDictionary<string, double?> CarFloorSillParameter { get; set; }
    public ObservableCollection<string?> OpeningDirections { get; set; }

    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, IInfoCenterService infoCenterService) :
     base(parameterDataService, dialogService, navigationService, infoCenterService)
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

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            CanRefreshCarFloorSill = !LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_AutogenerateFloorDoorData");
            FillCarFloorSillParameter();
        }
        OpeningDirectionA = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung");
        OpeningDirectionB = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B");
        OpeningDirectionC = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C");
        OpeningDirectionD = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D");
        CheckIsOpeningDirectionSelected();
        CanShowMirrorDimensions();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}