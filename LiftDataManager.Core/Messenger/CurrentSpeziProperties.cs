using Cogs.Collections;

namespace LiftDataManager.Core.Messenger;

public class CurrentSpeziProperties
{
    public bool Adminmode
    {
        get; set;
    }

    public bool CustomAccentColor
    {
        get; set;
    }

    public bool AuftragsbezogeneXml
    {
        get; set;
    }

    public bool CheckOut
    {
        get; set;
    }

    public bool LikeEditParameter
    {
        get; set;
    }

    public string SpezifikationStatusTyp
    {
        get; set;
    }

    public string FullPathXml
    {
        get; set;
    }

    public string SearchInput
    {
        get; set;
    }

    public string InfoSidebarPanelText
    {
        get; set;
    }

    public ObservableDictionary<string, Parameter> ParamterDictionary
    {
        get; set;
    }
}
