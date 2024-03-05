using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;

namespace LiftDataManager.Core.Models;
public partial class ParameterBase : ObservableRecipient, INotifyDataErrorInfo
{
    public ParameterTypValue ParameterTyp { get; set; }
    public ParameterCategoryValue ParameterCategory { get; set; }
    public TypeCodeValue TypeCode { get; set; }
    public readonly Dictionary<string, List<ParameterStateInfo>> parameterErrors = new();
    public char Symbol => (char)SymbolCode;
    public int SymbolCode { get; set; }

    public ParameterBase()
    {
        ParameterCategory ??= ParameterCategoryValue.None;
    }

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private ErrorLevel parameterState;

    [ObservableProperty]
    private string? validationErrors;

    public IEnumerable GetErrors(string? propertyName)
    {
        var errors = new List<ParameterStateInfo>();
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            foreach (var errorList in parameterErrors.Values)
            {
                errors.AddRange(errorList);
            }
            return errors;
        }
        else
        {
            if (parameterErrors.ContainsKey(propertyName))
                errors.AddRange(parameterErrors[propertyName]);
            return errors;
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public void AddError(string propertyName, ParameterStateInfo errorMessage)
    {
        if (!parameterErrors.TryGetValue(propertyName, out List<ParameterStateInfo>? value))
        {
            value = new List<ParameterStateInfo>();
            parameterErrors.Add(propertyName, value);
        }

        if (value.Count == 0 || !value.Exists(x => x.ErrorMessage == errorMessage.ErrorMessage))
        {
            value.Add(errorMessage);
        }

        OnErrorsChanged(propertyName);
    }

    public void RemoveError(string propertyName, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            return;
        if (parameterErrors.TryGetValue(propertyName, out List<ParameterStateInfo>? value))
        {
            var error = value.FirstOrDefault(x => x.ErrorMessage == errorMessage);
            if (error is not null)
                value.Remove(error);
            if (value.Count == 0)
                parameterErrors.Remove(propertyName);
        }
        OnErrorsChanged(propertyName);
    }

    public void ClearErrors(string propertyName)
    {
        if (parameterErrors.Remove(propertyName))
        {
            OnErrorsChanged(propertyName);
        }
    }

    protected void OnErrorsChanged(string propertyName)
    {
        HasErrors = parameterErrors.Any();
        if (HasErrors)
        {
            SetParameterState(propertyName);
        }
        else
        {
            ParameterState = ErrorLevel.Valid;
        }

        ValidationErrors = (GetErrors(null) != null) ? string.Join(Environment.NewLine, GetErrors(null).OfType<ParameterStateInfo>().Select(e => e.ErrorMessage)) : null;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void SetParameterState(string propertyName)
    {
        if (!string.Equals(propertyName, "Value"))
            return;
        if (parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? valueErrorList))
        {
            if (valueErrorList is null)
                return;
            if (!valueErrorList.Any())
                return;
            var state = valueErrorList.OrderByDescending(p => p.Severity).FirstOrDefault();

            if (state is not null)
            {
                ParameterState = state.Severity;
            }
        }
    }

    protected static int GetSymbolCode(TypeCodeValue TypeCode)
    {
        return TypeCode switch
        {
            TypeCodeValue.mm => 60220,
            TypeCodeValue.String => 59602,
            TypeCodeValue.kg => 59394,
            TypeCodeValue.oE => 60032,
            TypeCodeValue.Boolean => 62250,
            TypeCodeValue.mps => 60490,
            TypeCodeValue.m => 60614,
            TypeCodeValue.N => 59394,
            TypeCodeValue.Date => 57699,
            _ => 59412,
        };
    }
}