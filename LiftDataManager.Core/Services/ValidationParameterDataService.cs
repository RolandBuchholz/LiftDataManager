using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.Messenger.Messages;
using System.Globalization;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    private Dictionary<string, List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>> ValidationDictionary { get; set; } = new();
    private List<ParameterStateInfo> ValidationResult { get; set; }
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;

    public ValidationParameterDataService(ParameterContext parametercontext, ICalculationsModule calculationsModuleService)
    {
        IsActive = true;
        GetValidationDictionary();
        ParamterDictionary ??= new();
        ValidationResult ??= new();
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
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
        if (ParamterDictionary.Any())
            return;
        ParamterDictionary = message.Response.ParamterDictionary;
    }

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string? name, string? displayname, string? value)
    {
        ValidationResult.Clear();

        if (name is null || displayname is null)
        {
            ValidationResult.Add(new ParameterStateInfo("Parameter", "Parameter not found", true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        if (!ValidationDictionary.ContainsKey(key: name))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
            await Task.CompletedTask;
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
            new(ValidateCarArea, "Error", null)});

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbau") });

        ValidationDictionary.Add("var_ZUGANSSTELLEN_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauB"),
            new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_ZUGANSSTELLEN_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>>{ new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauC"),
             new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_ZUGANSSTELLEN_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauD"),
            new(ValidateCarDoorData, "None", null) });

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
            new(ValidateTravel, "Error", null) });

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
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null),
            new(ValidateCarEntranceRightSide, "None", null)});

        ValidationDictionary.Add("var_TB_B",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_TB_C",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_TB_D",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarEntranceRightSide, "None", null) });

        ValidationDictionary.Add("var_Variable_Tuerdaten",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_TH",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_Tuertyp",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_Tuerbezeichnung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_Tuergewicht",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarDoorData, "None", null) });

        ValidationDictionary.Add("var_Ersatzmassnahmen",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateReducedProtectionSpaces, "None", null) });

        ValidationDictionary.Add("var_Fangvorrichtung",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateSafetyGear, "None", null) });

        ValidationDictionary.Add("var_Aggregat",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateDriveSystemTypes, "None", null) });

        ValidationDictionary.Add("var_A_Kabine",
            new List<Tuple<Action<string, string, string?, string?, string?>, string?, string?>> { new(ValidateCarArea, "Error", null) });
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
        double bodenHoehe = 0;

        switch (bodentyp)
        {
            case "standard":
                bodenHoehe = 83;
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "3";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
                break;
            case "verstärkt":
                double bodenProfilHoehe = GetFloorProfilHeight(bodenProfil);
                bodenHoehe = bodenBlech + bodenProfilHoehe;
                break;
            case "standard mit Wanne":
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "5";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
                bodenHoehe = 85;
                break;
            case "extern":
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                break;
            default:
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                break;
        }

        ParamterDictionary["var_Bodenbelagsgewicht"].Value = GetFloorWeight(bodenBelag);
        ParamterDictionary["var_KU"].Value = Convert.ToString(bodenHoehe + bodenBelagHoehe);
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

    private void ValidateCarDoorData(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Variable_Tuerdaten");
        if (variableTuerdaten)
            return;

        // ToDo TurenModel aus Datenbank abrufen
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

        if (zugangC)
        {
            ParamterDictionary["var_Tuertyp_C"].DropDownListValue = tuerTyp;
            ParamterDictionary["var_Tuerbezeichnung_C"].DropDownListValue = tuerBezeichnung;
            ParamterDictionary["var_TB_C"].Value = Convert.ToString(tuerBreite);
            ParamterDictionary["var_TH_C"].Value = Convert.ToString(tuerHoehe);
            ParamterDictionary["var_Tuergewicht_C"].Value = Convert.ToString(tuerGewicht);
        }

        if (zugangD)
        {
            ParamterDictionary["var_Tuertyp_D"].DropDownListValue = tuerTyp;
            ParamterDictionary["var_Tuerbezeichnung_D"].DropDownListValue = tuerBezeichnung;
            ParamterDictionary["var_TB_D"].Value = Convert.ToString(tuerBreite);
            ParamterDictionary["var_TH_D"].Value = Convert.ToString(tuerHoehe);
            ParamterDictionary["var_Tuergewicht_D"].Value = Convert.ToString(tuerGewicht);
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
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "false";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "false";
                break;
            case "Schachtkopf":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "true";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "false";
                break;
            case "Schachtgrube":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "false";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "true";
                break;
            case "Schachtkopf und Schachtgrube":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "true";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "true";
                break;
            case "Vorausgelöstes Anhaltesystem":
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "true";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "true";
                break;
            default:
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "false";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "false";
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

    private void ValidateDriveSystemTypes(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var driveSystems = _parametercontext.Set<DriveSystem>().Include(i => i.DriveSystemType)
                                                               .ToList();
        var currentdriveSystem = driveSystems.FirstOrDefault(x => x.Name == value);
        ParamterDictionary["var_Getriebe"].Value = currentdriveSystem is not null ? currentdriveSystem.DriveSystemType!.Name : string.Empty;
        ParamterDictionary["var_Getriebe"].DropDownListValue = ParamterDictionary["var_Getriebe"].Value;
    }

    private void ValidateCarArea(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, "0"))
        {
            var load = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");
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
                ParamterDictionary["var_Q1"].Value = Convert.ToString(_calculationsModuleService.GetLoadFromTable(area, "Tabelle6"));
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
}