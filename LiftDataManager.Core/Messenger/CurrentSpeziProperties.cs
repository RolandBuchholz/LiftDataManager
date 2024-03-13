using Cogs.Collections;
using MvvmHelpers;

namespace LiftDataManager.Core.Messenger;

public class CurrentSpeziProperties
{
    public ObservableDictionary<string, Parameter>? ParameterDictionary { get; set; }
    public bool Adminmode { get; set; }
    public bool CustomAccentColor { get; set; }
    public bool AuftragsbezogeneXml { get; set; }
    public bool CheckOut { get; set; }
    public bool LikeEditParameter { get; set; }
    public bool HideInfoErrors { get; set; }
    public SpezifikationTyp? CurrentSpezifikationTyp { get; set; }
    public string? FullPathXml { get; set; }
    public string? SearchInput { get; set; }
    public ObservableRangeCollection<InfoCenterEntry>? InfoCenterEntrys { get; set; }
}