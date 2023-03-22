namespace LiftDataManager.Helpers;

public class ParameterSeverityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(ParameterStateInfo.ErrorLevel))
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        return value switch
        {
            ParameterStateInfo.ErrorLevel.Error => new SolidColorBrush(Colors.IndianRed),
            ParameterStateInfo.ErrorLevel.Warning => new SolidColorBrush(Colors.Orange),
            ParameterStateInfo.ErrorLevel.Informational => new SolidColorBrush(Colors.Gray),
            ParameterStateInfo.ErrorLevel.Valid => new SolidColorBrush(Colors.GreenYellow),
            _ => (object)new SolidColorBrush(Colors.Transparent),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}