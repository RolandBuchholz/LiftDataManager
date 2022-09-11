using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;

namespace LiftDataManager.Core.Services;
public class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; } = new();
    private Dictionary<string, List<Tuple<Delegate, string, string>>> ValidationDictionary { get; set; } = new();
    private List<ParameterStateInfo> ValidationResult { get; set; }
    private CurrentSpeziProperties CurrentSpeziProperties { get; set; } = new();

    public ValidationParameterDataService()
    {
        IsActive = true;
        GetValidationDictionary();
    }

    ~ValidationParameterDataService()
    {
        IsActive = false;
    }

    void IRecipient<SpeziPropertiesRequestMessage>.Receive(SpeziPropertiesRequestMessage message)
    {
        if (message is not null)
        {
            ParamterDictionary = message.Response.ParamterDictionary;
        }
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

    private void GetValidationDictionary()
    {
        ValidationDictionary.Add("var_AuftragsNummer", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_FabrikNummer", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null),
                                                                                                 new Tuple<Delegate, string, string>(ValidateJobNumber, "Warning", "var_AuftragsNummer") });
        ValidationDictionary.Add("var_Q", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", null) });
        ValidationDictionary.Add("var_Kennwort", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Betreiber", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_Projekt", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", null) });
        ValidationDictionary.Add("var_InformationAufzug", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Warning", "(keine Auswahl)") });
        ValidationDictionary.Add("var_Aufzugstyp", new List<Tuple<Delegate, string, string>> { new Tuple<Delegate, string, string>(NotEmpty, "Error", "(keine Auswahl)") });
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
        var test = ParamterDictionary["var_Betreiber"].Value;

        //if (string.IsNullOrWhiteSpace(value))
        //{
        //    ValidationResult.Add(new ParameterStateInfo(name, $"{name} darf nicht leer sein", SetSeverity(severity)));
        //}
    }
}
