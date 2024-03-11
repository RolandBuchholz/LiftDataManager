using Ardalis.SmartEnum;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// InfoCenterEntry State
/// </summary>

public sealed class InfoCenterEntryState : SmartEnum<InfoCenterEntryState>
{
    //    /// <summary>
    //    /// State not definiert
    //    /// </summary>
    public static readonly InfoCenterEntryState None = new(nameof(None), 0);
    //    /// <summary>
    //    /// State Message
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterMessage = new(nameof(InfoCenterMessage), 1);
    //    /// <summary>
    //    /// State Warning
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterWarning = new(nameof(InfoCenterWarning), 2);
    //    /// <summary>
    //    /// State Error
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterError = new(nameof(InfoCenterError), 3);
    //    /// <summary>
    //    /// State ParameterChanged
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterParameterChanged = new(nameof(InfoCenterParameterChanged), 4);
    //    /// <summary>
    //    /// State Save Parameter
    //    /// </summary>
    public static readonly InfoCenterEntryState InfoCenterSaveParameter = new (nameof(InfoCenterSaveParameter), 5);
    //    /// <summary>
    //    /// State Error
    //    /// </summary>
 
    private InfoCenterEntryState(string name, int value) : base(name, value)
    {
    }
}
