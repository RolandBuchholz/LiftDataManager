﻿namespace LiftDataManager.Helpers;

public class EnumToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value == null || targetType == null)
            return null;

        if (!targetType.IsEnum)
            throw new ArgumentException("Target type must be an Enum.");

        if (value is string stringValue)
        {
            try
            {
                return Enum.Parse(targetType, stringValue, ignoreCase: true);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        return null;
    }
}
