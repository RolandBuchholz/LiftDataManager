namespace LiftDataManager.Helpers;

public class DoubleToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(double))
        {
            return null;
        }
        return System.Convert.ToString((double)value, CultureInfo.CurrentCulture);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is null || value.GetType() != typeof(string))
        {
            return 0;
        }
        return string.IsNullOrWhiteSpace((string)value) ? 0 : System.Convert.ToDouble((string)value, CultureInfo.CurrentCulture);
    }
}
