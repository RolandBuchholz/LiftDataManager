using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class KabineViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public KabineViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is not null)
        {
            if ( message.PropertyName == "var_KBI" ||
                 message.PropertyName == "var_KTI")
            {
                _ = SetCalculatedValuesAsync();
            };
            if (message.PropertyName == "var_Bodenbelag") SetCanEditFlooringProperties();
            
            SetInfoSidebarPanelText(message);

            _ = SetModelStateAsync();
        }
    }

    [ObservableProperty]
    public bool canEditFlooringProperties;

    public void SetCanEditFlooringProperties()
    {
        CanEditFlooringProperties = ParamterDictionary!["var_Bodenbelag"].Value == "Nach Beschreibung" || ParamterDictionary["var_Bodenbelag"].Value == "bauseits lt. Beschreibung";
    }

    [RelayCommand]
    private void GoToKabineDetail() => _navigationService!.NavigateTo("LiftDataManager.ViewModels.KabineDetailViewModel");

    private async Task SetCalculatedValuesAsync()
    {
        var areaPersonsRequestMessageResult = await WeakReferenceMessenger.Default.Send<AreaPersonsRequestMessageAsync>();

        if (areaPersonsRequestMessageResult is not null)
        {
            var person = areaPersonsRequestMessageResult.Personen;
            var nutzflaecheKabine = areaPersonsRequestMessageResult.NutzflaecheKabine;

            if (person > 0)
            {
                ParamterDictionary!["var_Personen"].Value = Convert.ToString(person);
            }
            if (nutzflaecheKabine > 0)
            {
                ParamterDictionary!["var_A_Kabine"].Value = Convert.ToString(nutzflaecheKabine);
            }
        }
        await Task.CompletedTask;
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