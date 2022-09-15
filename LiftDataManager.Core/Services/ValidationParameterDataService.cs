using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using System.Globalization;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    private Dictionary<string, List<Tuple<Action<string,string?,string?,string?>, string?, string?>>> ValidationDictionary { get; set; } = new();
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
        if (message == null) return;
        if (!message.HasReceivedResponse) return;
        if (message.Response is null)return;
        if (message.Response.ParamterDictionary is null) return;
        if (ParamterDictionary.Any()) return;
            ParamterDictionary = message.Response.ParamterDictionary;
    }

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string? name, string? value)
    {
        ValidationResult.Clear();

        if (name is null)
        {
            ValidationResult.Add(new ParameterStateInfo("Parameter not found",true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        if (!ValidationDictionary.ContainsKey(key: name))
        {
            ValidationResult.Add(new ParameterStateInfo(name,true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        foreach (var rule in ValidationDictionary[name])
        {
            rule.Item1.Invoke(name, value, rule.Item2, rule.Item3);
        }

        if (!ValidationResult.Any())
        {
            ValidationResult.Add(new ParameterStateInfo(name,true));
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
        if (range == null) return;
        if (range.Length == 0) return;

        foreach (var par in ParamterDictionary)
        {
            if (range.Any(r =>  string.Equals(r, par.Value.Name)))
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
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauB") });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_C", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauC") });
        ValidationDictionary.Add("var_ZUGANSSTELLEN_D", new List<Tuple<Action<string, string?, string?, string?>, string?, string?>>
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(MustBeTrueWhenAnotherNotEmtyOr0, "Warning", "var_TuerEinbauD") });
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
                            { new Tuple<Action<string, string?, string?, string?>, string?, string?>(NotEmpty, "Warning", null),
                              new Tuple<Action<string, string?, string?, string?>, string?, string?>(ValidateTravel, "Error", null) });
    }

    private static ParameterStateInfo.ErrorLevel SetSeverity(string? severity)
    {
        if (severity is null) return ParameterStateInfo.ErrorLevel.Error;

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

    private void NotEmptyOr0WhenAnotherTrue(string name, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean)) return;
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
        if (string.IsNullOrWhiteSpace(anotherString)) return;
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
        if (string.IsNullOrWhiteSpace(odernummerName)) return;
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
        if (value is null) return;
        if (value.StartsWith("Jungblut"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{value} bei diesem Geschwindigkeitsbegrenzer wir gegebenfals eine Sicherheitsschaltung in der Steuerung benötigt.\n" +
                                                              $"Halten sie Rücksprache mit ihrem Steuerungshersteller.\n" +
                                                              $"Alternativ einen Geschwindigkeitsbegrenzer mit elektromagentischer Rückstellung verwenden.", SetSeverity(severity)));
        }
    }

    private void ValidateTravel(string name, string? value, string? severity, string? optional = null)
    {
        var foerderhoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_FH");
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

        if ((etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8) == foerderhoehe)
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"Die Förderhöhe stimmt nicht mit Etagenabständen überein.", SetSeverity(severity))
            { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, true) { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2","var_Etagenhoehe3","var_Etagenhoehe4","var_Etagenhoehe5","var_Etagenhoehe6","var_Etagenhoehe7","var_Etagenhoehe8" } });
        }
    }
}
