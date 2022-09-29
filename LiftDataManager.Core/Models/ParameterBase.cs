using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace LiftDataManager.Core.Models;
public partial class ParameterBase : ObservableRecipient, INotifyDataErrorInfo
{
    public enum ParameterTypValue
    {
        Text,
        NumberOnly,
        Date,
        Boolean,
        DropDownList
    }

    public enum ParameterCategoryValue
    {
        AllgemeineDaten,
        Schacht,
        Bausatz,
        Fahrkorb,
        Tueren,
        AntriebSteuerungNotruf,
        Signalisation,
        Wartung,
        MontageTUEV,
        RWA,
        Sonstiges,
        KommentareVault,
        CFP
    }

    public enum TypeCodeValue
    {
        String,
        Boolean,
        Date,
        N,
        mm,
        m,
        kg,
        oE,
        mps
    }

    public ParameterTypValue ParameterTyp { get; set; }
    public ParameterCategoryValue ParameterCategory { get; set; }
    public readonly Dictionary<string, List<ParameterStateInfo>> parameterErrors = new();
    public char Symbol => (char)SymbolCode;
    public int SymbolCode { get; set; }

    public ParameterBase()
    {
    }

    [ObservableProperty]
    private bool hasErrors;

    public IEnumerable GetErrors(string? propertyName)
    {
        var errors = new List<ParameterStateInfo>();
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            foreach (var errorList in parameterErrors.Values)
            {
                errors.AddRange(errorList);
            }
            return  errors;
        }
        else
        {
            if (parameterErrors.ContainsKey(propertyName)) errors.AddRange(parameterErrors[propertyName]);
            return errors;
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    protected void AddError(string propertyName, ParameterStateInfo errorMessage)
    {
        if (!parameterErrors.ContainsKey(propertyName))
        {
            parameterErrors.Add(propertyName, new List<ParameterStateInfo>());
        }
        parameterErrors[propertyName].Add(errorMessage);
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
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected int GetSymbolCode(string TypeCode)
    {
        switch (TypeCode.ToLower(new CultureInfo("de-DE", false)))
        {
            case "mm":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 60220;
            case "string":
                ParameterTyp = ParameterTypValue.Text;
                return 59602;
            case "kg":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 59394;
            case "oe":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 60032;
            case "boolean":
                ParameterTyp = ParameterTypValue.Boolean;
                return 62250;
            case "mps":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 60490;
            case "m":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 60614;
            case "n":
                ParameterTyp = ParameterTypValue.NumberOnly;
                return 59394;
            case "date":
                ParameterTyp = ParameterTypValue.Date;
                return 57699;
            default:
                return 59412;
        }
    }
}
