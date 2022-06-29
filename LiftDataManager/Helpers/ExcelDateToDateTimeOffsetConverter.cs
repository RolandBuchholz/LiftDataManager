using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace LiftDataManager.Helpers
{
    public class ExcelDateToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset? date;

            if (string.IsNullOrWhiteSpace((string)value) || value.GetType() != typeof(string) || (string)value == "0")
            {
                return null;
            }
            else
            {
                try
                {
                    double excelDate = System.Convert.ToDouble(value, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    date = DateTime.FromOADate(excelDate);
                    return date;
                }
                catch
                {
                    Debug.WriteLine($"string: {value} could not be converted to a dateTimeOffset");
                    return null;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string date;

            if (value == null || value.GetType() != typeof(DateTimeOffset))
            {
                return string.Empty;
            }

            try
            {
                date = ((DateTimeOffset)value).DateTime.ToOADate().ToString(CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                return date;
            }
            catch
            {
                Debug.WriteLine($"date: {value} could not be converted to a string");
                return string.Empty;
            }
        }
    }
}
