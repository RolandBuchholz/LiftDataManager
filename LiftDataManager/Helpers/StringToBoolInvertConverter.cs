﻿namespace LiftDataManager.Helpers;

public class StringToBoolInvertConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null || value.GetType() != typeof(string) || string.IsNullOrWhiteSpace((string)value))
        {
            return null;
        }

        if (string.Equals((string)value, "false", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else if (string.Equals((string)value, "true", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        else
        {
            Debug.WriteLine($"string: {value} could not be converted to a bool");
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}