using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Services;

namespace LiftDataManager.Core.Models;

public partial class Parameter : ObservableRecipient
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

    private readonly IAuswahlParameterDataService _auswahlParameterDataService;
    private readonly bool dataImport;
    public List<string> DropDownList { get; } = new();
    public ParameterTypValue ParameterTyp {get; set; }
    public ParameterCategoryValue ParameterCategory{get; set;}
    public char Symbol => (char)SymbolCode;
    public int SymbolCode {get; set;}
    public bool DefaultUserEditable {get; set;}

    public Parameter(string name, string typeCode, string value)
    {
        dataImport = true;
        AuswahlParameterDataService auswahlParameterDataService = new();
        _auswahlParameterDataService = auswahlParameterDataService;
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
            ParameterTypValue.Boolean => value.ToLower(),
            ParameterTypValue.DropDownList => value,
            _ => value,
        };
        dataImport = false;
    }

    public string Name{get; set; }
    public string TypeCode {get ; set;}

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

    private int GetSymbolCode(string TypeCode)
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