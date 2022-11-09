namespace LiftDataManager.Helpers;

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
        if (string.IsNullOrWhiteSpace((string)value))
        {
            return "Benutzerdefinierte Variable: 0";
        }
        else if (!((string)value).StartsWith("Benutzerdefinierte Variable: "))
        {
            return "Benutzerdefinierte Variable: " + value;
        }
        return value;
    }
}
