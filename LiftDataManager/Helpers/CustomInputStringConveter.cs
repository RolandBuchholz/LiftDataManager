using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace LiftDataManager.Helpers
{
    public class CustomInputStringConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string convertedString;

            if (((string)value) != "Benutzerdefinierte Variable: 0" && value != null)
            {
                try
                {
                    convertedString = ((string)value).Replace("Benutzerdefinierte Variable: ", "");
                    return convertedString;
                }
                catch
                {
                    Debug.WriteLine($"string: {value} could not be converted to a customString");
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string convertedString;

            if (value == null)
            {
                return string.Empty;
            }
            else if (!((string)value).StartsWith("Benutzerdefinierte Variable: "))
            {
                convertedString = "Benutzerdefinierte Variable: " + value;

                return convertedString;
            }
            return value;
        }
    }
}
