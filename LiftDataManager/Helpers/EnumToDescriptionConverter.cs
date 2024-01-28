using System.ComponentModel.DataAnnotations;
namespace LiftDataManager.Helpers;

public class EnumToDescriptionConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
            return string.Empty;

        var enumValueString = value.ToString();

        if (enumValueString is null)
            return string.Empty;

        return value.GetType().GetField(enumValueString)?.GetCustomAttributes(typeof(DisplayAttribute), false).SingleOrDefault() is not DisplayAttribute attribute ? value.ToString() : attribute.Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
