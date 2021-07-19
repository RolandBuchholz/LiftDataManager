using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Messenger.Messages;
using System;

namespace SpeziInspector.Core.Models
{
    public class Parameter : ObservableRecipient

    {
        public Parameter(string _TypeCode, string _Value)
        {
            IsDirty = false;
            TypeCode = _TypeCode;
            Value = _Value;
            SymbolCode = GetSymbolCode(TypeCode);

            if (IsDate)
            {
                if (string.IsNullOrWhiteSpace(_Value))
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

            if (IsBoolean)
            {
                if (string.IsNullOrWhiteSpace(_Value))
                {
                    BoolValue = null;
                }
                else
                {
                    if (_Value.ToLower() == "false") 
                    {
                        BoolValue = false;
                    }
                    else
                    {
                        BoolValue = true;
                    }
                }
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
        public bool IsDate { get; set; }
        public bool IsNumberOnly { get; set; }
        public bool IsString { get; set; }
        public bool IsBoolean { get; set; }
        public char Symbol => (char)SymbolCode;
        public int SymbolCode { get; set; }

        private int GetSymbolCode(string TypeCode)
        {
            switch (TypeCode.ToLower())
            {
                case "mm":
                    IsNumberOnly = true;
                    return 60220;

                case "string":
                    IsString = true;
                    return 59602;

                case "kg":
                    IsNumberOnly = true;
                    return 59394;

                case "oe":
                    IsNumberOnly = true;
                    return 60032;

                case "boolean":
                    IsBoolean = true;
                    return 62250;

                case "mps":
                    IsNumberOnly = true;
                    return 60490;

                case "m":
                    IsNumberOnly = true;
                    return 60614;

                case "n":
                    IsNumberOnly = true;
                    return 59394;

                case "date":
                    IsDate = true;
                    return 57699;

                default:
                    return 59412;
            }
        }
    }
}
