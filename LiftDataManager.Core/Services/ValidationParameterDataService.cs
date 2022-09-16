using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using System.Globalization;
using Windows.System;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    private Dictionary<string, List<Tuple<Action<string, string?, string?, string?>, string?, string?>>> ValidationDictionary { get; set; } = new();
    private List<ParameterStateInfo> ValidationResult { get; set; }

    public ValidationParameterDataService()
    {
        IsActive = true;
        GetValidationDictionary();
        ParamterDictionary ??= new();
        ValidationResult ??= new();
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

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string? name, string? value)
    {
        ValidationResult.Clear();

        if (name is null)
        {
            ValidationResult.Add(new ParameterStateInfo("Parameter not found", true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        if (!ValidationDictionary.ContainsKey(key: name))
        {
            ValidationResult.Add(new ParameterStateInfo(name, true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        foreach (var rule in ValidationDictionary[name])
        {
            rule.Item1.Invoke(name, value, rule.Item2, rule.Item3);
        }

        if (!ValidationResult.Any())
        {
            ValidationResult.Add(new ParameterStateInfo(name, true));
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
        ValidationDictionary.Add("var_AuftragsNummer", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_FabrikNummer", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string ?, string?>(NotEmpty, "Warning", null),
                              new Tuple<Action<string, string?, string?, string?>, string ?, string?>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_Q", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_Kennwort", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Betreiber", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Projekt", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_InformationAufzug", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Warning", "(keine Auswahl)"),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_Aufzugstyp", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Error", "(keine Auswahl)") });
        ValidationDictionary.Add("var_FabriknummerBestand", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_A", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbau") });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_B", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauB"),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_C", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauC"),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_D", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauD"),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_TuerEinbau", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_A") });
        ValidationDictionary.Add("var_TuerEinbauB", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_B") });
        ValidationDictionary.Add("var_TuerEinbauC", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_C") });
        ValidationDictionary.Add("var_TuerEinbauD", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmptyOr0WhenAnotherTrue, "Warning", "var_ZUGANSSTELLEN_D") });
        ValidationDictionary.Add("var_Geschwindigkeitsbegrenzer", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateJungblutOSG, "Informational", null) });
        ValidationDictionary.Add("var_FH", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmptyOr0, "Error", null),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe0", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe1", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe2", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe3", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe4", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe5", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe6", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe7", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Etagenhoehe8", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
        ValidationDictionary.Add("var_Bodentyp", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarFlooring, "None", null) });
        ValidationDictionary.Add("var_Bodenblech", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarFlooring, "None", null) });
        ValidationDictionary.Add("var_BoPr", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarFlooring, "None", null) });
        ValidationDictionary.Add("var_Bodenbelagsdicke", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarFlooring, "None", null) });
        ValidationDictionary.Add("var_Bodenbelag", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarFlooring, "None", null) });
        ValidationDictionary.Add("var_KU", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarHeight, "None", null) });
        ValidationDictionary.Add("var_KHLicht", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarHeight, "None", null) });
        ValidationDictionary.Add("var_KHA", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarHeight, "None", null) });
        ValidationDictionary.Add("var_KD", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarHeight, "None", null) });
        ValidationDictionary.Add("var_KBI", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_KTI", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_L1", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_L2", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_L3", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_L4", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null) });
        ValidationDictionary.Add("var_TB", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null)});
        ValidationDictionary.Add("var_TB_B", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            {new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null)});
        ValidationDictionary.Add("var_TB_C", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            {new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null)});
        ValidationDictionary.Add("var_TB_D", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            {new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarEntranceRightSide, "None", null)});
        ValidationDictionary.Add("var_Variable_Tuerdaten", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_TH", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_Tuertyp", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_Tuerbezeichnung", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
        ValidationDictionary.Add("var_Tuergewicht", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateCarDoorData, "None", null) });
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

    private void NotEmpty(string name, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyOr0(string name, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0") || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyOr0WhenAnotherTrue(string name, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = Convert.ToBoolean(ParamterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if ((string.IsNullOrWhiteSpace(value) || value == "0") && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = new string[] { anotherBoolean } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { anotherBoolean } });
        }
    }

    private void MustBeTrueWhenAnotherNotEmtyOr0(string name, string? value, string? severity, string? anotherString)
    {
        if (string.IsNullOrWhiteSpace(anotherString))
            return;
        var valueToBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        var stringValue = ParamterDictionary[anotherString].Value;

        if (valueToBool && (string.IsNullOrWhiteSpace(stringValue) || stringValue == "0"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{name} gesetzt (wahr) ist, darf {anotherString} nicht leer sein oder 0 sein", SetSeverity(severity))
            { DependentParameter = new string[] { anotherString } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { anotherString } });
        }
    }

    // Spezial validationrules

    private void ValidateJobNumber(string name, string? value, string? severity, string? odernummerName)
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
                    ValidationResult.Add(new ParameterStateInfo(name, $"Bei Neuanlagen und Ersatzanlagen muß die Auftragsnummer und Fabriknummer identisch sein", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            case "Umbau":
                if (string.IsNullOrWhiteSpace(fabriknummerBestand) && auftragsnummer != fabriknummer)
                    return;
                if (fabriknummerBestand != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, $"Bei Umbauten muß die Fabriknummer der alten Anlage beibehalten werden", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            default:
                return;
        }
    }

    private void ValidateJungblutOSG(string name, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;
        if (value.StartsWith("Jungblut"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{value} bei diesem Geschwindigkeitsbegrenzer wir gegebenfals eine Sicherheitsschaltung in der Steuerung benötigt.\n" +
                                                              $"Halten sie Rücksprache mit ihrem Steuerungshersteller.\n" +
                                                              $"Alternativ einen Geschwindigkeitsbegrenzer mit elektromagentischer Rückstellung verwenden.", SetSeverity(severity)));
        }
    }

    private void ValidateTravel(string name, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0"))
            return;
        var foerderhoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_FH") * 1000;
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
            ValidationResult.Add(new ParameterStateInfo(name, $"Die Förderhöhe ({foerderhoehe} mm) stimmt nicht mit Etagenabständen ({etagenhoeheTotal} mm) überein.", SetSeverity(severity))
            { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
    }

    private void ValidateCarFlooring(string name, string? value, string? severity, string? optional = null)
    {
        string bodentyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodentyp");
        string bodenProfil = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BoPr");
        string bodenBelag = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelag");
        double bodenBlech = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenblech");
        double bodenProfilHoehe = GetFloorProfilHeight(bodenProfil);
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
                return;
            default:
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                break;
        }

        ParamterDictionary["var_Bodenbelagsgewicht"].Value = GetFloorWeight(bodenBelag);
        ParamterDictionary["var_KU"].Value = Convert.ToString(bodenHoehe + bodenBelagHoehe);
        ParamterDictionary["var_Bodenbelagsdicke"].Value = Convert.ToString(bodenBelagHoehe);

        // ToDo Parameter aus Datenbank abrufen

        static double GetFloorProfilHeight(string bodenProfil)
        {
            return bodenProfil switch
            {
                "80 x 40 x 3" => 80,
                "80 x 50 x 5" => 80,
                "100 x 50 x 5" => 100,
                "120 x 60 x 6" => 120,
                "140 x 60 x 6" => 140,
                "UNP 140" => 140,
                "UNP 160" => 160,
                "UNP 180" => 180,
                _ => 0,
            };
        }

        string GetFloorWeight(string bodenBelag)
        {
            return bodenBelag switch
            {
                "kein" => "0",
                "4/6 Träneblech grundiert" => "33,4",
                "6/8 Tränenblech grundiert" => "49,1",
                "4/6 Träneblech feuerverzinkt" => "33,4",
                "6/8 Tränenblech feuerverzinkt" => "49,1",
                "4/6 Träneblech Edelstahl" => "33,4",
                "6/8 Tränenblech Edelstahl" => "49,1",
                "3,0 SE-TB1 R11 Edelstahl" => "10,25",
                "3,5/5 Alu Quintett" => "10,34",
                "PVC-Mipolam" => "3",
                "PVC-Mipolam 1010" => "3",
                "PVC-Mipolam 1060" => "3",
                "Mondo" => "3",
                "Linoleum" => "3",
                "Norament 926/354" => "3",
                "Norament 926 grano" => "3",
                "Grama Blend" => "18",
                "bauseits Stein 10 mm" => "25",
                "bauseits Stein 20 mm" => "50",
                "bauseits Stein 25 mm" => "62,5",
                "bauseits Stein 30 mm" => "75",
                "Alu Quintett 3,5/5" => "10,34",
                "bauseits lt. Beschreibung" => LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsgewicht"),
                _ => "0",
            };
        }

        double GetFlooringHeight(string bodenBelag)
        {
            return bodenBelag switch
            {
                "kein" => 0,
                "4/6 Träneblech grundiert" => 6,
                "6/8 Tränenblech grundiert" => 8,
                "4/6 Träneblech feuerverzinkt" => 6,
                "6/8 Tränenblech feuerverzinkt" => 8,
                "4/6 Träneblech Edelstahl" => 6,
                "6/8 Tränenblech Edelstahl" => 8,
                "3,0 SE-TB1 R11 Edelstahl" => 5,
                "3,5/5 Alu Quintett" => 5,
                "PVC-Mipolam" => 3,
                "PVC-Mipolam 1010" => 3,
                "PVC-Mipolam 1060" => 3,
                "Mondo" => 2,
                "Linoleum" => 4,
                "Norament 926/354" => 3,
                "Norament 926 grano" => 3,
                "Grama Blend" => 10,
                "bauseits Stein 10 mm" => 10,
                "bauseits Stein 20 mm" => 20,
                "bauseits Stein 25 mm" => 25,
                "bauseits Stein 30 mm" => 30,
                "Alu Quintett 3,5/5" => 5,
                "bauseits lt. Beschreibung" => LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke"),
                _ => 0,
            };
        }
    }

    private void ValidateCarHeight(string name, string? value, string? severity, string? optional = null)
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

    private void ValidateCarEntranceRightSide(string name, string? value, string? severity, string? optional = null)
    {
        double kabinenBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
        double kabinenTiefe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
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
                ParamterDictionary["var_R2"].Value = Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite));
                return;
            case "var_L3" or "var_TB_B" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L3");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_B");
                ParamterDictionary["var_R3"].Value = Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite));
                return;
            case "var_L4" or "var_TB_D" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L4");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_D");
                ParamterDictionary["var_R4"].Value = Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite));
                return;
            default:
                return;
        }
    }

    private void ValidateCarDoorData(string name, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Variable_Tuerdaten");
        if (variableTuerdaten)
            return;

        // ToDo TurenModel aus Datenbank abrufen
        string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp");
        string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TH");
        double tuerGewicht= LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht");

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
}