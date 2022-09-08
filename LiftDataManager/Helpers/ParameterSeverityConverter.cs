namespace LiftDataManager.Helpers;

public class ParameterSeverityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(ParameterStateInfo.ErrorLevel))
        {
            return InfoBarSeverity.Success;
        }

        return value switch
        {
            ParameterStateInfo.ErrorLevel.Error => InfoBarSeverity.Error,
            ParameterStateInfo.ErrorLevel.Warning => InfoBarSeverity.Warning,
            ParameterStateInfo.ErrorLevel.Informational => InfoBarSeverity.Informational,
            _ => (object)InfoBarSeverity.Success,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}