using Ardalis.SmartEnum;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// InfoCenterEntry State with german humanized Displayname
/// </summary>

public sealed class InfoCenterEntryState : SmartEnum<InfoCenterEntryState>
{
    //    /// <summary>
    //    /// State not definiert
    //    /// </summary>
    public static readonly InfoCenterEntryState None = new("Keine", 0);
    //    /// <summary>
    //    /// State Message
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterMessage = new("Nachricht", 1);
    //    /// <summary>
    //    /// State Warning
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterWarning = new("Warnung", 2);
    //    /// <summary>
    //    /// State Error
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterError = new("Fehler", 3);
    //    /// <summary>
    //    /// State Parameter changed
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterParameterChanged = new("Parameterwert geändert", 4);
    //    /// <summary>
    //    /// State Parameter saved
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterSaveParameter = new("Parameterwert gespeichert", 5);
    //    /// <summary>
    //    /// State Parameter LDM auto update
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterAutoUpdate = new("LDM auto update", 6);

    private InfoCenterEntryState(string name, int value) : base(name, value)
    {
    }
}
