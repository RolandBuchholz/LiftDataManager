using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Messenger.Messages;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
    private ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public string? FullPathXml { get; set; }
    private string SpezifikationsNumber => !string.IsNullOrWhiteSpace(FullPathXml) ? Path.GetFileNameWithoutExtension(FullPathXml!).Replace("-AutoDeskTransfer", "") : string.Empty;
    private DateTime ZaHtmlCreationTime { get; set; }
    private Dictionary<string, string> ZliDataDictionary { get; set; }
    private Dictionary<string, List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>> ValidationDictionary { get; set; } = new();
    private List<ParameterStateInfo> ValidationResult { get; set; }
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public ValidationParameterDataService(ParameterContext parametercontext, ICalculationsModule calculationsModuleService)
    {
        IsActive = true;
        ParameterDictionary ??= new();
        ZliDataDictionary ??= new();
        ValidationResult ??= new();
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        GetValidationDictionary();
    }

    ~ValidationParameterDataService()
    {
        IsActive = false;
    }

    void IRecipient<SpeziPropertiesRequestMessage>.Receive(SpeziPropertiesRequestMessage message)
    {
        if (message == null)
            return;
        if (!message.HasReceivedResponse)
            return;
        if (message.Response is null)
            return;
        if (message.Response.ParameterDictionary is null)
            return;
        ParameterDictionary = message.Response.ParameterDictionary;
        FullPathXml = message.Response.FullPathXml;
    }

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string? name, string? displayname, string? value)
    {
        ValidationResult.Clear();

        if (name is null || displayname is null)
        {
            return ValidationResult;
        }

        if (!ValidationDictionary.ContainsKey(key: name))
        {
            return ValidationResult;
        }

        foreach (var rule in ValidationDictionary[name])
        {
            rule.Item1.Invoke(name, displayname, value, rule.Item2, rule.Item3);
        }

        if (!ValidationResult.Any())
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }

        await Task.CompletedTask;
        return ValidationResult;
    }

    public async Task ValidateAllParameterAsync()
    {
        foreach (var par in ParameterDictionary)
        {
            _ = par.Value.ValidateParameterAsync();
        }

        await Task.CompletedTask;
    }

    public async Task ValidateRangeOfParameterAsync(string[] range)
    {
        if (range is null || range.Length == 0)
            return;

        foreach (var par in ParameterDictionary)
        {
            if (range.Any(r => string.Equals(r, par.Value.Name)))
                _ = par.Value.ValidateParameterAsync();
        }

        await Task.CompletedTask;
    }

    private void GetValidationDictionary()
    {
        ValidationDictionary.Add("var_AuftragsNummer",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Error", null) });

        ValidationDictionary.Add("var_FabrikNummer",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Warning", null),
            new(ValidateJobNumber, "Warning", "var_AuftragsNummer") });

        ValidationDictionary.Add("var_Q",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>{ new(NotEmpty, "Error", null),
            new(ValidateCounterweightMass, "None", null),
            new(ValidateCarArea, "Error", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_F",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateCarweightWithoutFrame, "None", null),
            new(ValidateCounterweightMass, "None", null) });

        ValidationDictionary.Add("var_Kennwort",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Warning", null) });

        ValidationDictionary.Add("var_Betreiber",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Warning", null) });

        ValidationDictionary.Add("var_Projekt",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Warning", null) });

        ValidationDictionary.Add("var_InformationAufzug",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>{ new(NotEmpty, "Warning", "(keine Auswahl)"),
            new(ValidateJobNumber, "Warning", "var_AuftragsNummer") });

        ValidationDictionary.Add("var_Aufzugstyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmpty, "Error", "(keine Auswahl)"),
            new(ValidateCarFrameSelection, "None", null),
            new(ValidateCarArea, "Error", null)});

        ValidationDictionary.Add("var_FabriknummerBestand",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateJobNumber, "Warning", "var_AuftragsNummer") });

        ValidationDictionary.Add("var_ZUGANSSTELLEN_A",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbau"),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauB"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>{ new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauC"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmty, "Warning", "var_TuerEinbauD"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_TuerEinbau",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_A"),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_TuerEinbauB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_B"),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_TuerEinbauC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_C"),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_TuerEinbauD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyWhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_D"),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Geschwindigkeitsbegrenzer",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateJungblutOSG, "Informational", null) });

        ValidationDictionary.Add("var_FH",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyOr0, "Error", null),
            new(ValidateTravel, "Error", null),
            new(ValidateZAliftData, "Warning", null)});

        ValidationDictionary.Add("var_Etagenhoehe0",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe1",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe2",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe3",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe4",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe5",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe6",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe7",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Etagenhoehe8",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateTravel, "Error", null) });

        ValidationDictionary.Add("var_Bodentyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null) });

        ValidationDictionary.Add("var_Bodenblech",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null) });

        ValidationDictionary.Add("var_BoPr",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null) });

        ValidationDictionary.Add("var_Bodenbelagsdicke",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null) });

        ValidationDictionary.Add("var_Bodenbelag",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null),
            new(ValidateFloorColorTyps, "None", null)});

        ValidationDictionary.Add("var_KU",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KHLicht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> {
                new(ValidateCarHeight, "None", null),
                new(ValidateCarHeightExcludingSuspendedCeiling, "None", null) });;

        ValidationDictionary.Add("var_KD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KBI",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { 
                new(ValidateCarEntranceRightSide, "None", null),
                new(ValidateCarCeilingDetails, "None", null) });

        ValidationDictionary.Add("var_KTI",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_L1",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_L2",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_L3",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_L4",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_TB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null)});

        ValidationDictionary.Add("var_TB_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_TB_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_TB_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null),
            new(ValidateReducedCarDoorHeaderHeight, "Error", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Variable_Tuerdaten",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_TH",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_Tuertyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null),
            new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null)});

        ValidationDictionary.Add("var_Tuerbezeichnung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateEntryDimensions, "None", null)});

        ValidationDictionary.Add("var_Tuergewicht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_Ersatzmassnahmen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedProtectionSpaces, "Warning", "var_TypFV") });

        ValidationDictionary.Add("var_Fuehrungsart",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateGuideModel, "None", null),
            new(ValidateSafetyRange, "Error", null)});

        ValidationDictionary.Add("var_FuehrungsschieneFahrkorb",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateSafetyRange, "Error", null) });

        ValidationDictionary.Add("var_Fuehrungsart_GGW",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateGuideModel, "None", null) });

        ValidationDictionary.Add("var_Fangvorrichtung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateSafetyGear, "None", null) });

        ValidationDictionary.Add("var_TypFV",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateSafetyRange, "None", null),
            new(ValidateReducedProtectionSpaces, "Warning", "var_Ersatzmassnahmen")});

        ValidationDictionary.Add("var_Aggregat",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDriveSystemTypes, "None", null),
            new(ValidateUCMValues, "None", null) });

        ValidationDictionary.Add("var_A_Kabine",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarArea, "Error", null) });

        ValidationDictionary.Add("var_F_Korr",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCorrectionWeight, "Warning", null) });

        ValidationDictionary.Add("var_Steuerungstyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null),
            new(ValidateLiftPositionSystems, "Warning", "var_Schachtinformationssystem")});

        ValidationDictionary.Add("var_ElektrBremsenansteuerung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null),
            new(ValidateZAliftData, "Warning", null)});

        ValidationDictionary.Add("var_Schachtinformationssystem",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null),
            new(ValidateLiftPositionSystems, "Warning", "var_Steuerungstyp")});

        ValidationDictionary.Add("var_Treibscheibegehaertet",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Fremdbelueftung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Handlueftung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Erkennungsweg",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Totzeit",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Vdetektor",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null) });

        ValidationDictionary.Add("var_Schacht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateShaftWalls, "Informational", null) });

        ValidationDictionary.Add("var_Rammschutz",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateRammingProtections, "Informational", null) });

        ValidationDictionary.Add("var_Schachtgrubenleiter",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidatePitLadder, "None", null) });

        ValidationDictionary.Add("var_Tuertyp_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) });

        ValidationDictionary.Add("var_Tuertyp_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) });

        ValidationDictionary.Add("var_Tuertyp_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null),
            new(ValidateDoorSill, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null),
            new(ValidateDoorSill, "None", null),
            new(ValidateCarDoorHeaders, "None", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Aufsetzvorrichtung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateHydrauliclock, "None", null) });

        ValidationDictionary.Add("var_GGWNutzlastausgleich",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCounterweightMass, "None", null) });

        ValidationDictionary.Add("var_Schutzgelaender_A",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateProtectiveRailingSwitch, "None", null) });

        ValidationDictionary.Add("var_Schutzgelaender_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateProtectiveRailingSwitch, "None", null) });

        ValidationDictionary.Add("var_Schutzgelaender_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateProtectiveRailingSwitch, "None", null) });

        ValidationDictionary.Add("var_Schutzgelaender_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateProtectiveRailingSwitch, "None", null) });

        ValidationDictionary.Add("var_SchwellenprofilKabTuere",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null),
            new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_SchwellenprofilKabTuereB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_SchwellenprofilKabTuereC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_SchwellenprofilKabTuereD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tueroeffnung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tueroeffnung_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tueroeffnung_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_Tueroeffnung_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateEntryDimensions, "None", null) });

        ValidationDictionary.Add("var_SpiegelA",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateMirrorDimensions, "None", null) });

        ValidationDictionary.Add("var_SpiegelB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateMirrorDimensions, "None", null) });

        ValidationDictionary.Add("var_SpiegelC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateMirrorDimensions, "None", null) });

        ValidationDictionary.Add("var_SpiegelD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateMirrorDimensions, "None", null) });

        ValidationDictionary.Add("var_EN8171Cat012",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorSill, "None", null) });

        ValidationDictionary.Add("var_STflRollen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotTrueWhenTheOtherIsTrue, "Error", "var_SThlRollen") });

        ValidationDictionary.Add("var_KTflRollen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotTrueWhenTheOtherIsTrue, "Error", "var_KThlRollen") });

        ValidationDictionary.Add("var_SThlRollen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotTrueWhenTheOtherIsTrue, "Error", "var_STflRollen") });

        ValidationDictionary.Add("var_KThlRollen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotTrueWhenTheOtherIsTrue, "Error", "var_KTflRollen") });

        ValidationDictionary.Add("var_KabTuerKaempferBreiteA",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferBreiteB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferBreiteC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferBreiteD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferHoeheA",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferHoeheB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferHoeheC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_KabTuerKaempferHoeheD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_Tuerverriegelung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedCarDoorHeaderHeight, "Error", null) });

        ValidationDictionary.Add("var_abgeDeckeHoehe",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeightExcludingSuspendedCeiling, "None", null) });

        ValidationDictionary.Add("var_abgDecke",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarCeilingDetails, "None", null) });

        ValidationDictionary.Add("var_DeckenCSchienenHoehe",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarCeilingDetails, "None", null) });

        ValidationDictionary.Add("var_overrideDefaultCeiling",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarCeilingDetails, "None", null) });

        ValidationDictionary.Add("var_overrideSuspendedCeiling",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarCeilingDetails, "None", null) });

        ValidationDictionary.Add("var_Paneelmaterial",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateGlassPanelColor, "None", null) });

        ValidationDictionary.Add("var_PaneelmaterialGlas",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateGlassPanelColor, "None", null) });

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
                        new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ListContainsValue, "Error", null) });
                }
            }
        }

    }

    private static ParameterStateInfo.ErrorLevel SetSeverity(string? severity)
    {
        if (severity is null)
            return ParameterStateInfo.ErrorLevel.Error;

        return severity switch
        {
            "Error" => ParameterStateInfo.ErrorLevel.Error,
            "Warning" => ParameterStateInfo.ErrorLevel.Warning,
            "Informational" => ParameterStateInfo.ErrorLevel.Informational,
            _ => ParameterStateInfo.ErrorLevel.Error,
        };
    }
}