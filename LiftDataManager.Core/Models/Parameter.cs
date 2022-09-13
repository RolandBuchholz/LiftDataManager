using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.Contracts.Services;

namespace LiftDataManager.Core.Models;

public partial class Parameter : ParameterBase
{
    private readonly IAuswahlParameterDataService _auswahlParameterDataService;
    private readonly IValidationParameterDataService _validationParameterDataService;
    public List<string> DropDownList { get; } = new();
    private readonly bool dataImport;
    public bool DefaultUserEditable{get; set;}

    public Parameter(string name, string typeCode, string value, IAuswahlParameterDataService auswahlParameterDataService, IValidationParameterDataService validationParameterDataService) 
    {
        dataImport = true;
        _auswahlParameterDataService = auswahlParameterDataService;
        _validationParameterDataService = validationParameterDataService;
        IsDirty = false;
        Name = name;
        TypeCode = typeCode;
        SymbolCode = GetSymbolCode(TypeCode);
        if (_auswahlParameterDataService.ParameterHasAuswahlliste(name))
        {
            DropDownList = _auswahlParameterDataService.GetListeAuswahlparameter(name);
            DropDownListValue = value;
            ParameterTyp = ParameterTypValue.DropDownList;
        }

        Value = ParameterTyp switch
        {
            ParameterTypValue.Text => (value is not null) ? value : "",
            ParameterTypValue.NumberOnly => (value is not null) ? value : "",
            ParameterTypValue.Date => value,
            ParameterTypValue.Boolean => (value is not null) ? value.ToLower() : "false",
            ParameterTypValue.DropDownList => value,
            _ => value,
        };

        dataImport = false;
    }

    public string Name {get; set;}
    public string TypeCode {get; set;}

    public string Errors => (GetErrors(Name) != null) ? string.Join(" ", GetErrors(Name).OfType<ParameterStateInfo>().Select(e => e.ErrorMessage)) : null;
        
    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private string comment;
    partial void OnCommentChanged(string value) => IsDirty = true;

    [ObservableProperty]
    private bool isKey;
    partial void OnIsKeyChanged(bool value) => IsDirty = true;

    [ObservableProperty]
    private string value;
    private string oldTempValue;
    partial void OnValueChanging(string value)
    {
        oldTempValue = Value;
    }
    partial void OnValueChanged(string value)
    {
        if (!dataImport)
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
    private string dropDownListValue;

    partial void OnDropDownListValueChanged(string value)
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
        ClearErrors(Name);
        var result = await _validationParameterDataService.ValidateParameterAsync(Name, Value);

        if (!result.Any(r => r.IsValid))
        {
            foreach (var parameterState in result)
            {
                if(!parameterState.IsValid) AddError(Name, parameterState);
            }
        }
        return result;
    }

    public async Task AfterValidateRangeParameterAsync(string[] dependentParameters)
    {
        await _validationParameterDataService.ValidateRangeOfParameterAsync(dependentParameters);
    }
}