namespace LiftDataManager.Helpers;

public class ParameterSeverityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(ErrorLevel))
        {
            return InfoBarSeverity.Success;
        }

        return value switch
        {
            ErrorLevel.Error => InfoBarSeverity.Error,
            ErrorLevel.Warning => InfoBarSeverity.Warning,
            ErrorLevel.Informational => InfoBarSeverity.Informational,
            ErrorLevel.Valid => InfoBarSeverity.Success,
            _ => (object)InfoBarSeverity.Success,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}