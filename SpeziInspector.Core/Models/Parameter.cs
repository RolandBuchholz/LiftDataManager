using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Messenger.Messages;
using SpeziInspector.Core.Services;
using System;
using System.Collections.Generic;

namespace SpeziInspector.Core.Models
{
    public class Parameter : ObservableRecipient
    {
        public enum ParameterTypValue
        {
            String,
            NumberOnly,
            Date,
            Boolean,
            DropDownList
        }
        private readonly IAuswahlParameterDataService _auswahlParameterDataService;
        public List<string> DropDownList { get; } = new();
        public Parameter( string name, string typeCode, string value)
        {
            AuswahlParameterDataService auswahlParameterDataService = new();
            _auswahlParameterDataService = auswahlParameterDataService;
            IsDirty = false;
            TypeCode = typeCode;
            Value = value;
            Name = name;
            SymbolCode = GetSymbolCode(TypeCode);
            if (ParameterTyp == ParameterTypValue.Date)
            {
                if (string.IsNullOrWhiteSpace(Value) || Value == "0")
                {
                    Date = null;
                }
                else
                {
                    try
                    {
                        double excelDate = Convert.ToDouble(_Value);
                        Date = DateTime.FromOADate(excelDate);
                    }
                    catch
                    {
                        Date = null;
                    }
                }
            }

            if (ParameterTyp == ParameterTypValue.Boolean)
            {
                if (string.IsNullOrWhiteSpace(Value))
                {
                    BoolValue = null;
                }
                else
                {
                    if (Value.ToLower() == "false")
                    {
                        BoolValue = false;
                    }
                    else
                    {
                        BoolValue = true;
                    }
                }
            }
      
            if (_auswahlParameterDataService.ParameterHasAuswahlliste(name))
            {
                DropDownList = _auswahlParameterDataService.GetListeAuswahlparameter(name);
                DropDownListValue = Value;
                ParameterTyp = ParameterTypValue.DropDownList;
            }
        }
        public string Name { get; set; }
        public string TypeCode { get; set; }
        private string _Value;
        public string Value
        {
            get => _Value;
            set
            {
                if ((_Value != null && value != _Value) || (_Value is null && value != ""))
                {
                    IsDirty = true;
                }
                SetProperty(ref _Value, value);
            }
        }
        private string _Comment;
        public string Comment
        {
            get => _Comment;
            set
            {
                if ((_Comment != null && value != _Comment) || (_Comment is null && value != ""))
                {
                    IsDirty = true;
                }
                SetProperty(ref _Comment, value);
            }
        }
        private DateTimeOffset? _Date;
        public DateTimeOffset? Date
        {
            get => _Date;
            set
            {
                if ((_Date != null && value != _Date) || (_Date is null && value != null))
                {
                    IsDirty = true;
                }
                SetProperty(ref _Date, value);
                if (_Date != null)
                {
                    _Value = _Date?.ToString("d");
                }
            }
        }
        private bool _IsKey;
        public bool IsKey
        {
            get => _IsKey;
            set
            {
                if (value != _IsKey)
                {
                    IsDirty = true;
                }
                SetProperty(ref _IsKey, value);

            }
        }
        private bool? _BoolValue;
        public bool? BoolValue
        {
            get => _BoolValue;
            set
            {
                if (_BoolValue != null && value != _BoolValue)
                {
                    IsDirty = true;
                    _Value = value.ToString();
                }
                SetProperty(ref _BoolValue, value);
            }
        }

        private string _DropDownListValue;
        public string DropDownListValue
        {
            get => _DropDownListValue;
            set
            {
                if (value != _DropDownListValue)
                {
                    Value = value;
                }
                SetProperty(ref _DropDownListValue, value);
            }
        }


        private bool _IsDirty;
        public bool IsDirty
        {
            get => _IsDirty;
            set
            {
                SetProperty(ref _IsDirty, value);
                Messenger.Send(new ParameterDirtyMessage(value));
            }
        }
        public ParameterTypValue ParameterTyp { get; set; }
        public char Symbol => (char)SymbolCode;
        public int SymbolCode { get; set; }

        private int GetSymbolCode(string TypeCode)
        {
            switch (TypeCode.ToLower())
            {
                case "mm":
                    ParameterTyp = ParameterTypValue.NumberOnly;
                    return 60220;

                case "string":
                    ParameterTyp = ParameterTypValue.String;
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
}
