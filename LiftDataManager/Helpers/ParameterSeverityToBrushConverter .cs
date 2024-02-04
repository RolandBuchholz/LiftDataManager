namespace LiftDataManager.Helpers;

public class ParameterSeverityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(ErrorLevel))
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        return value switch
        {
            ErrorLevel.Error => new SolidColorBrush(Colors.IndianRed),
            ErrorLevel.Warning => new SolidColorBrush(Colors.Orange),
            ErrorLevel.Informational => new SolidColorBrush(Colors.Gray),
            ErrorLevel.Valid => new SolidColorBrush(Colors.GreenYellow),
            _ => (object)new SolidColorBrush(Colors.Transparent),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}