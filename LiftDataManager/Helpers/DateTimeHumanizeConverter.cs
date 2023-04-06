using Humanizer;

namespace LiftDataManager.Helpers;

public class DateTimeHumanizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(DateTime))
        {
            return string.Empty;
        }

        return ((DateTime)value).Humanize();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}