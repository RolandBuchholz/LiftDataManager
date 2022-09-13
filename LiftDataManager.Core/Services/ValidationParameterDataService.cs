using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    private Dictionary<string, List<Tuple<Delegate, string, string>>> ValidationDictionary { get; set; } = new();
    private List<ParameterStateInfo> ValidationResult { get; set; }

    public ValidationParameterDataService()
    {
        IsActive = true;
        GetValidationDictionary();
        ParamterDictionary ??= new();
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

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync(string name, string value)
    {
        ValidationResult ??= new();
        ValidationResult.Clear();

        if (name is null)
        {
            ValidationResult.Add(new ParameterStateInfo(true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        if (!ValidationDictionary.ContainsKey(key: name))
        {
            ValidationResult.Add(new ParameterStateInfo(true));
            await Task.CompletedTask;
            return ValidationResult;
        }

        foreach (var rule in ValidationDictionary[name])
        {
            rule.Item1.DynamicInvoke(name, value, rule.Item2, rule.Item3);
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

    private void GetValidationDictionary()
    {
        ValidationDictionary.Add("var_AuftragsNummer", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_FabrikNummer", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null),
                                                                                                 new Tuple<Delegate, string, string>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_Q", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_Kennwort", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Betreiber", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Projekt", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_InformationAufzug", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", "(keine Auswahl)"),
                                                                                                      new Tuple<Delegate, string, string>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_Aufzugstyp", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", "(keine Auswahl)") });
        ValidationDictionary.Add("var_FabriknummerBestand", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
    }

    private static ParameterStateInfo.ErrorLevel SetSeverity(string severity)
    {
        return severity switch
        {
            "Error" => ParameterStateInfo.ErrorLevel.Error,
            "Warning" => ParameterStateInfo.ErrorLevel.Warning,
            "Informational" => ParameterStateInfo.ErrorLevel.Informational,
            _ => ParameterStateInfo.ErrorLevel.Error,
        };
    }

    private void NotEmpty(string name, string value, string severity, string optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void ValidateJobNumber(string name, string value, string severity, string odernummerName)
    {
        var fabriknummer = ParamterDictionary["var_FabrikNummer"].Value;

        if (string.IsNullOrWhiteSpace(fabriknummer)) return;
        ParamterDictionary["var_FabrikNummer"].ClearErrors("var_FabrikNummer");

        var auftragsnummer = ParamterDictionary[odernummerName].Value;
        var informationAufzug = ParamterDictionary["var_InformationAufzug"].Value;
        var fabriknummerBestand = ParamterDictionary["var_FabriknummerBestand"].Value;

        switch (informationAufzug)
        {
            case "Neuanlage" or "Ersatzanlage":
                if (auftragsnummer != fabriknummer)
                    ValidationResult.Add(new ParameterStateInfo("var_FabrikNummer", $"Bei Neuanlagen und Ersatzanlagen muß die Auftragsnummer und Fabriknummer identisch sein", SetSeverity(severity)));
                return;
            case "Umbau":
                if (string.IsNullOrWhiteSpace(fabriknummerBestand) && auftragsnummer != fabriknummer) return;
                if (fabriknummerBestand != fabriknummer)
                    ValidationResult.Add(new ParameterStateInfo("var_FabrikNummer", $"Bei Umbauten muß die Fabriknummer der alten Anlage beibehalten werden", SetSeverity(severity)));
                return;
            default:
                return;
        }
    }
}
