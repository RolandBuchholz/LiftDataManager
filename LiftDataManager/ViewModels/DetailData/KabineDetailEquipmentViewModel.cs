using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.Services;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailEquipmentViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public event Action? CarViewChanged;

    private readonly ICalculationsModule _calculationsModuleService;

    public Dictionary<string, float> CarEquipmentDataBaseData { get; set; }

    public KabineDetailEquipmentViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ICalculationsModule calculationsModuleService) :
     base(parameterDataService, dialogService, infoCenterService)
    {
        _calculationsModuleService = calculationsModuleService;
        CarEquipmentDataBaseData ??= new() 
        {
            {"SkirtingBoardHeight", 0f },
            {"HandrailHeight", 0f },
            {"RammingProtection",0f }
        } ;
    }

    private readonly string[] carEquipment = [ "var_BreiteSpiegel", "var_BreiteSpiegel2", "var_BreiteSpiegel3",
                                               "var_HoeheSpiegel", "var_HoeheSpiegel2", "var_HoeheSpiegel3",
                                               "var_BreiteSpiegelKorrektur", "var_BreiteSpiegelKorrektur2", "var_BreiteSpiegelKorrektur3",
                                               "var_HoeheSpiegelKorrektur", "var_HoeheSpiegelKorrektur2", "var_HoeheSpiegelKorrektur3",
                                               "var_AbstandSpiegelDecke", "var_AbstandSpiegelDecke2", "var_AbstandSpiegelDecke3",
                                               "var_AbstandSpiegelvonLinks", "var_AbstandSpiegelvonLinks2", "var_AbstandSpiegelvonLinks3"];

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;
        if (carEquipment.Contains(message.PropertyName))
        {
            RefreshView();
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

    private void CanShowMirrorDimensions()
    {
        List<string> mirrors = [];

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

    public void RefreshView()
    {
        CarViewChanged?.Invoke();
    }

    [RelayCommand]
    private static void GoToKabine()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabinePage));
    }

    [ObservableProperty]
    private PivotItem? selectedPivotItem;
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

    private void GetCarEquipmentDataBaseData()
    {
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Sockelleiste"].Value))
        {
            
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Handlauf"].Value))
        {

        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Rammschutz"].Value))
        {

        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CanShowMirrorDimensions();
        GetCarEquipmentDataBaseData();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}