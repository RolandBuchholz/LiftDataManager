namespace LiftDataManager.Helpers;
public static class XamlFunctions
{
    public static Visibility TrueToVisible(bool value)
    {
        return value
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public static Visibility FalseToVisible(bool value)
    {
        return value
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public static Visibility NotEmptyStringToVisible(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public static Visibility DoubleNot0ToVisible(double value)
    {
        return value != 0d
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public static bool NotEmptyStringToBool(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

}
