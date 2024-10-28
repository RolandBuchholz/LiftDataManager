using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;

namespace LiftDataManager.Core.Services;
/// <summary>
/// A <see langword="class"/> that implements the <see cref="IValidationParameterDataService"/> <see langword="interface"/> using LiftDataManager Validation APIs.
/// </summary>
public partial class ValidationParameterDataService : IValidationParameterDataService
{
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
    private ObservableDictionary<string, Parameter> _parameterDictionary;
    private string? _fullPathXml;
    private string SpezifikationsNumber => !string.IsNullOrWhiteSpace(_fullPathXml) || _fullPathXml == pathDefaultAutoDeskTransfer ? 
                                           Path.GetFileNameWithoutExtension(_fullPathXml!).Replace("-AutoDeskTransfer", "") : string.Empty;
    private DateTime ZaHtmlCreationTime { get; set; }
    private DateTime CFPCreationTime { get; set; }
    private Dictionary<string, string> ZliDataDictionary { get; set; }
    private Dictionary<string, string> CFPDataDictionary { get; set; }
    private Dictionary<string, List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>> ValidationDictionary { get; set; } = [];
    private List<ParameterStateInfo> ValidationResult { get; set; }
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public ValidationParameterDataService(ParameterContext parametercontext, ICalculationsModule calculationsModuleService)
    {
        ZliDataDictionary ??= [];
        CFPDataDictionary ??= [];
        ValidationResult ??= [];
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _parameterDictionary = [];
        GetValidationDictionary();
    }

    /// <inheritdoc/>
    public async Task InitializeValidationParameterDataServicerAsync(ObservableDictionary<string, Parameter> parameterDictionary)
    {
        _parameterDictionary = parameterDictionary;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ResetAsync()
    {
        _fullPathXml = string.Empty;
        ZaHtmlCreationTime = default;
        CFPCreationTime = default;
        ZliDataDictionary.Clear();
        CFPDataDictionary.Clear();
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SetFullPathXmlAsync(string? path)
    {
        _fullPathXml = path;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string? name, string? displayname, string? value)
    {
        ValidationResult.Clear();
        if (name is null || 
            displayname is null ||
            !ValidationDictionary.TryGetValue(key: name, 
            out List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>? rules))
        {
            return ValidationResult;
        }
        foreach (var rule in rules)
        {
            rule.Item1.Invoke(name, displayname, value, rule.Item2, rule.Item3);
        }

        if (ValidationResult.Count == 0)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }

        await Task.CompletedTask;
        return ValidationResult;
    }

    /// <inheritdoc/>
    public async Task ValidateAllParameterAsync()
    {
        foreach (var par in _parameterDictionary)
        {
            await par.Value.ValidateParameterAsync();
        }
    }

    /// <inheritdoc/>
    public async Task ValidateRangeOfParameterAsync(string[] range)
    {
        if (range is null || range.Length == 0)
        {
            return;
        }
        foreach (var par in _parameterDictionary)
        {
            if (range.Any(r => string.Equals(r, par.Value.Name)))
                _ = par.Value.ValidateParameterAsync();
        }
        await Task.CompletedTask;
    }

    private void GetValidationDictionary()
    {
        ValidationDictionary.Add("var_ErstelltAm",
            [new(ValidateCreationDate, "None", null)]);

        ValidationDictionary.Add("var_AuftragsNummer",
            [new(NotEmpty, "Error", null)]);

        ValidationDictionary.Add("var_FabrikNummer",
            [new(NotEmpty, "Warning", null),
            new(ValidateJobNumber, "Warning", "var_AuftragsNummer") ]);

        ValidationDictionary.Add("var_Q",
            [new(NotEmpty, "Error", null),
            new(ValidateCounterweightMass, "None", null),
            new(ValidateCarArea, "Error", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateCarFrameProgramData, "Warning", null),
            new(ValidateZAliftData, "Warning", null) ]);

        ValidationDictionary.Add("var_F",
            [new(ValidateZAliftData, "Warning", null),
            new(ValidateCarFrameProgramData, "Warning", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateCarweightWithoutFrame, "None", null),
            new(ValidateCounterweightMass, "None", null) ]);

        ValidationDictionary.Add("var_Kennwort",
            [new(NotEmpty, "Warning", null)]);

        ValidationDictionary.Add("var_Betreiber",
            [new(NotEmpty, "Warning", null)]);

        ValidationDictionary.Add("var_Projekt",
            [new(NotEmpty, "Warning", null)]);

        ValidationDictionary.Add("var_InformationAufzug",
            [new(NotEmpty, "Warning", "(keine Auswahl)"),
            new(ValidateJobNumber, "Warning", "var_AuftragsNummer") ]);

        ValidationDictionary.Add("var_Aufzugstyp",
            [new(NotEmpty, "Error", "(keine Auswahl)"),
            new(ValidateCarFrameSelection, "None", null),
            new(ValidateCarArea, "Error", null)]);

        ValidationDictionary.Add("var_FabriknummerBestand",
            [new(ValidateJobNumber, "Warning", "var_AuftragsNummer")]);

        ValidationDictionary.Add("var_ZUGANSSTELLEN_A",
            [new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbau"),
            new(ValidateCarFramePosition, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)]);

        ValidationDictionary.Add("var_ZUGANSSTELLEN_B",
            [new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauB"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarFramePosition, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)]);

        ValidationDictionary.Add("var_ZUGANSSTELLEN_C",
            [new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauC"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarFramePosition, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)]);

        ValidationDictionary.Add("var_ZUGANSSTELLEN_D",
            [new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauD"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarFramePosition, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)]);

        ValidationDictionary.Add("var_TuerEinbau",
            [new(ValidateEntryDimensions, "None", null),
            new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_A"),
            new(ValidateCarDoorMountingDimensions, "Warning", null),]);

        ValidationDictionary.Add("var_TuerEinbauB",
            [new(ValidateEntryDimensions, "None", null),
            new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_B"),
            new(ValidateCarDoorMountingDimensions, "Warning", null)]);

        ValidationDictionary.Add("var_TuerEinbauC",
            [new(ValidateEntryDimensions, "None", null),
            new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_C"),
            new(ValidateCarDoorMountingDimensions, "Warning", null)]);

        ValidationDictionary.Add("var_TuerEinbauD",
            [new(ValidateEntryDimensions, "None", null),
            new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_D"),
            new(ValidateCarDoorMountingDimensions, "Warning", null)]);

        ValidationDictionary.Add("var_Geschwindigkeitsbegrenzer",
            [new(ValidateJungblutOSG, "Informational", null),
            new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_FH",
            [new(NotEmptyOr0, "Error", null),
            new(ValidateTravel, "Error", null),
            new(ValidateCarFrameProgramData, "Warning", null),
            new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Etagenhoehe0",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe1",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe2",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe3",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe4",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe5",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe6",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe7",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Etagenhoehe8",
            [new(ValidateTravel, "Error", null)]);

        ValidationDictionary.Add("var_Bodentyp",
            [new(ValidateCarFlooring, "None", null)]);

        ValidationDictionary.Add("var_Bodenblech",
            [new(ValidateCarFlooring, "None", null)]);

        ValidationDictionary.Add("var_BoPr",
            [new(ValidateCarFlooring, "None", null)]);

        ValidationDictionary.Add("var_Bodenbelagsdicke",
            [new(ValidateCarFlooring, "None", null)]);

        ValidationDictionary.Add("var_Bodenbelag",
            [ new(ValidateCarFlooring, "None", null),
            new(ValidateFloorColorTyps, "None", null)]);

        ValidationDictionary.Add("var_KU",
            [new(ValidateCarHeight, "None", null)]);

        ValidationDictionary.Add("var_KHLicht",
            [new(ValidateCarHeight, "None", null),
             new(ValidateCarFrameProgramData, "Warning", null),
             new(ValidateCarHeightExcludingSuspendedCeiling, "None", null) ]);

        ValidationDictionary.Add("var_KD",
            [new(ValidateCarHeight, "None", null)]);

        ValidationDictionary.Add("var_KBI",
            [new(ValidateCarEntranceRightSide, "None", null),
             new(ValidateCarCeilingDetails, "None", null),
             new(ValidateCarFrameProgramData, "Warning", null),
             new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_KTI",
            [new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateCarFrameProgramData, "Warning", null),
            new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_L1",
            [new(ValidateCarEntranceRightSide, "None", null)]);

        ValidationDictionary.Add("var_L2",
            [new(ValidateCarEntranceRightSide, "None", null)]);

        ValidationDictionary.Add("var_L3",
            [new(ValidateCarEntranceRightSide, "None", null)]);

        ValidationDictionary.Add("var_L4",
            [new(ValidateCarEntranceRightSide, "None", null)]);

        ValidationDictionary.Add("var_TB",
            [new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_TB_B",
            [new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_TB_C",
            [new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_TB_D",
            [new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_Variable_Tuerdaten",
            [new(ValidateVariableCarDoors, "None", null)]);

        ValidationDictionary.Add("var_TH",
            [new(ValidateVariableCarDoors, "None", null)]);

        ValidationDictionary.Add("var_Tuertyp",
            [new(ValidateVariableCarDoors, "None", null),
            new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null)]);

        ValidationDictionary.Add("var_Tuerbezeichnung",
            [new(ValidateDoorData, "None", null),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_Tuergewicht",
            [new(ValidateVariableCarDoors, "None", null)]);

        ValidationDictionary.Add("var_Ersatzmassnahmen",
            [new(ValidateReducedProtectionSpaces, "Warning", "var_TypFV")]);

        ValidationDictionary.Add("var_Fuehrungsart",
            [new(ValidateGuideModel, "None", null),
            new(ValidateSafetyRange, "Error", null)]);

        ValidationDictionary.Add("var_FuehrungsschieneFahrkorb",
            [new(ValidateSafetyRange, "Error", null),
            new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_FuehrungsschieneGegengewicht",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_Fuehrungsart_GGW",
            [new(ValidateGuideModel, "None", null)]);

        ValidationDictionary.Add("var_Fangvorrichtung",
            [new(ValidateSafetyGear, "None", null)]);

        ValidationDictionary.Add("var_TypFV",
            [new(ValidateSafetyRange, "None", null),
            new(ValidateCarFrameProgramData, "Warning", null),
            new(ValidateReducedProtectionSpaces, "Warning", "var_Ersatzmassnahmen")]);

        ValidationDictionary.Add("var_Aggregat",
            [new(ValidateDriveSystemTypes, "None", null),
            new(ValidateUCMValues, "None", null) ]);

        ValidationDictionary.Add("var_A_Kabine",
            [new(ValidateCarArea, "Error", null)]);

        ValidationDictionary.Add("var_SkipRatedLoad",
            [new(ValidateCarArea, "Error", null)]);

        ValidationDictionary.Add("var_F_Korr",
            [new(ValidateCorrectionWeight, "Warning", null)]);

        ValidationDictionary.Add("var_Steuerungstyp",
            [new(ValidateUCMValues, "None", null),
            new(ValidateLiftPositionSystems, "Warning", "var_Schachtinformationssystem")]);

        ValidationDictionary.Add("var_ElektrBremsenansteuerung",
            [new(ValidateUCMValues, "None", null),
            new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Schachtinformationssystem",
            [new(ValidateUCMValues, "None", null),
            new(ValidateLiftPositionSystems, "Warning", "var_Steuerungstyp"),
            new(ValidateSchindlerCertifiedComponents, "None", null)]);

        ValidationDictionary.Add("var_Treibscheibegehaertet",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Fremdbelueftung",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Handlueftung",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Erkennungsweg",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Totzeit",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Vdetektor",
            [new(ValidateZAliftData, "Warning", null)]);

        ValidationDictionary.Add("var_Schacht",
            [new(ValidateShaftWalls, "Informational", null)]);

        ValidationDictionary.Add("var_Schachtgrubenleiter",
            [new(ValidatePitLadder, "None", null)]);

        ValidationDictionary.Add("var_Tuertyp_B",
            [new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) ]);

        ValidationDictionary.Add("var_Tuertyp_C",
            [new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) ]);

        ValidationDictionary.Add("var_Tuertyp_D",
            [new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) ]);

        ValidationDictionary.Add("var_Tuerbezeichnung_B",
            [new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_Tuerbezeichnung_C",
            [new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_Tuerbezeichnung_D",
            [new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_Aufsetzvorrichtung",
            [new(ValidateHydrauliclock, "None", null)]);

        ValidationDictionary.Add("var_GGWNutzlastausgleich",
            [new(ValidateCounterweightMass, "None", null)]);

        ValidationDictionary.Add("var_Schutzgelaender_A",
            [new(ValidateProtectiveRailingSwitch, "None", null)]);

        ValidationDictionary.Add("var_Schutzgelaender_B",
            [new(ValidateProtectiveRailingSwitch, "None", null)]);

        ValidationDictionary.Add("var_Schutzgelaender_C",
            [new(ValidateProtectiveRailingSwitch, "None", null)]);

        ValidationDictionary.Add("var_Schutzgelaender_D",
            [new(ValidateProtectiveRailingSwitch, "None", null)]);

        ValidationDictionary.Add("var_SchwellenprofilKabTuere",
            [new(ValidateVariableCarDoors, "None", null),
            new(ValidateEntryDimensions, "None", null) ]);

        ValidationDictionary.Add("var_SchwellenprofilKabTuereB",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_SchwellenprofilKabTuereC",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_SchwellenprofilKabTuereD",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_Tueroeffnung",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_Tueroeffnung_B",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_Tueroeffnung_C",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_Tueroeffnung_D",
            [new(ValidateEntryDimensions, "None", null)]);

        ValidationDictionary.Add("var_SpiegelA",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_SpiegelB",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_SpiegelC",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_SpiegelD",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_EN8171Cat012",
            [new(ValidateDoorSill, "None", null)]);

        ValidationDictionary.Add("var_STflRollen",
            [new(NotTrueWhenTheOtherIsTrue, "Error", "var_SThlRollen")]);

        ValidationDictionary.Add("var_KTflRollen",
            [new(NotTrueWhenTheOtherIsTrue, "Error", "var_KThlRollen")]);

        ValidationDictionary.Add("var_SThlRollen",
            [new(NotTrueWhenTheOtherIsTrue, "Error", "var_STflRollen")]);

        ValidationDictionary.Add("var_KThlRollen",
            [new(NotTrueWhenTheOtherIsTrue, "Error", "var_KTflRollen")]);

        ValidationDictionary.Add("var_KabTuerKaempferBreiteA",
            [new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferBreiteB",
            [new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferBreiteC",
            [new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferBreiteD",
            [new(ValidateCarDoorMountingDimensions, "Warning", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferHoeheA",
            [new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferHoeheB",
            [new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferHoeheC",
            [new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_KabTuerKaempferHoeheD",
            [new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_Tuerverriegelung",
            [new(ValidateReducedCarDoorHeaderHeight, "Error", null)]);

        ValidationDictionary.Add("var_abgeDeckeHoehe",
            [new(ValidateCarHeightExcludingSuspendedCeiling, "None", null)]);

        ValidationDictionary.Add("var_abgDecke",
            [new(ValidateCarCeilingDetails, "None", null)]);

        ValidationDictionary.Add("var_DeckenCSchienenHoehe",
            [new(ValidateCarCeilingDetails, "None", null)]);

        ValidationDictionary.Add("var_overrideDefaultCeiling",
            [new(ValidateCarCeilingDetails, "None", null)]);

        ValidationDictionary.Add("var_overrideSuspendedCeiling",
            [new(ValidateCarCeilingDetails, "None", null)]);

        ValidationDictionary.Add("var_Paneelmaterial",
            [new(ValidateGlassPanelColor, "None", null)]);

        ValidationDictionary.Add("var_PaneelmaterialGlas",
            [new(ValidateGlassPanelColor, "None", null)]);

        ValidationDictionary.Add("var_Bausatz",
            [new(ValidateCarFramePosition, "None", null)]);

        ValidationDictionary.Add("var_Bausatzlage",
            [new(ValidateCarFramePosition, "None", null)]);

        ValidationDictionary.Add("var_AutoDimensionsMirror",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_SpiegelPaneel",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_PaneelPosA",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_PaneelPosB",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_PaneelPosC",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_PaneelPosD",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_KHRoh",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HoeheHandlauf",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_SockelleisteOKFF",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HandlaufA",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HandlaufB",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HandlaufC",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HandlaufD",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HoeheSpiegelKorrektur",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HoeheSpiegelKorrektur2",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_HoeheSpiegelKorrektur3",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_BreiteSpiegelKorrektur",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_BreiteSpiegelKorrektur2",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_BreiteSpiegelKorrektur3",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_Spiegelausfuehrung",
            [new(ValidateMirrorDimensions, "None", null)]);

        ValidationDictionary.Add("var_TypFuehrung",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_TypFuehrung_GGW",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_SG",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_SK",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_SB",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_ST",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_KHA",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_v",
            [new(ValidateCarFrameProgramData, "Warning", null)]);

        ValidationDictionary.Add("var_CFPdefiniert",
            [new(ValidateLayOutDrawingLoads, "None", null)]);

        ValidationDictionary.Add("var_GGW_Rahmen_Gewicht",
            [new(ValidateCounterweightMass, "None", null)]);

        ValidationDictionary.Add("var_Gegengewicht_Einlagenbreite",
            [new(ValidateCounterweightMass, "None", null)]);

        ValidationDictionary.Add("var_Gegengewicht_Einlagentiefe",
            [new(ValidateCounterweightMass, "None", null)]);

        ValidationDictionary.Add("var_Haupthaltestelle",
            [new(ValidateEntrancePosition, "Error", null)]);

        AddDropDownListValidation();
    }

    private void AddDropDownListValidation()
    {
        var dropDownParameters = _parametercontext.ParameterDtos?.Where(x => !string.IsNullOrWhiteSpace(x.DropdownList))
                                    .Select(x => x.Name)
                                    .ToList();
        if (dropDownParameters is not null)
        {
            foreach (var par in dropDownParameters)
            {
                if (ValidationDictionary.ContainsKey(par))
                {
                    ValidationDictionary[par].Add(new(ListContainsValue, "Error", null));
                }
                else
                {
                    ValidationDictionary.Add(par,
                        [new(ListContainsValue, "Error", null)]);
                }
            }
        }
    }

    private static ErrorLevel SetSeverity(string? severity)
    {
        if (severity is null)
            return ErrorLevel.Error;

        return severity switch
        {
            "Error" => ErrorLevel.Error,
            "Warning" => ErrorLevel.Warning,
            "Informational" => ErrorLevel.Informational,
            _ => ErrorLevel.Error,
        };
    }
}