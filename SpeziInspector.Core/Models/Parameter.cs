using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;


namespace SpeziInspector.Core.Models
{
    public class Parameter : ObservableObject

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
                    double excelDate = Convert.ToDouble(_Value);
                    Date = DateTime.FromOADate(excelDate);
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
                if (Value != null && value != Value)
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
                if (Comment != null && value != Comment)
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
                if (value != IsKey)
                {
                    IsDirty = true;
                }
                SetProperty(ref _IsKey, value);
            } 
        }

        private bool _IsDirty;
        public bool IsDirty 
        {
            get => _IsDirty;
            set
            {
                SetProperty(ref _IsDirty, value);
            }
        }

        public bool IsDate { get; set; }

        public bool IsNumberOnly { get; set; }

        public bool IsString { get; set; }

        public DateTimeOffset? Date { get; set; }

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
