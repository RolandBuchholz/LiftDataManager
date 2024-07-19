using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.Contracts.Services;
using MvvmHelpers;

namespace LiftDataManager.Core.Models;

public partial class Parameter : ParameterBase
{
    private readonly IValidationParameterDataService _validationParameterDataService;
    private bool _autoUpdatedRunning;
    public bool DataImport { get; set; }
    public bool DefaultUserEditable { get; set; }
    public bool IsAutoUpdated { get; private set; }

    public Parameter(string value, int parameterTypeCodeId, int parameterTypId, string comment, IValidationParameterDataService validationParameterDataService)
    {
        _validationParameterDataService = validationParameterDataService;
        DropDownList ??= [];
        DataImport = true;
        TypeCode = (TypeCodeValue)parameterTypeCodeId;
        ParameterTyp = (ParameterTypValue)parameterTypId;
        SymbolCode = GetSymbolCode(TypeCode);
        Comment = comment;
        Value = ParameterTyp switch
        {
            ParameterTypValue.Text => (value is not null) ? value : "",
            ParameterTypValue.NumberOnly => (value is not null) ? value : "",
            ParameterTypValue.Date => value,
            ParameterTypValue.Boolean => (value is not null) ? LiftParameterHelper.FirstCharToUpperAsSpan(value) : "False",
            ParameterTypValue.DropDownList => value,
            _ => value,
        };
        DataImport = false;
    }

    public required string Name { get; set; }
    public required string DisplayName { get; set; }

    [ObservableProperty]
    public ObservableRangeCollection<SelectionValue> dropDownList;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private string? comment;
    partial void OnCommentChanged(string? oldValue, string? newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            Broadcast(oldValue, newValue, Name);
        }
    }

    [ObservableProperty]
    private bool isKey;
    partial void OnIsKeyChanged(bool oldValue, bool newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            Broadcast(oldValue, newValue, nameof(IsKey));
        }
    }

    [ObservableProperty]
    private string? value;
    partial void OnValueChanged(string? oldValue, string? newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            IsAutoUpdated = _autoUpdatedRunning;
            var result = ValidateParameterAsync().Result;
            foreach (var item in result.ToList())
            {
                if (item.HasDependentParameters)
                {
                    _ = AfterValidateRangeParameterAsync(item.DependentParameter);
                }
            }
            Broadcast(oldValue, newValue, Name);
        }
    }

    [ObservableProperty]
    private SelectionValue? dropDownListValue;
    partial void OnDropDownListValueChanged(SelectionValue? oldValue, SelectionValue? newValue)
    {
        dropDownListValue = (newValue is null || newValue.Id != 0) ? newValue : null;
        if (newValue is null && oldValue is not null)
        {
            dropDownListValue = oldValue;
        }
        Value = dropDownListValue is not null ? dropDownListValue?.Name : string.Empty;
    }

    public async Task<List<ParameterStateInfo>> ValidateParameterAsync()
    {
        ClearErrors(nameof(Value));
        var result = await _validationParameterDataService.ValidateParameterAsync(Name!, DisplayName!, Value);

        if (result.Any(r => r.IsValid == false))
        {
            foreach (var parameterState in result)
            {
                if (!parameterState.IsValid)
                    AddError(nameof(Value), parameterState);
            }
        }
        return result;
    }

    public async Task AfterValidateRangeParameterAsync(string[] dependentParameters)
    {
        await _validationParameterDataService.ValidateRangeOfParameterAsync(dependentParameters);
    }

    public void AutoUpdateParameterValue(string? newParamterValue)
    {
        _autoUpdatedRunning = true;
        if (ParameterTyp == ParameterTypValue.DropDownList)
        {
             DropDownListValue = LiftParameterHelper.GetDropDownListValue(DropDownList, newParamterValue);
        }
        else
        {
            Value = newParamterValue;
        }
        _autoUpdatedRunning = false;
    }
}