namespace LiftDataManager.Helpers;

public class StringToVisibilityConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(string) || string.IsNullOrWhiteSpace((string)value))
        {
            return null;
        }

        if (string.Equals((string)value, "True", StringComparison.OrdinalIgnoreCase))
        {
            return Visibility.Visible;
        }
        else if (string.Equals((string)value, "False", StringComparison.OrdinalIgnoreCase))
        {
            return Visibility.Collapsed;
        }
        else
        {
            Debug.WriteLine($"string: {value} could not be converted to Visibility");
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return "True";
        }
        return "False";
    }
}