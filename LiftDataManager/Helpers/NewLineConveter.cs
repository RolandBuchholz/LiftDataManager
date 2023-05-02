namespace LiftDataManager.Helpers;

public class NewLineConveter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not null and string)
        {
            return ((string)value).Replace("\r", "\n");
        }

        return string.Empty;
    }
}
