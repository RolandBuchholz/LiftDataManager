using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class SchachtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public SchachtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    private string? _DisplayNameHauptzugang;
    public string? DisplayNameHauptzugang
    {
        get => ParamterDictionary!["var_Haupthaltestelle"].Value == "NV"
                ? (_DisplayNameHauptzugang = "Kein Hauptzugang gewählt")
                : (_DisplayNameHauptzugang = !string.IsNullOrEmpty(ParamterDictionary["var_Haupthaltestelle"].Value) ? ParamterDictionary["var_Haupthaltestelle"].Value!.Replace("ZG_", "") : "Kein Hauptzugang gewählt");
        set => SetProperty(ref _DisplayNameHauptzugang, value);
    }

    [RelayCommand]
    private void SetHauptzugang(string? parameter)
    {
        if (!string.Equals(ParamterDictionary!["var_Haupthaltestelle"].Value, parameter, StringComparison.OrdinalIgnoreCase))
        {
            ParamterDictionary["var_Haupthaltestelle"].Value = parameter is not null ? parameter : string.Empty;
            DisplayNameHauptzugang = parameter;
        }
    }
    [RelayCommand]
    private void ResetHauptzugang()
    {
        if (!string.Equals(ParamterDictionary!["var_Haupthaltestelle"].Value, "NV", StringComparison.OrdinalIgnoreCase))
        {
            ParamterDictionary["var_Haupthaltestelle"].Value = "NV";
            DisplayNameHauptzugang = "NV";
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParamterDictionary is not null &&
            CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
