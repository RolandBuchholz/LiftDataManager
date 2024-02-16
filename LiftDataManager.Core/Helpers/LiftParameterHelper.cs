using Cogs.Collections;
using System.Globalization;

namespace LiftDataManager.Core.Helpers;

public class LiftParameterHelper
{
    public static dynamic GetLiftParameterValue<T>(ObservableDictionary<string, Parameter>? ParameterDictionary, string parametername)
    {
        var value = string.Empty;

        if (ParameterDictionary is not null)
        {
            value = ParameterDictionary[parametername].Value;
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

    public static string GetShortDate(string dateString)
    {
        if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, out DateTime date))
        {
            return date.ToShortDateString();
        }
        else
        {
            return string.Empty;
        }
    }

    public static string GetShortDateFromCalendarWeek(string calendarWeek)
    {
        var dateSplitt = calendarWeek.Split('/', '.', '-');
        if (dateSplitt.Length != 2) return string.Empty;
        if (!int.TryParse(dateSplitt[0], out int weekOfYear)) return string.Empty;
        if (!int.TryParse(dateSplitt[1], out int year)) return string.Empty;
        if (weekOfYear > 53) return string.Empty;
        if (year < 2000 || weekOfYear > 2099) return string.Empty;

        var jan1 = new DateTime(year, 1, 1);
        var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
        var firstThursday = jan1.AddDays(daysOffset);
        var cal = CultureInfo.CurrentCulture.Calendar;
        var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var weekNum = weekOfYear;

        if (firstWeek == 1)
        {
            weekNum -= 1;
        }
        var result = firstThursday.AddDays(weekNum * 7);
        return result.AddDays(2).ToShortDateString();
    }
}