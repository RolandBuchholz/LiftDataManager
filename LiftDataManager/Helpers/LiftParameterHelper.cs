using Cogs.Collections;

namespace LiftDataManager.Helpers;

public class LiftParameterHelper
{
    internal static dynamic GetLiftParameterValue<T>(ObservableDictionary<string, Parameter>? ParamterDictionary, string parametername)
    {
        var value = string.Empty;

        if (ParamterDictionary is not null)
        {
            value = ParamterDictionary[parametername].Value;
        }

        if (typeof(T) == typeof(int))
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : Convert.ToInt32(value, CultureInfo.CurrentCulture);
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
}
