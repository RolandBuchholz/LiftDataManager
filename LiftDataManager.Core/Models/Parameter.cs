using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.Contracts.Services;
using System.Collections.Immutable;
using System.Diagnostics;

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
    public partial ObservableRangeCollection<SelectionValue> DropDownList { get; set; }

    [ObservableProperty]
    public partial bool IsDirty { get; set; }

    [ObservableProperty]
    public partial string? Comment { get; set; }
    partial void OnCommentChanged(string? oldValue, string? newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            Broadcast(oldValue, newValue, Name);
        }
    }

    [ObservableProperty]
    public partial bool IsKey { get; set; }
    partial void OnIsKeyChanged(bool oldValue, bool newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            Broadcast(oldValue, newValue, nameof(IsKey));
        }
    }

    [ObservableProperty]
    public partial string? Value { get; set; }
    partial void OnValueChanged(string? oldValue, string? newValue)
    {
        if (!DataImport)
        {
            IsDirty = true;
            IsAutoUpdated = _autoUpdatedRunning;
            var result = ValidateParameterAsync();
            if (!result.IsFaulted)
            {
                foreach (var item in result.Result.ToImmutableArray())

                {
                    if (item.HasDependentParameters)
                    {
                        _ = AfterValidateRangeParameterAsync(item.DependentParameter);
                    }
                }
            }
            else
            {
                Debug.WriteLine($"{Name}:Validation Failed");
                if (result.Exception is not null)
                {
                    foreach (var ex in result.Exception.InnerExceptions)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            Broadcast(oldValue, newValue, Name);
        }
    }

    public SelectionValue? DropDownListValue
    {
        get => field;
        set
        {
            if (value is null && field is not null)
            {
                return;
            }
            if (!EqualityComparer<SelectionValue?>.Default.Equals(field, value))
            {
                OnPropertyChanging(nameof(DropDownListValue));
                field = value?.Id != 0 ? value : null;
                OnPropertyChanged(nameof(DropDownListValue));
                Value = field?.Id != 0 ? field?.Name : string.Empty;
            }
        }
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
                {
                    AddError(nameof(Value), parameterState);
                }
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

    public void RefreshDropDownListValue()
    {
        DropDownListValue = LiftParameterHelper.GetDropDownListValue(DropDownList, Value);
    }
}