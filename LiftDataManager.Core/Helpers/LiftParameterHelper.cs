using Cogs.Collections;
using System.Globalization;

namespace LiftDataManager.core.Helpers;

public class LiftParameterHelper
{
    public static dynamic GetLiftParameterValue<T>(ObservableDictionary<string, Parameter>? ParamterDictionary, string parametername)
    {
        var value = string.Empty;

        if (ParamterDictionary is not null)
        {
            value = ParamterDictionary[parametername].Value;
        }

        if (typeof(T) == typeof(int))
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : Convert.ToInt32(Math.Round(Convert.ToDouble(value, CultureInfo.CurrentCulture)), CultureInfo.CurrentCulture);
        }
        else if (typeof(T) == typeof(string))
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }
        else if (typeof(T) == typeof(double))
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }
        else if (typeof(T) == typeof(bool))
        {
            return !string.IsNullOrWhiteSpace(value) && Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }
        else
        {
            return string.Empty;
        }
    }

    public static string FirstCharToUpperAsSpan(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        Span<char> destination = stackalloc char[1];
        input.AsSpan(0, 1).ToUpperInvariant(destination);
        return $"{destination}{input.AsSpan(1)}";
    }

}
