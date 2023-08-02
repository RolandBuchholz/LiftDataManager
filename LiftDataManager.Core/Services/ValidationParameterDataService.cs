using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Messenger.Messages;
using System.Globalization;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    private const string pathDefaultAutoDeskTransfer = @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml";
    private ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
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
        ParamterDictionary ??= new();
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
        if (message.Response.ParamterDictionary is null)
            return;
        ParamterDictionary = message.Response.ParamterDictionary;
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
        foreach (var par in ParamterDictionary)
        {
            _ = par.Value.ValidateParameterAsync();
        }

        await Task.CompletedTask;
    }

    public async Task ValidateRangeOfParameterAsync(string[] range)
    {
        if (range == null)
            return;
        if (range.Length == 0)
            return;

        foreach (var par in ParamterDictionary)
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
            new(ValidateCarArea, "Error", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateZAliftData, "Warning", null)});

        ValidationDictionary.Add("var_F",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateZAliftData, "Warning", null),
            new(ValidateSafetyRange, "Error", null),
            new(ValidateCarweightWithoutFrame, "None", null)});

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbau"),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauB"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>{ new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauC"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_ZUGANSSTELLEN_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauD"),
            new(ValidateVariableCarDoors, "None", null),
            new(ValidateCarEquipmentPosition, "Error", null)});

        ValidationDictionary.Add("var_TuerEinbau",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_A") });

        ValidationDictionary.Add("var_TuerEinbauB",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_B") });

        ValidationDictionary.Add("var_TuerEinbauC",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_C") });

        ValidationDictionary.Add("var_TuerEinbauD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_D") });

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarFlooring, "None", null) });

        ValidationDictionary.Add("var_KU",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KHLicht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KHA",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KD",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarHeight, "None", null) });

        ValidationDictionary.Add("var_KBI",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

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
            new(ValidateCarEntranceRightSide, "None", null)});

        ValidationDictionary.Add("var_TB_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_TB_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_TB_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_Variable_Tuerdaten",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_TH",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_Tuertyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null),
            new(ValidateDoorTyps, "None", null)});

        ValidationDictionary.Add("var_Tuerbezeichnung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null),
            new(ValidateVariableCarDoors, "None", null)});

        ValidationDictionary.Add("var_Tuergewicht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateVariableCarDoors, "None", null) });

        ValidationDictionary.Add("var_Ersatzmassnahmen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedProtectionSpaces, "None", null) });

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateSafetyRange, "None", null) });

        ValidationDictionary.Add("var_Aggregat",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDriveSystemTypes, "None", null),
            new(ValidateUCMValues, "None", null) });

        ValidationDictionary.Add("var_A_Kabine",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarArea, "Error", null) });

        ValidationDictionary.Add("var_F_Korr",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCorrectionWeight, "Warning", null) });

        ValidationDictionary.Add("var_Steuerungstyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null) });

        ValidationDictionary.Add("var_ElektrBremsenansteuerung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null),
            new(ValidateZAliftData, "Warning", null)});

        ValidationDictionary.Add("var_Schachtinformationssystem",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateUCMValues, "None", null) });

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null) });

        ValidationDictionary.Add("var_Tuertyp_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null) });

        ValidationDictionary.Add("var_Tuertyp_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorTyps, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDoorData, "None", null) });

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

    // Standard validationrules

    private void NotEmpty(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyOr0(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0") || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyOr0WhenAnotherTrue(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = Convert.ToBoolean(ParamterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if ((string.IsNullOrWhiteSpace(value) || value == "0") && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = new string[] { anotherBoolean } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { anotherBoolean } });
        }
    }

    private void MustBeTrueWhenAnotherNotEmtyOr0(string name, string displayname, string? value, string? severity, string? anotherString)
    {
        if (string.IsNullOrWhiteSpace(anotherString))
            return;
        var valueToBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        var stringValue = ParamterDictionary[anotherString].Value;

        if (valueToBool && (string.IsNullOrWhiteSpace(stringValue) || stringValue == "0"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} gesetzt (wahr) ist, darf {anotherString} nicht leer sein oder 0 sein", SetSeverity(severity))
            { DependentParameter = new string[] { anotherString } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { anotherString } });
        }
    }

    private void ListContainsValue(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (!ParamterDictionary[name].dropDownList.Contains(value))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: ungültiger Wert | {value} | ist nicht in der Auswahlliste vorhanden.", SetSeverity(severity)));
        }
    }

    // Spezial validationrules

    private void ValidateJobNumber(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (string.IsNullOrWhiteSpace(odernummerName))
            return;
        var fabriknummer = ParamterDictionary["var_FabrikNummer"].Value;

        if (string.IsNullOrWhiteSpace(fabriknummer))
            return;
        ParamterDictionary["var_FabrikNummer"].ClearErrors("var_FabrikNummer");

        var auftragsnummer = ParamterDictionary[odernummerName].Value;
        var informationAufzug = ParamterDictionary["var_InformationAufzug"].Value;
        var fabriknummerBestand = ParamterDictionary["var_FabriknummerBestand"].Value;

        switch (informationAufzug)
        {
            case "Neuanlage" or "Ersatzanlage":
                if (auftragsnummer != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Neuanlagen und Ersatzanlagen muß die Auftragsnummer und Fabriknummer identisch sein", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            case "Umbau":
                if (string.IsNullOrWhiteSpace(fabriknummerBestand) && auftragsnummer != fabriknummer)
                    return;
                if (fabriknummerBestand != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Umbauten muß die Fabriknummer der alten Anlage beibehalten werden", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            default:
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                return;
        }
    }

    private void ValidateJungblutOSG(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;
        if (value.StartsWith("Jungblut"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{value} bei diesem Geschwindigkeitsbegrenzer wir gegebenfals eine Sicherheitsschaltung in der Steuerung benötigt.\n" +
                                                              $"Halten sie Rücksprache mit ihrem Steuerungshersteller.\n" +
                                                              $"Alternativ einen Geschwindigkeitsbegrenzer mit elektromagentischer Rückstellung verwenden.", SetSeverity(severity)));
        }
    }

    private void ValidateRammingProtections(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;
        if (value == "Rammschutz siehe Beschreibung")
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Rammschutz |{value}| muss das Gewicht über das Kabinenkorrekturgewicht mitgegeben werden!", SetSeverity(severity)));
        }
    }

    private void ValidateTravel(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0"))
            return;
        var foerderhoehe = Math.Round(LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_FH") * 1000);
        var etagenhoehe0 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe0");
        var etagenhoehe1 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe1");
        var etagenhoehe2 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe2");
        var etagenhoehe3 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe3");
        var etagenhoehe4 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe4");
        var etagenhoehe5 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe5");
        var etagenhoehe6 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe6");
        var etagenhoehe7 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe7");
        var etagenhoehe8 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe8");

        if ((etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8) == 0)
            return;

        var etagenhoeheTotal = etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8;

        if (etagenhoeheTotal != foerderhoehe)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Die Förderhöhe ({foerderhoehe} mm) stimmt nicht mit Etagenabständen ({etagenhoeheTotal} mm) überein.", SetSeverity(severity))
            { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
    }

    private void ValidateCarFlooring(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string bodentyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodentyp");
        string bodenProfil = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BoPr");
        string bodenBelag = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelag");
        double bodenBlech = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenblech");
        double bodenBelagHoehe = GetFlooringHeight(bodenBelag);
        double bodenHoehe = -1;

        switch (bodentyp)
        {
            case "standard":
                bodenHoehe = 83;
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "3";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
                break;
            case "verstärkt":
                if (bodenBlech <= 0)
                    SetDefaultReinforcedFloor(name);
                if (string.IsNullOrWhiteSpace(bodenProfil))
                    SetDefaultReinforcedFloor(name);
                if (bodenBlech == 3 && bodenProfil == "80 x 40 x 3")
                    SetDefaultReinforcedFloor(name);
                double bodenProfilHoehe = GetFloorProfilHeight(bodenProfil);
                bodenHoehe = bodenBlech + bodenProfilHoehe;
                break;
            case "standard mit Wanne":
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "5";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
                bodenHoehe = 85;
                break;
            case "sonder":
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                bodenHoehe = -1;
                break;
            case "extern":
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                bodenHoehe = -1;
                break;
            default:
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                break;
        }

        ParamterDictionary["var_Bodenbelagsgewicht"].Value = GetFloorWeight(bodenBelag);
        if (bodenHoehe != -1)
        {
            ParamterDictionary["var_KU"].Value = Convert.ToString(bodenHoehe + bodenBelagHoehe);
        }
        ParamterDictionary["var_Bodenbelagsdicke"].Value = Convert.ToString(bodenBelagHoehe);

        double GetFloorProfilHeight(string bodenProfil)
        {
            if (string.IsNullOrEmpty(bodenProfil))
                return 0;
            var profile = _parametercontext.Set<CarFloorProfile>().FirstOrDefault(x => x.Name == bodenProfil);
            if (profile is null)
                return 0;
            return (double)profile.Height!;
        }

        string GetFloorWeight(string bodenBelag)
        {
            if (string.IsNullOrEmpty(bodenBelag))
                return "0";
            if (string.Equals(bodenBelag, "bauseits lt. Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsgewicht");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsgewicht");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return "0";
            return boden.WeightPerSquareMeter.ToString()!;
        }

        double GetFlooringHeight(string bodenBelag)
        {
            if (string.IsNullOrEmpty(bodenBelag))
                return 0;
            if (string.Equals(bodenBelag, "bauseits lt. Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return 0;
            return (double)boden.Thickness!;
        }

        void SetDefaultReinforcedFloor(string name)
        {
            ParamterDictionary["var_Bodenblech"].DropDownListValue = "3";
            ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 50 x 5";
            if (name == "var_BoPr")
            {
                ParamterDictionary["var_BoPr"].DropDownList.Add("Refresh");
                ParamterDictionary["var_BoPr"].DropDownList.Remove("Refresh");
            }
        }
    }

    private void ValidateCarHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double bodenHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KU");
        double kabinenHoeheInnen = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHLicht");
        double kabinenHoeheAussen = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHA");
        double deckenhoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KD");

        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe == kabinenHoeheAussen)
            return;

        switch (name)
        {
            case "var_KU" or "var_KHLicht" or "var_KD":
                ParamterDictionary["var_KHA"].Value = Convert.ToString(bodenHoehe + kabinenHoeheInnen + deckenhoehe);
                return;
            case "var_KHA":
                ParamterDictionary["var_KD"].Value = Convert.ToString(kabinenHoeheAussen - (bodenHoehe + kabinenHoeheInnen));
                return;
            default:
                return;
        }
    }

    private void ValidateCarEntranceRightSide(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double kabinenBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
        double kabinenTiefe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");
        double linkeSeite;
        double tuerBreite;

        switch (name)
        {
            case "var_L1" or "var_TB" or "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L1");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
                ParamterDictionary["var_R1"].Value = Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite));
                return;
            case "var_L2" or "var_TB_C" or "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L2");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_C");
                double r2 = zugangC ? kabinenBreite - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R2"].Value = Convert.ToString(r2);
                return;
            case "var_L3" or "var_TB_B" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L3");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_B");
                double r3 = zugangB ? kabinenTiefe - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R3"].Value = Convert.ToString(r3);
                return;
            case "var_L4" or "var_TB_D" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L4");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_D");
                double r4 = zugangD ? kabinenTiefe - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R4"].Value = Convert.ToString(r4);
                return;
            default:
                return;
        }
    }

    private void ValidateVariableCarDoors(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Variable_Tuerdaten");
        if (variableTuerdaten)
            return;

        string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp");
        string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TH");
        double tuerGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht");

        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");

        if (zugangB)
        {
            ParamterDictionary["var_Tuertyp_B"].DropDownListValue = tuerTyp;
            ParamterDictionary["var_Tuerbezeichnung_B"].DropDownListValue = tuerBezeichnung;
            ParamterDictionary["var_TB_B"].Value = Convert.ToString(tuerBreite);
            ParamterDictionary["var_TH_B"].Value = Convert.ToString(tuerHoehe);
            ParamterDictionary["var_Tuergewicht_B"].Value = Convert.ToString(tuerGewicht);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuertyp_B"].DropDownListValue))
                ParamterDictionary["var_Tuertyp_B"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuerbezeichnung_B"].DropDownListValue))
                ParamterDictionary["var_Tuerbezeichnung_B"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TB_B"].Value))
            {
                if (ParamterDictionary["var_TB_B"].Value != "0")
                    ParamterDictionary["var_TB_B"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TH_B"].Value))
            {
                if (ParamterDictionary["var_TH_B"].Value != "0")
                    ParamterDictionary["var_TH_B"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuergewicht_B"].Value))
            {
                if (ParamterDictionary["var_Tuergewicht_B"].Value != "0")
                    ParamterDictionary["var_Tuergewicht_B"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TuerEinbauB"].Value))
            {
                if (ParamterDictionary["var_TuerEinbauB"].Value != "0")
                    ParamterDictionary["var_TuerEinbauB"].Value = string.Empty;
            }
        }

        if (zugangC)
        {
            ParamterDictionary["var_Tuertyp_C"].DropDownListValue = tuerTyp;
            ParamterDictionary["var_Tuerbezeichnung_C"].DropDownListValue = tuerBezeichnung;
            ParamterDictionary["var_TB_C"].Value = Convert.ToString(tuerBreite);
            ParamterDictionary["var_TH_C"].Value = Convert.ToString(tuerHoehe);
            ParamterDictionary["var_Tuergewicht_C"].Value = Convert.ToString(tuerGewicht);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuertyp_C"].DropDownListValue))
                ParamterDictionary["var_Tuertyp_C"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuerbezeichnung_C"].DropDownListValue))
                ParamterDictionary["var_Tuerbezeichnung_C"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TB_C"].Value))
            {
                if (ParamterDictionary["var_TB_C"].Value != "0")
                    ParamterDictionary["var_TB_C"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TH_C"].Value))
            {
                if (ParamterDictionary["var_TH_C"].Value != "0")
                    ParamterDictionary["var_TH_C"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuergewicht_C"].Value))
            {
                if (ParamterDictionary["var_Tuergewicht_C"].Value != "0")
                    ParamterDictionary["var_Tuergewicht_C"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TuerEinbauC"].Value))
            {
                if (ParamterDictionary["var_TuerEinbauC"].Value != "0")
                    ParamterDictionary["var_TuerEinbauC"].Value = string.Empty;
            }
        }

        if (zugangD)
        {
            ParamterDictionary["var_Tuertyp_D"].DropDownListValue = tuerTyp;
            ParamterDictionary["var_Tuerbezeichnung_D"].DropDownListValue = tuerBezeichnung;
            ParamterDictionary["var_TB_D"].Value = Convert.ToString(tuerBreite);
            ParamterDictionary["var_TH_D"].Value = Convert.ToString(tuerHoehe);
            ParamterDictionary["var_Tuergewicht_D"].Value = Convert.ToString(tuerGewicht);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuertyp_D"].DropDownListValue))
                ParamterDictionary["var_Tuertyp_D"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuerbezeichnung_D"].DropDownListValue))
                ParamterDictionary["var_Tuerbezeichnung_D"].DropDownListValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TB_D"].Value))
            {
                if (ParamterDictionary["var_TB_D"].Value != "0")
                    ParamterDictionary["var_TB_D"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TH_D"].Value))
            {
                if (ParamterDictionary["var_TH_D"].Value != "0")
                    ParamterDictionary["var_TH_D"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Tuergewicht_D"].Value))
            {
                if (ParamterDictionary["var_Tuergewicht_D"].Value != "0")
                    ParamterDictionary["var_Tuergewicht_D"].Value = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_TuerEinbauD"].Value))
            {
                if (ParamterDictionary["var_TuerEinbauD"].Value != "0")
                    ParamterDictionary["var_TuerEinbauD"].Value = string.Empty;
            }
        }
    }

    private void ValidateCarFrameSelection(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var liftTypes = _parametercontext.Set<LiftType>().ToList();
        var currentLiftType = liftTypes.FirstOrDefault(x => x.Name == value);

        var driveTypeId = currentLiftType is not null ? currentLiftType.DriveTypeId : 1;

        var carframes = _parametercontext.Set<CarFrameType>().ToList();

        var availableCarframes = carframes.Where(x => x.DriveTypeId == driveTypeId).Select(s => s.Name);

        if (availableCarframes is not null)
        {
            ParamterDictionary["var_Bausatz"].DropDownList.Clear();
            foreach (var item in availableCarframes)
            {
                ParamterDictionary["var_Bausatz"].DropDownList.Add(item!);
            }
        }
    }

    private void ValidateReducedProtectionSpaces(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        switch (value)
        {
            case "keine":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "False";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "False";
                break;
            case "Schachtkopf":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "True";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "False";
                break;
            case "Schachtgrube":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "False";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "True";
                break;
            case "Schachtkopf und Schachtgrube":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "True";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "True";
                break;
            case "Vorausgelöstes Anhaltesystem":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "True";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "True";
                break;
            default:
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "False";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "False";
                break;
        }
    }

    private void ValidateSafetyGear(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var safetyGears = _parametercontext.Set<SafetyGearModelType>().ToList();
        var selectedSafetyGear = ParamterDictionary["var_TypFV"].Value;
        IEnumerable<string?> availablseafetyGears = value switch
        {
            "keine" => Enumerable.Empty<string?>(),
            "Sperrfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 1).Select(s => s.Name),
            "Bremsfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 2).Select(s => s.Name),
            _ => safetyGears.Select(s => s.Name),
        };
        if (availablseafetyGears is not null)
        {
            ParamterDictionary["var_TypFV"].DropDownList.Clear();
            foreach (var item in availablseafetyGears)
            {
                ParamterDictionary["var_TypFV"].DropDownList.Add(item!);
            }

            if (!string.IsNullOrWhiteSpace(selectedSafetyGear) && !availablseafetyGears.Contains(selectedSafetyGear))
            {
                ParamterDictionary["var_TypFV"].Value = string.Empty;
                ParamterDictionary["var_TypFV"].DropDownListValue = null;
            }
        }
    }

    private void ValidateSafetyRange(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Q"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_F"].Value) ||
            ParamterDictionary["var_F"].Value == "0" ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_Fuehrungsart"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_FuehrungsschieneFahrkorb"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_TypFV"].Value))
            return;

        var safetygearResult = _calculationsModuleService.GetSafetyGearCalculation(ParamterDictionary);
        if (safetygearResult is not null)
        {
            if (!safetygearResult.RailHeadAllowed)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählter Schienenkopf ist für diese Fangvorrichtung nicht zulässig.", SetSeverity(severity)));
                return;
            }

            var load = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_Q");
            var carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_F");

            if (safetygearResult.MinLoad > load + carWeight)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Fangvorrichtung nicht zulässig Minimalgewicht {safetygearResult.MinLoad} kg | Nutzlast+Fahrkorbgewicht: {load + carWeight} kg unterschritten.", SetSeverity(severity)));
                return;
            }
            if (safetygearResult.MaxLoad < load + carWeight)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Fangvorrichtung nicht zulässig Maximalgewicht {safetygearResult.MaxLoad} kg | Nutzlast+Fahrkorbgewicht: {carWeight + load} kg überschritten.", SetSeverity(severity)));
                return;
            }
        }
    }

    private void ValidateDriveSystemTypes(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var driveSystems = _parametercontext.Set<DriveSystem>().Include(i => i.DriveSystemType)
                                                               .ToList();
        var currentdriveSystem = driveSystems.FirstOrDefault(x => x.Name == value);
        ParamterDictionary["var_Getriebe"].Value = currentdriveSystem is not null ? currentdriveSystem.DriveSystemType!.Name : string.Empty;
        ParamterDictionary["var_Getriebe"].DropDownListValue = ParamterDictionary["var_Getriebe"].Value;
    }

    private void ValidateCarweightWithoutFrame(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        int carFrameWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_Rahmengewicht");

        if (carFrameWeight > 0)
        {
            int carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_F");
            if (carWeight > 0)
            {
                ParamterDictionary["var_KabTueF"].Value = Convert.ToString(carWeight - carFrameWeight);
            }
        }
    }

    private void ValidateCorrectionWeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        try
        {
            if (Math.Abs(Convert.ToInt16(value)) > 10)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname} ist größer +-10 kg überprüfen Sie die Eingabe", SetSeverity(severity)));
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private void ValidateCarArea(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, "0"))
        {
            var load = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");
            var reducedLoad = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q1");
            var area = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_A_Kabine");
            var lift = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Aufzugstyp");
            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == lift);
            var cargotyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
            var driveSystemDB = _parametercontext.Set<LiftType>().Include(i => i.DriveType)
                                                               .ToList()
                                                               .FirstOrDefault(x => x.Name == lift);
            var drivesystem = driveSystemDB is not null ? driveSystemDB.DriveType!.Name! : "";

            if (string.Equals(cargotyp, "Lastenaufzug") && string.Equals(drivesystem, "Hydraulik"))
            {
                var loadTable7 = _calculationsModuleService.GetLoadFromTable(area, "Tabelle7");
                if (reducedLoad < loadTable7)
                    ParamterDictionary["var_Q1"].Value = Convert.ToString(loadTable7);
                if (reducedLoad > load)
                    ParamterDictionary["var_Q1"].Value = Convert.ToString(loadTable7);
            }
            else
            {
                ParamterDictionary["var_Q1"].Value = Convert.ToString(load);
            }

            if (!_calculationsModuleService.ValdidateLiftLoad(load, area, cargotyp, drivesystem))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Nennlast enspricht nicht der EN81:20!", SetSeverity(severity)) { DependentParameter = new string[] { "var_Aufzugstyp", "var_Q", "var_A_Kabine" } });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_Aufzugstyp", "var_Q", "var_A_Kabine" } });
            }
        }
    }

    private void ValidateUCMValues(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Aggregat"].Value)
            || ParamterDictionary["var_Aggregat"].Value != "Ziehl-Abegg"
            || string.IsNullOrWhiteSpace(ParamterDictionary["var_Steuerungstyp"].Value))
        {
            ParamterDictionary["var_Erkennungsweg"].Value = "0";
            ParamterDictionary["var_Totzeit"].Value = "0";
            ParamterDictionary["var_Vdetektor"].Value = "0";

            if (name == "var_Steuerungstyp" && ParamterDictionary["var_Aggregat"].Value == "Ziehl-Abegg" && string.IsNullOrWhiteSpace(ParamterDictionary["var_Steuerungstyp"].Value))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"UCM-Daten können nicht berechnet werden es ist kein Steuerung gewählt ist!", SetSeverity("Warning")));
            }
            return;
        }

        var currentLiftControlManufacturers = _parametercontext.Set<LiftControlManufacturer>().FirstOrDefault(x => x.Name == ParamterDictionary["var_Steuerungstyp"].Value);

        if (ParamterDictionary["var_Schachtinformationssystem"].Value == "Limax 33CP"
            || ParamterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S1-Box"
            || ParamterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S2 (FST-3)")
        {
            ParamterDictionary["var_Erkennungsweg"].Value = Convert.ToString(currentLiftControlManufacturers?.DetectionDistanceSIL3);
            ParamterDictionary["var_Totzeit"].Value = Convert.ToString(currentLiftControlManufacturers?.DeadTimeSIL3);
            ParamterDictionary["var_Vdetektor"].Value = Convert.ToString(currentLiftControlManufacturers?.SpeeddetectorSIL3);
        }
        else
        {
            var oldTotzeit = ParamterDictionary["var_Totzeit"].Value;
            var oldVdetektor = ParamterDictionary["var_Vdetektor"].Value;
            var newTotzeit = Convert.ToBoolean(ParamterDictionary["var_ElektrBremsenansteuerung"].Value) ?
                Convert.ToString(currentLiftControlManufacturers?.DeadTimeZAsbc4) :
                Convert.ToString(currentLiftControlManufacturers?.DeadTime);
            var newVdetektor = Convert.ToString(currentLiftControlManufacturers?.Speeddetector);

            if (oldTotzeit == newTotzeit && oldVdetektor == newVdetektor)
            {
                return;
            }

            ParamterDictionary["var_Erkennungsweg"].Value = Convert.ToString(currentLiftControlManufacturers?.DetectionDistance);
            ParamterDictionary["var_Totzeit"].Value = newTotzeit;
            ParamterDictionary["var_Vdetektor"].Value = newVdetektor;
        }
    }

    private void ValidateZAliftData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) || FullPathXml == pathDefaultAutoDeskTransfer)
            return;

        var zaHtmlPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");
        if (!File.Exists(zaHtmlPath))
            return;

        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Aggregat"].Value))
        {
            ParamterDictionary["var_Aggregat"].Value = "Ziehl-Abegg";
            ParamterDictionary["var_Aggregat"].DropDownListValue = "Ziehl-Abegg";
        }

        var lastWriteTime = File.GetLastWriteTime(zaHtmlPath);

        if (lastWriteTime != ZaHtmlCreationTime)
        {
            var zaliftHtml = new HtmlDocument();
            zaliftHtml.Load(zaHtmlPath);
            var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
                                                                            .InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (zliData is null)
                return;

            ZliDataDictionary.Clear();

            foreach (var zlipar in zliData)
            {

                if (!string.IsNullOrWhiteSpace(zlipar) && zlipar.Contains('='))
                {
                    var zliPairValue = zlipar.Split('=');

                    if (!ZliDataDictionary.ContainsKey(zliPairValue[0]))
                    {
                        ZliDataDictionary.Add(zliPairValue[0], zliPairValue[1]);
                    }
                }
            }

            var htmlNodes = zaliftHtml.DocumentNode.SelectNodes("//tr");

            if (htmlNodes is not null)
            {
                ZliDataDictionary.Add("ElektrBremsenansteuerung", htmlNodes.Any(x => x.InnerText.StartsWith("Bremsansteuermodul")).ToString());
            }
            else
            {
                ZliDataDictionary.Add("ElektrBremsenansteuerung", "False");
            }

            var detectionDistanceMeter = htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("Erkennungsweg"))?.ChildNodes[1].InnerText;
            if (!string.IsNullOrWhiteSpace(detectionDistanceMeter))
            {
                ZliDataDictionary.Add("DetectionDistance", (Convert.ToDouble(detectionDistanceMeter.Replace("m", "").Trim(), CultureInfo.CurrentCulture) * 1000).ToString());
            }

            var deadTime = htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("Totzeit"))?.ChildNodes[1].InnerText.Replace("ms", "").Trim();
            if (!string.IsNullOrWhiteSpace(deadTime))
            {
                ZliDataDictionary.Add("DeadTime", deadTime);
            }

            var vDetector = Convert.ToDouble(htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("V Detektor"))?.ChildNodes[1].InnerText.Replace("m/s", "").Trim(), CultureInfo.CurrentCulture).ToString();
            if (!string.IsNullOrWhiteSpace(vDetector))
            {
                ZliDataDictionary.Add("VDetector", vDetector);
            }

            ZaHtmlCreationTime = lastWriteTime;
        }

        var zaLiftValue = string.Empty;
        var zaLiftValue2 = string.Empty;
        var brakerelease = string.Empty;

        var searchString = name switch
        {
            "var_Q" => "Nennlast_Q",
            "var_F" => "Fahrkorbgewicht_F",
            "var_FH" => "Anlage-FH",
            "var_Fremdbelueftung" => "Motor-Fan",
            "var_ElektrBremsenansteuerung" => "ElektrBremsenansteuerung",
            "var_Treibscheibegehaertet" => "Treibscheibe-RF",
            "var_Handlueftung" => "Bremse-Handlueftung",
            "var_Erkennungsweg" => "DetectionDistance",
            "var_Totzeit" => "DeadTime",
            "var_Vdetektor" => "VDetector",
            _ => string.Empty,
        };

        ZliDataDictionary.TryGetValue(searchString, out zaLiftValue);

        if (string.IsNullOrWhiteSpace(zaLiftValue))
            return;

        if (name == "var_Handlueftung")
        {
            ZliDataDictionary.TryGetValue("Bremse-Lueftueberwachung", out zaLiftValue2);
            if (string.IsNullOrWhiteSpace(zaLiftValue2))
                return;
            if (zaLiftValue == "ohne Handlueftung" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. ohne Handl. Mikrosch.";
            if (zaLiftValue == "ohne Handlueftung" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. ohne Hand. Indukt. NS";
            if (zaLiftValue == "mit Handlueftung" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. mit Handl. Mikrosch.";
            if (zaLiftValue == "mit Handlueftung" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. mit Handl. induktiver NS";
            if (zaLiftValue == "fuer Bowdenzug" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Mikrosch.";
            if (zaLiftValue == "fuer Bowdenzug" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Indukt. NS";
        }

        var isValid = name switch
        {
            "var_Q" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_F" => Math.Abs(Convert.ToInt32(value) - Convert.ToInt32(zaLiftValue)) <= 10,
            "var_FH" => Math.Abs(Convert.ToDouble(value) * 1000 - Convert.ToDouble(zaLiftValue) * 1000) <= 20,
            "var_Fremdbelueftung" => string.Equals(value, Convert.ToString(!zaLiftValue.StartsWith("ohne")), StringComparison.CurrentCultureIgnoreCase),
            "var_ElektrBremsenansteuerung" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Treibscheibegehaertet" => string.Equals(value, Convert.ToString(zaLiftValue.Contains("gehaertet")), StringComparison.CurrentCultureIgnoreCase),
            "var_Handlueftung" => string.Equals(value, brakerelease, StringComparison.CurrentCultureIgnoreCase),
            "var_Erkennungsweg" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Totzeit" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Vdetektor" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            _ => true,
        };
        ;

        if (!isValid)
        {
            if (name != "var_Handlueftung")
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Unterschiedliche Werte für >{displayname}<  Wert Spezifikation {value} | Wert ZALiftauslegung {zaLiftValue}", SetSeverity(severity)));
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Unterschiedliche Werte für >{displayname}<  Wert Spezifikation {value} | Wert ZALiftauslegung {zaLiftValue} - {zaLiftValue2} ", SetSeverity(severity)));
            }

        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
    }

    private void ValidateShaftWalls(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;

        if (string.Equals(value, "Holz"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Achtung: Holzschacht - LBO und Türzulassung beachten!\n" +
                $"Schienenbügelbefestigung muß durch bauseitigem Statiker erfolgen!", SetSeverity(severity)));
        }
    }

    private void ValidateGuideModel(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        if (!name.StartsWith("var_Fuehrungsart"))
            return;

        var guideModels = _parametercontext.Set<GuideModelType>().ToList();
        var guideTyp = name.Replace("Fuehrungsart", "TypFuehrung");
        var selectedguideModel = ParamterDictionary[guideTyp].Value;

        IEnumerable<string?> availableguideModels = value switch
        {
            "Gleitführung" => guideModels.Where(x => x.GuideTypeId == 1).Select(s => s.Name),
            "Rollenführung" => guideModels.Where(x => x.GuideTypeId == 2).Select(s => s.Name),
            _ => guideModels.Select(s => s.Name),
        };

        if (availableguideModels is not null)
        {
            ParamterDictionary[guideTyp].DropDownList.Clear();
            foreach (var item in availableguideModels)
            {
                ParamterDictionary[guideTyp].DropDownList.Add(item!);
            }

            if (!string.IsNullOrWhiteSpace(selectedguideModel) && !availableguideModels.Contains(selectedguideModel))
            {
                ParamterDictionary[guideTyp].Value = string.Empty;
                ParamterDictionary[guideTyp].DropDownListValue = null;
            }
        }
    }

    private void ValidatePitLadder(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "False";
            return;
        }
        if (value == "Schachtgrubenleiter EN81:20 mit el. Kontakt")
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "True";
        }
        else
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "False";
        }
    }

    private void ValidateDoorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuertyp"))
            return;

        var liftDoorGroups = name.Replace("var_Tuertyp", "var_Tuerbezeichnung");

        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary[liftDoorGroups].Value = string.Empty;
            ParamterDictionary[liftDoorGroups].DropDownListValue = null;
            ParamterDictionary[liftDoorGroups].DropDownList.Clear();
        }
        else
        {
            var selectedDoorSytem = ParamterDictionary[name].Value![..1];

            var availableLiftDoorGroups = _parametercontext.Set<LiftDoorGroup>().Where(x => x.DoorManufacturer!.StartsWith(selectedDoorSytem)).ToList();

            if (availableLiftDoorGroups is not null)
            {
                ParamterDictionary[liftDoorGroups].DropDownList.Clear();
                foreach (var item in availableLiftDoorGroups)
                {
                    ParamterDictionary[liftDoorGroups].DropDownList.Add(item.Name);
                }
            }
        }
        _ = ParamterDictionary[liftDoorGroups].ValidateParameterAsync().Result;
    }

    private void ValidateDoorData(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuerbezeichnung"))
            return;

        var liftDoortyp = name.Replace("var_Tuerbezeichnung", "var_Tuertyp");

        var doorOpeningDirection = name.Replace("var_Tuerbezeichnung", "var_Tueroeffnung");
        var doorPanelCount = name.Replace("var_Tuerbezeichnung", "var_AnzahlTuerfluegel");

        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary[doorOpeningDirection].Value = string.Empty;
            ParamterDictionary[doorOpeningDirection].DropDownListValue = null;
            ParamterDictionary[doorPanelCount].Value = "0";
        }
        else
        {
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.ShaftDoor)
                                                                      .ThenInclude(t => t!.LiftDoorOpeningDirection)
                                                                      .FirstOrDefault(x => x.Name == value);
            if (liftDoorGroup is not null && liftDoorGroup.ShaftDoor is not null)
            {
                if (liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection is not null)
                {
                    ParamterDictionary[doorOpeningDirection].Value = liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name;
                    ParamterDictionary[doorOpeningDirection].DropDownListValue = liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name;
                }
                ParamterDictionary[doorPanelCount].Value = Convert.ToString(liftDoorGroup.ShaftDoor.DoorPanelCount);
            }
        }
        if (ParamterDictionary[liftDoortyp].HasErrors)
            ParamterDictionary[liftDoortyp].ClearErrors("Value");
    }

    private void ValidateCarEquipmentPosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, name))
            return;

        var zugang = name.Last();
        var hasSpiegel = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Spiegel{zugang}");
        var hasHandlauf = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Handlauf{zugang}");
        var hasSockelleiste = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Sockelleiste{zugang}");
        var hasRammschutz = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Rammschutz{zugang}");
        var hasPaneel = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_PaneelPos{zugang}");

        if (hasSpiegel || hasHandlauf || hasSockelleiste || hasRammschutz || hasPaneel)
        {
            var errorMessage = $"Bei Zugang {zugang} wurde folgende Ausstattung gewählt:";
            if (hasSpiegel)
                errorMessage += " Spiegel,";
            if (hasHandlauf)
                errorMessage += " Handlauf,";
            if (hasSockelleiste)
                errorMessage += " Sockelleiste,";
            if (hasRammschutz)
                errorMessage += " Rammschutz,";
            if (hasPaneel)
                errorMessage += " Paneel,";
            errorMessage += " dies erfordert eine Plausibilitätsprüfung!";

            ValidationResult.Add(new ParameterStateInfo(name, displayname, errorMessage, SetSeverity(severity)));
        }
    }
}