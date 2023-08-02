namespace LiftDataManager.Helpers;

public class BoolToHighlightBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(bool))
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        if (parameter != null && parameter.GetType() == typeof(string) && (string)parameter == "EC")
        {
            return (bool)value ? new SolidColorBrush(Colors.YellowGreen) { Opacity = 0.2 }
                   : new SolidColorBrush(Colors.Gray) { Opacity = 0.15 };
        }

        return (bool)value ? new SolidColorBrush(Colors.YellowGreen) { Opacity = 0.2 }
                           : new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}