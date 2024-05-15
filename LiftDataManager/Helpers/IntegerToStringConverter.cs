namespace LiftDataManager.Helpers;

public class IntegerToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(int))
        {
            return null;
        }
        return System.Convert.ToString((int)value, CultureInfo.CurrentCulture);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is null || value.GetType() != typeof(string))
        {
            return 0;
        }
        if (int.TryParse((string)value, out int result))
        {
            return result;
        }
        return 0;
    }
}