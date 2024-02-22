using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Schema;

namespace LiftDataManager.Core.Models.ComponentModels;
public class CarFrameProgramOptions
{
    private readonly Dictionary<string,string> _listofProperty = new();

    public CarFrameProgramOptions()
    {
        SetListofPropertys();
    }

    private void SetListofPropertys()
    {
        var probs = GetType().GetProperties();
        foreach (var property in probs)
        {
            var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                _listofProperty.Add(property.Name, name);
            }
        }
    }

    public bool IsCFPOption(string optionName)
    {
       return _listofProperty.ContainsValue(optionName);
    }

    public void SetOption(string optionName, int optionValue)
    {
        var propertyName = _listofProperty.FirstOrDefault(x => x.Value == optionName).Key;
        if (propertyName is not null)
            GetType().GetProperty(propertyName)?.SetValue(this, optionValue);
    }

    [JsonPropertyName("mit_Seilen_TG2")]
    public int RopesScopeOfDeliveryTG2 { get; set; }

    [JsonPropertyName("mit_Seilen_BR2")]
    public int RopesScopeOfDeliveryBR2 { get; set; }

    [JsonPropertyName("mit_Seilen_BT2")]
    public int RopesScopeOfDeliveryBT2 { get; set; }

    [JsonPropertyName("mit_Seilen_BRR")]
    public int RopesScopeOfDeliveryBRR { get; set; }

    [JsonPropertyName("mit_Seilen_ZZE_S")]
    public int RopesScopeOfDeliveryZZES { get; set; }

    [JsonPropertyName("mit_Seilen_EZE_SR")]
    public int RopesScopeOfDeliveryEZESR { get; set; }

    [JsonPropertyName("mit_Schienen_TG2")]
    public int RailsScopeOfDeliveryTG2 { get; set; }

    [JsonPropertyName("mit_Schienen_BR1")]
    public int RailsScopeOfDeliveryBR1 { get; set; }

    [JsonPropertyName("mit_Schienen_BR2")]
    public int RailsScopeOfDeliveryBR2 { get; set; }

    [JsonPropertyName("mit_Schienen_BT1")]
    public int RailsScopeOfDeliveryBT1 { get; set; }

    [JsonPropertyName("mit_Schienen_BT2")]
    public int RailsScopeOfDeliveryBT2 { get; set; }

    [JsonPropertyName("mit_Schienen_BRR")]
    public int RailsScopeOfDeliveryBRR { get; set; }

    [JsonPropertyName("mit_Schienen_ZZE_S")]
    public int RailsScopeOfDeliveryZZES { get; set; }

    [JsonPropertyName("mit_Schienen_EZE_SR")]
    public int RailsScopeOfDeliveryEZESR { get; set; }

    [JsonPropertyName("Schienenbuegelbefestigungsmaterial_EZE_SR")]
    public int RailsBracketsFasteningMaterialScopeOfDeliveryEZESR { get; set; }

    [JsonPropertyName("mit_Buegeln_TG2")]
    public int RailsBracketsScopeOfDeliveryTG2 { get; set; }

    [JsonPropertyName("mit_Buegeln_BR1")]
    public int RailsBracketsScopeOfDeliveryBR1 { get; set; }

    [JsonPropertyName("mit_Buegeln_BR2")]
    public int RailsBracketsScopeOfDeliveryBR2 { get; set; }

    [JsonPropertyName("mit_Buegeln_BT1")]
    public int RailsBracketsScopeOfDeliveryBT1 { get; set; }

    [JsonPropertyName("mit_Buegeln_BT2")]
    public int RailsBracketsScopeOfDeliveryBT2 { get; set; }

    [JsonPropertyName("mit_Buegeln_BRR")]
    public int RailsBracketsScopeOfDeliveryBRR { get; set; }

    [JsonPropertyName("mit_Buegeln_ZZE_S")]
    public int RailsBracketsScopeOfDeliveryZZES { get; set; }

    [JsonPropertyName("mit_Buegeln_EZE_SR")]
    public int RailsBracketsScopeOfDeliveryEZESR { get; set; }

    [JsonPropertyName("Hilfsschienentyp_BT1")]
    public int YokeRailsScopeOfDeliveryBT1 { get; set; }

    [JsonPropertyName("Hilfsschienentyp_BT2")]
    public int YokeRailsScopeOfDeliveryBT2 { get; set; }

    [JsonPropertyName("Hilfsschienentyp_EZE_SR")]
    public int CWTRailEZESR { get; set; }

    [JsonPropertyName("Fuehrungsart_BR1")]
    public int GuideTypBR1 { get; set; }

    [JsonPropertyName("Fuehrungsart_BR2")]
    public int GuideTypBR2 { get; set; }

    [JsonPropertyName("Fuehrungsart_BT1")]
    public int GuideTypBT1 { get; set; }

    [JsonPropertyName("Fuehrungsart_BT2")]
    public int GuideTypBT2 { get; set; }

    [JsonPropertyName("Fuehrungsart_BRR")]
    public int GuideTypBRR { get; set; }

    [JsonPropertyName("Fuehrungsart_ZZE_S")]
    public int GuideTypZZES { get; set; }

    [JsonPropertyName("Fuehrungsart_EZE_SR")]
    public int GuideTypEZESR { get; set; }

    [JsonPropertyName("GB_ID_BR2")]
    public int OverspeedGovernorTypBR2 { get; set; }

    [JsonPropertyName("GB_ID_BT2")]
    public int OverspeedGovernorTypBT2 { get; set; }

    [JsonPropertyName("GB_ID_BRR")]
    public int OverspeedGovernorTypBRR { get; set; }

    [JsonPropertyName("GB_ID_ZZE_S")]
    public int OverspeedGovernorTypZZES { get; set; }

    [JsonPropertyName("GB_ID_EZE_SR")]
    public int OverspeedGovernorTypEZESR { get; set; }

    [JsonPropertyName("Fangvorrichtungindex_BR2")]
    public int OverspeedGovernorIndexBR2 { get; set; }

    [JsonPropertyName("FA_TG2")]
    public int RemoteControlTG2 { get; set; }

    [JsonPropertyName("FA_BR2")]
    public int RemoteControlBR2 { get; set; }

    [JsonPropertyName("FA_BT2")]
    public int RemoteControlBT2 { get; set; }

    [JsonPropertyName("FA_BRR")]
    public int RemoteControlBRR { get; set; }

    [JsonPropertyName("FA_ZZE_S")]
    public int RemoteControlZZES { get; set; }

    [JsonPropertyName("FA_EZE_SR")]
    public int RemoteControlEZESR { get; set; }

    [JsonPropertyName("MR_TG2")]
    public int MachineRoomTG2 { get; set; }

    [JsonPropertyName("OLEA_TG2")]
    public int LeakOilPodTG2 { get; set; }

    [JsonPropertyName("OLEA_BR1")]
    public int LeakOilPodBR1 { get; set; }

    [JsonPropertyName("OLEA_BR2")]
    public int LeakOilPodBR2 { get; set; }

    [JsonPropertyName("OLEA_BT1")]
    public int LeakOilPodBT1 { get; set; }

    [JsonPropertyName("OLEA_BT2")]
    public int LeakOilPodBT2 { get; set; }

    [JsonPropertyName("OLEA_BRR")]
    public int LeakOilPodBRR { get; set; }

    [JsonPropertyName("OLEA_ZZE_S")]
    public int LeakOilPodZZES { get; set; }

    [JsonPropertyName("PawlDevice_BT1")]
    public int PawlDeviceBT1 { get; set; }

    [JsonPropertyName("PawlDevice_BT2")]
    public int PawlDeviceBT2 { get; set; }

    [JsonPropertyName("Staplerbeladung_BT1")]
    public int StackerLoadingBT1 { get; set; }

    [JsonPropertyName("Staplerbeladung_BT2")]
    public int StackerLoadingBT2 { get; set; }

    [JsonPropertyName("XL_BT1")]
    public int XLBT1 { get; set; }

    [JsonPropertyName("XR_BT1")]
    public int XRBT1 { get; set; }

    [JsonPropertyName("XL_BT2")]
    public int XLBT2 { get; set; }

    [JsonPropertyName("XR_BT2")]
    public int XRBT2 { get; set; }
}
