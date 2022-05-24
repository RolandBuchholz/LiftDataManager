using Microsoft.UI.Xaml.Data;
using System;

namespace LiftDataManager.Helpers
{
    public class ParameterNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.GetType() != typeof(string) || string.IsNullOrWhiteSpace((string)value))
            {
                return string.Empty;
            }
            else
            {
                return ((string)value).Replace("var_", "");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}