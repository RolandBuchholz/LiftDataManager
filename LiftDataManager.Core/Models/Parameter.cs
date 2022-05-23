using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Services;
using System.Collections.Generic;

namespace LiftDataManager.Core.Models
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
        private bool dataImport = false;
        private ParameterChangeInfo _ParameterChangeInfo { get; set; } = new();
        public List<string> DropDownList { get; } = new();
        public ParameterTypValue ParameterTyp { get; set; }
        public char Symbol => (char)SymbolCode;
        public int SymbolCode { get; set; }

        public Parameter( string name, string typeCode, string value)
        {
            dataImport = true;
            AuswahlParameterDataService auswahlParameterDataService = new();
            _auswahlParameterDataService = auswahlParameterDataService;
            TypeCode = typeCode;
            SymbolCode = GetSymbolCode(TypeCode);
            _ParameterChangeInfo.ParameterName = name;
            _ParameterChangeInfo.OldValue = value;
            IsDirty = false;
            Value = value;
            Name = name;

            if (_auswahlParameterDataService.ParameterHasAuswahlliste(name))
            {
                DropDownList = _auswahlParameterDataService.GetListeAuswahlparameter(name);
                DropDownListValue = value;
                ParameterTyp = ParameterTypValue.DropDownList;
            }
            dataImport = false;
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
                    
                    if (_ParameterChangeInfo.OldValue != value)
                    {
                        if (string.IsNullOrWhiteSpace(_ParameterChangeInfo.NewValue))
                        {
                            _ParameterChangeInfo.NewValue = value;
                        }
                        else
                        {
                            _ParameterChangeInfo.OldValue = _ParameterChangeInfo.NewValue;
                            _ParameterChangeInfo.NewValue = value;
                        }

                        IsDirty = true;
                    }

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
                if ((_Comment != null && value != _Comment) || (_Comment != null && value != ""))
                {
                    IsDirty = true;
                }
                SetProperty(ref _Comment, value);
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

        private string _DropDownListValue;
        public string DropDownListValue
        {
            get => _DropDownListValue;
            set
            {
                if (value != null && value != _DropDownListValue)
                {
                    if (value == "(keine Auswahl)")
                    {
                        value = null;
                    }
                    Value = value;
                    SetProperty(ref _DropDownListValue, value);
                }
            }
        }

        private bool _IsDirty;
        public bool IsDirty
        {
            get => _IsDirty;
            set
            {
                SetProperty(ref _IsDirty, value);
                _ParameterChangeInfo.IsDirty = value;
                if (value && !dataImport && (Value != _ParameterChangeInfo.NewValue))
                {
                    Messenger.Send(new ParameterDirtyMessage(_ParameterChangeInfo));
                }
            }
        }

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
