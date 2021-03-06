namespace LiftDataManager.ViewModels;

public class TürenViewModel : DataViewModelBase, INavigationAware
{
    public TürenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
        {
            if (m is not null && m.Value.IsDirty)
            {
                SetInfoSidebarPanelText(m);
                await CheckUnsavedParametresAsync();
            }
        });

        SetVariableCarDoorDataAsyncCommand = new AsyncRelayCommand(SetVariableCarDoorDataAsync, () => CanSetVariableCarDoorData);
    }

    public IAsyncRelayCommand SetVariableCarDoorDataAsyncCommand
    {
        get;
    }

    private bool _CanSetVariableCarDoorData;
    public bool CanSetVariableCarDoorData
    {
        get => _CanSetVariableCarDoorData;
        set
        {
            SetProperty(ref _CanSetVariableCarDoorData, value);
            SetVariableCarDoorDataAsyncCommand.NotifyCanExecuteChanged();
        }
    }

    private bool _ShowCarDoorDataB;
    public bool ShowCarDoorDataB
    {
        get => _ShowCarDoorDataB;
        set => SetProperty(ref _ShowCarDoorDataB, value);
    }

    private bool _ShowCarDoorDataC;
    public bool ShowCarDoorDataC
    {
        get => _ShowCarDoorDataC;
        set => SetProperty(ref _ShowCarDoorDataC, value);
    }

    private bool _ShowCarDoorDataD;
    public bool ShowCarDoorDataD
    {
        get => _ShowCarDoorDataD;
        set => SetProperty(ref _ShowCarDoorDataD, value);
    }

    private async Task SetVariableCarDoorDataAsync()
    {
        var currentVariableCarDoorData = Convert.ToBoolean(ParamterDictionary["var_Variable_Tuerdaten"].Value);
        ParamterDictionary["var_Variable_Tuerdaten"].Value = (currentVariableCarDoorData) ? "false" : "true";
        SetCarDoorDataVisibility();
        await Task.CompletedTask;
    }

    private void SetCarDoorDataVisibility()
    {
        var variableCarDoorData = Convert.ToBoolean(ParamterDictionary["var_Variable_Tuerdaten"].Value);
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
        SynchronizeViewModelParameter();
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null)
        {
            _ = CheckUnsavedParametresAsync();
        }
        SetCarDoorDataVisibility();
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }
}
