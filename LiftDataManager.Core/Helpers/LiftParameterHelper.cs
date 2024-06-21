using Cogs.Collections;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
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

        switch (typeof(T))
        {
            case var par when par == typeof(double):
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0d;
                }
                if (double.TryParse(value, out double doubleValue))
                {
                    return doubleValue;
                }
                return 0d;

            case var par when par == typeof(int):
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0;
                }
                if (int.TryParse(value, out int intValue))
                {
                    return intValue;
                }
                return 0;

            case var par when par == typeof(float):
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0f;
                }
                if (float.TryParse(value, out float floatValue))
                {
                    return floatValue;
                }
                return 0f;

            case var par when par == typeof(bool):
                return !string.IsNullOrWhiteSpace(value) && Convert.ToBoolean(value, CultureInfo.CurrentCulture);

            case var par when par == typeof(string):
                return string.IsNullOrWhiteSpace(value) ? string.Empty : value;

            default:
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
        if (dateSplitt.Length != 2)
            return string.Empty;
        if (!int.TryParse(dateSplitt[0], out int weekOfYear))
            return string.Empty;
        if (!int.TryParse(dateSplitt[1], out int year))
            return string.Empty;
        if (weekOfYear > 53)
            return string.Empty;
        if (year < 2000 || weekOfYear > 2099)
            return string.Empty;

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

    public static bool IsDefaultCarTyp(string? carTyp)
    {
        if (string.IsNullOrWhiteSpace(carTyp))
            return true;
        return string.Equals(carTyp, "C100 (aufg. Sockel)", StringComparison.CurrentCultureIgnoreCase) ||
               string.Equals(carTyp, "C200 (vers. Sockel)", StringComparison.CurrentCultureIgnoreCase);
    }

    public static double GetLayoutDrawingLoad(double load)
    {
        var loadWithSafetyLoading = load * 1.03;
        return loadWithSafetyLoading switch
        {
            < 99 => Math.Ceiling(loadWithSafetyLoading / 10) * 10,
            < 999 => loadWithSafetyLoading % 100 < 50 ? Math.Floor(loadWithSafetyLoading / 100) * 100 + 50 : Math.Ceiling(loadWithSafetyLoading / 100) * 100,
            < 9999 => loadWithSafetyLoading % 1000 < 500 ? Math.Floor(loadWithSafetyLoading / 1000) * 1000 + 500 : Math.Ceiling(loadWithSafetyLoading / 1000) * 1000,
            _ => Math.Ceiling(loadWithSafetyLoading / 1000) * 1000,
        };
        ;
    }

    public static void SetDefaultCarFrameData(ObservableDictionary<string, Parameter> parameterDictionary, CarFrameType? carFrameTyp)
    {
        if (carFrameTyp is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(parameterDictionary["var_Stichmass"].Value))
        {
            if (carFrameTyp.CarFrameDGB != 0)
            {
                parameterDictionary["var_Stichmass"].AutoUpdateParameterValue(carFrameTyp.CarFrameDGB.ToString());
            }
        }
        if (string.IsNullOrWhiteSpace(parameterDictionary["var_Stichmass_GGW"].Value))
        {
            if (carFrameTyp.CounterweightDGB != 0)
            {
                parameterDictionary["var_Stichmass_GGW"].AutoUpdateParameterValue(carFrameTyp.CounterweightDGB.ToString());
            }
        }
        if (string.IsNullOrWhiteSpace(parameterDictionary["var_Versatz_Stichmass_Y"].Value))
        {
            if (carFrameTyp.CarFrameDGBOffset != 0)
            {
                parameterDictionary["var_Versatz_Stichmass_Y"].AutoUpdateParameterValue(carFrameTyp.CarFrameDGBOffset.ToString());
            }
        }
        if (string.IsNullOrWhiteSpace(parameterDictionary["var_Versatz_Gegengewicht_Stichmass"].Value))
        {
            if (carFrameTyp.CounterweightDGBOffset != 0)
            {
                parameterDictionary["var_Versatz_Gegengewicht_Stichmass"].AutoUpdateParameterValue(carFrameTyp.CounterweightDGBOffset.ToString());
            }
        }
    }
}