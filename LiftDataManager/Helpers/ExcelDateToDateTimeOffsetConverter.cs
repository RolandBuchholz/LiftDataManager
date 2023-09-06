namespace LiftDataManager.Helpers;

public class ExcelDateToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (string.IsNullOrWhiteSpace((string)value) || value.GetType() != typeof(string) || (string)value == "0")
        {
            return null;
        }
        else
        {
            if (DateTimeOffset.TryParse(value as string, out DateTimeOffset date))
            {
                return date;
            }
            else
            {
                Debug.WriteLine($"string: {value} could not be converted to a dateTimeOffset");
                return null;
            }
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(DateTimeOffset))
        {
            return string.Empty;
        }
        else
        {
            return ((DateTimeOffset)value).ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
        }
    }
}
