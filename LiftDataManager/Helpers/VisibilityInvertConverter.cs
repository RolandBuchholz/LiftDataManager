namespace LiftDataManager.Helpers;

public class VisibilityInvertConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }
}