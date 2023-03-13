using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.Contracts.Services;
using System.Collections.ObjectModel;

namespace LiftDataManager.Core.Models;

public partial class Parameter : ParameterBase
{
    private readonly IValidationParameterDataService _validationParameterDataService;
    public bool DataImport { get; set; }
    public bool DefaultUserEditable {get; set;}
 
    public Parameter(string value,int parameterTypeCodeId,int parameterTypId, IValidationParameterDataService validationParameterDataService)
    {
        _validationParameterDataService = validationParameterDataService;
        DataImport = true;
        TypeCode = (TypeCodeValue)parameterTypeCodeId;
        ParameterTyp = (ParameterTypValue)parameterTypId;
        SymbolCode = GetSymbolCode(TypeCode);

        Value = ParameterTyp switch
        {
            ParameterTypValue.Text => (value is not null) ? value : "",
            ParameterTypValue.NumberOnly => (value is not null) ? value : "",
            ParameterTypValue.Date => value,
            ParameterTypValue.Boolean => (value is not null) ? value.ToLower() : "false",
            ParameterTypValue.DropDownList => value,
            _ => value,
        };

        if (Value is not null && ParameterTyp == ParameterTypValue.DropDownList)
        {
            DropDownListValue = Value;
        }

        DataImport = false;
    }

    public string? Name {get; set;}
    public string? DisplayName { get; set; }
    
    public string? Errors => (GetErrors(null) != null) ? string.Join(Environment.NewLine, GetErrors(null).OfType<ParameterStateInfo>().Select(e => e.ErrorMessage)) : null;

    [ObservableProperty]
    public ObservableCollection<string> dropDownList = new();

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private string? comment;
    partial void OnCommentChanged(string? value)
    {
        if (!DataImport) IsDirty = true;
    }

    [ObservableProperty]
    private bool isKey;
    partial void OnIsKeyChanged(bool value)
    {
        if (!DataImport) IsDirty = true;
    }

    [ObservableProperty]
    private string? value;
    private string? oldTempValue;
    partial void OnValueChanging(string? value)
    {
        oldTempValue = Value;
    }
    partial void OnValueChanged(string? value)
    {
        if (!DataImport)
        {
            var result = ValidateParameterAsync().Result;
            foreach (var item in result.ToList())
            {
                if (item.HasDependentParameters)
                {
                    _ = AfterValidateRangeParameterAsync(item.DependentParameter);
                }
            }
            isDirty = true;
            Broadcast(oldTempValue, value, Name);
        }
    }

    [ObservableProperty]
    private string? dropDownListValue;

    partial void OnDropDownListValueChanged(string? value)
    {
        dropDownListValue = (value != "(keine Auswahl)") ? value : null;
        if (value is null && Value is not null)
        {
            dropDownListValue = Value;
        }
        Value = dropDownListValue;
    }

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync()
    {
        ClearErrors(nameof(Value));
        var result = await _validationParameterDataService.ValidateParameterAsync(Name!,DisplayName!, Value);

        if (result.Any(r => r.IsValid == false))
        {
            foreach (var parameterState in result)
            {
                if(!parameterState.IsValid) AddError(nameof(Value), parameterState);
            }
        }
       return result;
    }

    public async Task AfterValidateRangeParameterAsync(string[] dependentParameters)
    {
        await _validationParameterDataService.ValidateRangeOfParameterAsync(dependentParameters);
    }
}