using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public class SchachtViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{


    public SchachtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        SetHauptzugangCommand = new RelayCommand<string>(SetHauptzugang);
        ResetHauptzugangCommand = new RelayCommand(ResetHauptzugang);
    }

    public IRelayCommand SetHauptzugangCommand
    {
        get;
    }
    public IRelayCommand ResetHauptzugangCommand
    {
        get;
    }

    private void SetHauptzugang(string? parameter)
    {
        if (!string.Equals(ParamterDictionary!["var_Haupthaltestelle"].Value, parameter, StringComparison.OrdinalIgnoreCase))
        {
            ParamterDictionary["var_Haupthaltestelle"].Value = parameter;
            DisplayNameHauptzugang = parameter;
        }
    }

    private void ResetHauptzugang()
    {
        if (!string.Equals(ParamterDictionary!["var_Haupthaltestelle"].Value, "NV", StringComparison.OrdinalIgnoreCase))
        {
            ParamterDictionary["var_Haupthaltestelle"].Value = "NV";
            DisplayNameHauptzugang = "NV";
        }
    }

    private string? _DisplayNameHauptzugang;
    public string? DisplayNameHauptzugang
    {
        get => ParamterDictionary!["var_Haupthaltestelle"].Value == "NV"
                ? (_DisplayNameHauptzugang = "Kein Hauptzugang gewählt")
                : (_DisplayNameHauptzugang = ParamterDictionary["var_Haupthaltestelle"].Value.Replace("ZG_", ""));
        set => SetProperty(ref _DisplayNameHauptzugang, value);
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties is not null && _CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
