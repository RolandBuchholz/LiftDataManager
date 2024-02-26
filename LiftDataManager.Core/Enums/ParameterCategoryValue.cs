using Ardalis.SmartEnum;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// German categorys of parameter
/// </summary>

public sealed class ParameterCategoryValue : SmartEnum<ParameterCategoryValue>
{
    //    /// <summary>
    //    /// Category not definiert
    //    /// </summary>
    public static readonly ParameterCategoryValue None = new(nameof(None), 0);
    //    /// <summary>
    //    /// Category general data
    //    /// </summary>
    public static readonly ParameterCategoryValue AllgemeineDaten = new(nameof(AllgemeineDaten), 1);
    //    /// <summary>
    //    /// Category shaft data
    //    /// </summary>
    public static readonly ParameterCategoryValue Schacht = new(nameof(Schacht), 2);
    //    /// <summary>
    //    /// Category carframe data
    //    /// </summary>
    public static readonly ParameterCategoryValue Bausatz = new(nameof(Bausatz), 3);
    //    /// <summary>
    //    /// Category car data
    //    /// </summary>
    public static readonly ParameterCategoryValue Fahrkorb = new(nameof(Fahrkorb), 4);
    //    /// <summary>
    //    /// Category liftdoor data
    //    /// </summary>
    public static readonly ParameterCategoryValue Tueren = new(nameof(Tueren), 5);
    //    /// <summary>
    //    /// Category drive controls 
    //    /// </summary>
    public static readonly ParameterCategoryValue AntriebSteuerungNotruf = new(nameof(AntriebSteuerungNotruf), 6);
    //    /// <summary>
    //    /// Category emergency call
    //    /// </summary>
    public static readonly ParameterCategoryValue Signalisation = new(nameof(Signalisation), 7);
    //    /// <summary>
    //    /// Category maintenance
    //    /// </summary>
    public static readonly ParameterCategoryValue Wartung = new(nameof(Wartung), 8);
    //    /// <summary>
    //    /// Category montage technical control
    //    /// </summary>
    public static readonly ParameterCategoryValue MontageTUEV = new(nameof(MontageTUEV), 9);
    //    /// <summary>
    //    /// Category smoke heat ventilation system
    //    /// </summary>
    public static readonly ParameterCategoryValue RWA = new(nameof(RWA), 10);
    //    /// <summary>
    //    /// Category other
    //    /// </summary>
    public static readonly ParameterCategoryValue Sonstiges = new(nameof(Sonstiges), 11);
    //    /// <summary>
    //    /// Category vaultcomments
    //    /// </summary>
    public static readonly ParameterCategoryValue KommentareVault = new(nameof(KommentareVault), 12);
    //    /// <summary>
    //    /// Category car frame program
    //    /// </summary>
    public static readonly ParameterCategoryValue CFP = new(nameof(CFP), 13);
    //    /// <summary>
    //    /// Category car design
    //    /// </summary>
    public static readonly ParameterCategoryValue CarDesign = new(nameof(CarDesign), 14);

    private ParameterCategoryValue(string name, int value) : base(name, value)
    {
    }
}
