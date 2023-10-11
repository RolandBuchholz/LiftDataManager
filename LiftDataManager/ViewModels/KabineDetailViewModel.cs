using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.core.Helpers;
using WinUIEx.Messaging;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public Dictionary<string, double> CarFloorSillParameter { get; set; }
    
    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        CarFloorSillParameter = new Dictionary<string, double>();
        SetupCarFloorSillParameter();
    }

    private void SetupCarFloorSillParameter()
    {
        CarFloorSillParameter.Clear();
        for (int i = 1; i < 17; i++)
        {
            CarFloorSillParameter.Add($"{i}_A", 0);
            CarFloorSillParameter.Add($"{i}_B", 0);
            CarFloorSillParameter.Add($"{i}_C", 0);
            CarFloorSillParameter.Add($"{i}_D", 0);
        }
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
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

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
            if (LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_ZUGANSSTELLEN_{access}"))
            {
                var carFloorSillParameterString = new StringBuilder();
                var parameters = CarFloorSillParameter.Where(x => x.Key.EndsWith(access)).Select(s => s.Value.ToString());
                carFloorSillParameterString.AppendJoin(';',parameters);
                ParamterDictionary![$"var_SchwellenUnterbau{access}"].Value = carFloorSillParameterString.ToString();
            }
        }
    }

    private void FillCarFloorSillParameter()
    {
        if (ParamterDictionary is not null && ParamterDictionary.Any())
        {
            string[] accesses = new string[] { "A", "B", "C", "D" };
            foreach (var access in accesses)
            {
                if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_SchwellenUnterbau{access}"].Value))
                {
                    var cardesignparametres = ParamterDictionary[$"var_SchwellenUnterbau{access}"].Value!.Split(";");
                    for (int i = 0; i < cardesignparametres.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(cardesignparametres[i]))
                        {
                            CarFloorSillParameter[$"{i}_{access}"] = 0;
                        }
                        else
                        {
                            CarFloorSillParameter[$"{i}_{access}"] = Convert.ToDouble(cardesignparametres[i], CultureInfo.CurrentCulture);
                        }
                    }
                }
            }
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
            CanRefreshCarFloorSill = !LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AutogenerateFloorDoorData");
            FillCarFloorSillParameter();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
